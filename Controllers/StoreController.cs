using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Wix.Models;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Query;
using Wix.QueryLanguage;

namespace Wix.Controllers
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

        [HttpGet]
        [EnableQuery]
        public ActionResult<IEnumerable<StoreModel>> GetStores([FromQuery] string query)
        {
            if (!_memoryCache.TryGetValue(storeCacheKey, out List<StoreModel> stores))
            {
                stores = new List<StoreModel>();
                // You might want to populate the cache here if it's empty.
                // PopulateInitialStoreData(...);
            }

            if (string.IsNullOrEmpty(query))
            {
                // If no query is provided, return all stores.
                return Ok(stores);
            }

            try
            {
                var parser = new ExpressionParser();
                var predicate = parser.ParseExpression<StoreModel>(query);

                var result = stores.AsQueryable().Where(predicate.Compile()).ToList();

                // Return the filtered list
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
