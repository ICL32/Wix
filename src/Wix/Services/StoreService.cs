using Microsoft.Extensions.Caching.Memory;
using Wix.Models;
using Wix.QueryLanguage;
using Wix.Repository;

namespace Wix.Services
{
    public interface IStoreService
    {
        IEnumerable<StoreModel> GetAllStores();
        StoreModel GetStoreById(string id);
        void AddStore(StoreModel store);
        void UpdateStore(string id, StoreModel updatedStore);
        void DeleteStore(string id);
        IEnumerable<StoreModel> GetStoresByQuery(string query);
    }

    public class StoreService : IStoreService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IStoreRepository _storeRepository;
        private static readonly string storeCacheKey = "StoreData";

        public StoreService(IMemoryCache memoryCache, IStoreRepository storeRepository)
        {
            _memoryCache = memoryCache;
            _storeRepository = storeRepository;
        }

        public IEnumerable<StoreModel> GetAllStores()
        {
            return _storeRepository.GetAll();
        }

        public StoreModel GetStoreById(string id)
        {
            return _storeRepository.GetById(id);
        }

        public void AddStore(StoreModel store)
        {
            var existingStore = _storeRepository.GetAll()
                .FirstOrDefault(s => s.Title.Equals(store.Title, StringComparison.OrdinalIgnoreCase));

            if (existingStore != null)
            {
                throw new InvalidOperationException($"A store with the title '{store.Title}' already exists.");
            }

            _storeRepository.Add(store);
        }

        public void UpdateStore(string id, StoreModel updatedStore)
        {
            var existingStore = _storeRepository.GetById(id);
            if (existingStore == null)
            {
                throw new KeyNotFoundException($"Store with ID '{id}' not found.");
            }

            // Update properties (excluding Id)
            existingStore.Title = updatedStore.Title;
            existingStore.Content = updatedStore.Content;
            existingStore.Views = updatedStore.Views;
            existingStore.TimeStamp = updatedStore.TimeStamp;

            _storeRepository.Update(existingStore);
        }

        public void DeleteStore(string id)
        {
            if (!_storeRepository.Delete(id))
            {
                throw new KeyNotFoundException($"Store with ID '{id}' not found.");
            }
        }

        public IEnumerable<StoreModel> GetStoresByQuery(string query)
        {
            if (!_memoryCache.TryGetValue(storeCacheKey, out List<StoreModel> stores))
            {
                stores = new List<StoreModel>();
            }

            if (string.IsNullOrEmpty(query))
            {
                return stores;
            }

            var parser = new ExpressionParser();
            var predicate = parser.ParseExpression<StoreModel>(query);
            return stores.AsQueryable().Where(predicate.Compile()).ToList();
        }
    }

}
