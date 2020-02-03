using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Extensions;
using UserService.Models;

namespace UserService.Repositories
{
    public class UsersRepositoryWithCache : IRepository<User>
    {
        protected UsersRepository usersRepository;

        public UsersRepositoryWithCache(UsersRepository usersRepository)
        {
            this.usersRepository = usersRepository;
        }

        public IQueryable<User> Get()
        {
            return usersRepository.Get();
        }

        public async Task<User> GetById(int id)
        {
            return ((string)await RedisStore.Cache
                       .StringGetAsync(id.ToString()))
                   .ToObject<User>() ?? await usersRepository.GetById(id);
        }

        public async Task Insert(User entity)
        {
            await usersRepository.Insert(entity);
            await RedisStore.Cache.StringSetAsync(entity.UserID.ToString(), entity.ToJson());
        }

        public async Task Delete(int id)
        {
            await usersRepository.Delete(id);
            await RedisStore.Cache.KeyDeleteAsync(id.ToString());
        }

        public async Task Update(User entity)
        {
            await usersRepository.Update(entity);
            await RedisStore.Cache.StringSetAsync(entity.UserID.ToString(), entity.ToJson());
        }

        public async Task Save()
        {
            await usersRepository.Save();
        }

        public void Dispose()
        {
            usersRepository.Dispose();
        }
    }
}
