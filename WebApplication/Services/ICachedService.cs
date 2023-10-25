using ConsoleApp;
using System.Threading.Tasks;

namespace WebApp.Services
{
    public interface ICachedService<T>
    {

        public IEnumerable<T> Get(int rowsNumber = 20);
        public void Add(string cacheKey, int rowsNumber = 20);
        public IEnumerable<T> Get(string cacheKey, int rowsNumber = 20);
        public Position GetPos(int id);
    }
}
