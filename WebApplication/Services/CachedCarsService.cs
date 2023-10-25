using ConsoleApp;
using Microsoft.Extensions.Caching.Memory;

namespace WebApp.Services
{
    public class CachedCarsService : ICachedService<Car>
    {
        private readonly CarDealershipContext _dbContext;
        private readonly IMemoryCache _memoryCache;

        public CachedCarsService(CarDealershipContext dbContext, IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
        }

        public IEnumerable<Car> Get(int rowsNumber = 20)
        {
            return _dbContext.Cars.Take(rowsNumber).ToList();
        }

        public void Add(string cacheKey, int rowsNumber = 20)
        {
            IEnumerable<Car> cars = _dbContext.Cars.Take(rowsNumber).ToList();
            if (cars != null)
            {
                _memoryCache.Set(cacheKey, cars, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(268)
                });

            }

        }


        public IEnumerable<Car> Get(string cacheKey, int rowsNumber = 20)
        {
            IEnumerable<Car> cars;
            if (!_memoryCache.TryGetValue(cacheKey, out cars))
            {
                cars = _dbContext.Cars.Take(rowsNumber).ToList();
                if (cars != null)
                {
                    _memoryCache.Set(cacheKey, cars,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(268)));
                }
            }
            return cars;
        }

        Position ICachedService<Car>.GetPos(int id)
        {
            throw new NotImplementedException();
        }
    }
}
