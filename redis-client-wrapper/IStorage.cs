using System.Threading.Tasks;

namespace Projectr.RedisClientWrapper
{
    public interface IStorage
    {
        Task<T> GetAsync<T>(int id);
    }
}