using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationService.Repositories
{
    public interface IRepository<T> : IDisposable
    {
        IQueryable<T> Get();
        Task<T> GetById(int id);
        Task Insert(T entity);
        Task Delete(int id);
        Task Update(T entity);
        Task Save();
    }
}
