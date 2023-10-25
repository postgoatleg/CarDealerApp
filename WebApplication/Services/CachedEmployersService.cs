using ConsoleApp;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace WebApp.Services
{
    public class CachedEmployersService : ICachedService<Employee>
    {
        private readonly CarDealershipContext _dbContext;
        private readonly IMemoryCache _memoryCache;

        public CachedEmployersService(CarDealershipContext dbContext, IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
        }
        public void Add(string cacheKey, int rowsNumber = 20)
        {
            IEnumerable<Employee> employees= _dbContext.Employees.Take(rowsNumber).ToList();
            Debug.WriteLine("метод");
            foreach (var e in employees)         
            {
                Debug.WriteLine("выполняется");
                e.Position = _dbContext.Positions.Where(p => p.PositionId == e.PositionId).Single();
            }
            if (employees != null)
            {
                _memoryCache.Set(cacheKey, employees, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(268)
                });

            }
        }

        

        public IEnumerable<Employee> Get(int rowsNumber = 20)
        {
            return _dbContext.Employees.Take(rowsNumber).ToList();
        }

        public IEnumerable<Employee> Get(string cacheKey, int rowsNumber = 20)
        {
            IEnumerable<Employee> employers;
            if (!_memoryCache.TryGetValue(cacheKey, out employers))
            {
                employers = _dbContext.Employees.Take(rowsNumber).ToList();
                foreach(var e in employers)
                {
                    Debug.WriteLine("выполняется");
                    e.Position = _dbContext.Positions.Where(p => p.PositionId == e.PositionId).FirstOrDefault();
                }
                
                if (employers != null)
                {
                    _memoryCache.Set(cacheKey, employers,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(268)));
                }
            }
            return employers;
        }

        public Position GetPos(int id)
        {
            return _dbContext.Positions.Where(p => p.PositionId == id).Single();
        }
    }
}
