﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PriceIt.Data.DbContexts;
using PriceIt.Data.Interfaces;
using PriceIt.Data.Models;

namespace PriceIt.Data.Services
{
    public class ListRepository : IListRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ListRepository(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            _httpContextAccessor = httpContextAccessor;
        }

        public AppUser GetCurrentUser()
        {
            var id = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return _appDbContext.Users.FirstOrDefault(u => u.Id == id);
        }

        public List<UserList> GetUserLists(AppUser user)
        {
            var lists = _appDbContext.UserLists.Where(l => l.UserId == user.Id).Include(l => l.ListItems).ToList();

            return !lists.Any() ? new List<UserList>() : lists;
        }

        public UserList GetUserListById(int id)
        {
            return id <= 0 ? null : _appDbContext.UserLists.FirstOrDefault(l => l.UserListId == id);
        }

        public async Task AddListAsync(string listname)
        {
            try
            {
                var user = GetCurrentUser();

                var list = new UserList()
                {
                    Name = listname,
                    UserId = user.Id,
                    User = user,
                    ListItems = new List<ListItem>()
                };

                await _appDbContext.UserLists.AddAsync(list);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public bool DeleteList(int id)
        {
            var list = GetUserListById(id);

            if (list != null)
            {
                if (list.ListItems != null && list.ListItems.Any())
                {
                    _appDbContext.ListItems.RemoveRange(list.ListItems);
                }

                _appDbContext.UserLists.Remove(list);
                return Save();
            }

            return false;
        }

        public bool UpdateList(UserList list)
        {
            if (list == null) return false;

            var entity = _appDbContext.UserLists.Attach(list);
            entity.State = EntityState.Modified;

            return Save();
        }

        public bool HasAccessList(int id)
        {
            var list = _appDbContext.UserLists.FirstOrDefault(l => l.UserListId == id);
            if (list == null) return false;
            return list.UserId == GetCurrentUser().Id;
        }

        public bool Save()
        {
            return (_appDbContext.SaveChanges() >= 0);
        }
    }
}