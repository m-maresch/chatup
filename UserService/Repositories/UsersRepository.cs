using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Repositories
{
    public class UsersRepository : IRepository<User>
    {
        private readonly UsersContext _context;

        private bool _disposed;

        public UsersRepository(UsersContext context)
        {
            this._context = context;
        }

        public IQueryable<User> Get()
        {
            return _context.Users.AsQueryable();
        }

        public async Task<User> GetById(int id)
        {
            return await _context.Users
                .FindAsync(id);
        }

        public Task Insert(User entity)
        {
            _context.Users.Add(entity);
            return Task.CompletedTask;
        }

        public Task Delete(int id)
        {
            User user = _context.Users.Find(id);
            _context.Users.Remove(user);
            return Task.CompletedTask;
        }

        public Task Update(User entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this._disposed = true;
        }
    }
}
