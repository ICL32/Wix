using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Wix_Technical_Test.Models;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Query;

namespace Wix_Technical_Test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreController : ODataController
    {
        private readonly IMemoryCache _memoryCache;
        private static readonly string storeCacheKey = "StoreData";

        public StoreController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        // GET: api/store
        [HttpGet]
        [EnableQuery]
        public ActionResult<IEnumerable<StoreModel>> GetStores()
        {
            if (!_memoryCache.TryGetValue(storeCacheKey, out List<StoreModel> stores))
            {
                stores = new List<StoreModel>();
            }

            var filterCriteria = Request.Query["filter"];

            IQueryable<StoreModel> filteredStores = stores.AsQueryable();


            return new OkObjectResult(stores.AsQueryable());
        }

        // POST: api/store
        [HttpPost]
        public IActionResult AddStore([FromBody] StoreModel newStore)
        {
            if (!_memoryCache.TryGetValue(storeCacheKey, out List<StoreModel> stores))
            {
                stores = new List<StoreModel>();
            }

            int existingStoreIndex = stores.FindIndex(x => x.Id == newStore.Id);
            if (existingStoreIndex != -1)
            {
                stores[existingStoreIndex] = newStore;
            }
            else
            {
                stores.Add(newStore);
            }

            _memoryCache.Set(storeCacheKey, stores);

            return new OkObjectResult(newStore);
        }
    }
}
