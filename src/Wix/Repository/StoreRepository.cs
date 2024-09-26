using Microsoft.Extensions.Caching.Memory;
using Wix.Models;

namespace Wix.Repository
{
    public interface IStoreRepository
    {
        IEnumerable<StoreModel> GetAll();
        StoreModel? GetById(string id);
        void Add(StoreModel store);
        void Update(StoreModel store);
        bool Delete(string id);
    }

    public class StoreRepository : IStoreRepository
    {
        private readonly IMemoryCache _memoryCache;
        private const string CacheKey = "StoreData";

        public StoreRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public IEnumerable<StoreModel> GetAll()
        {
            if (_memoryCache.TryGetValue(CacheKey, out List<StoreModel> stores))
            {
                return stores;
            }
            return new List<StoreModel>();
        }

        public StoreModel? GetById(string id)
        {
            var stores = GetAll().ToList();
            return stores.FirstOrDefault(s => s.Id == id);
        }

        public void Add(StoreModel store)
        {
            var stores = GetAll().ToList();
            stores.Add(store);
            _memoryCache.Set(CacheKey, stores);
        }

        public void Update(StoreModel store)
        {
            var stores = GetAll().ToList();
            var existingStore = stores.FirstOrDefault(s => s.Id == store.Id);
            if (existingStore != null)
            {
                stores.Remove(existingStore);
                stores.Add(store);
                _memoryCache.Set(CacheKey, stores);
            }
        }

        public bool Delete(string id)
        {
            var stores = GetAll().ToList();
            var store = stores.FirstOrDefault(s => s.Id == id);
            if (store != null)
            {
                stores.Remove(store);
                _memoryCache.Set(CacheKey, stores);
                return true;
            }
            return false;
        }
    }
}
