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
        private readonly IStoreRepository _storeRepository;

        public StoreService(IStoreRepository storeRepository)
        {
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
            var stores = _storeRepository.GetAll().ToList();

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
