using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wix.Controllers;
using Wix.Models;

namespace Wix_Unit_Tests.Controllers
{
    public class StoreControllerTests : IDisposable
    {
        private readonly IMemoryCache _memoryCache;
        private readonly StoreController _controller;
        private List<StoreModel> _stores;

        public StoreControllerTests()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            _stores = new List<StoreModel>
        {
            new StoreModel { Id = "store-1", Title = "Gadget Haven", Content = "Find the latest in tech gadgets.", Views = 150, TimeStamp = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
            new StoreModel { Id = "store-2", Title = "Book World", Content = "Explore our vast collection of books.", Views = 200, TimeStamp = (int)DateTimeOffset.UtcNow.AddSeconds(-3600).ToUnixTimeSeconds() },
            new StoreModel { Id = "store-3", Title = "Fashion Hub", Content = "Your one-stop shop for the latest fashion trends.", Views = 250, TimeStamp = (int)DateTimeOffset.UtcNow.AddSeconds(-7200).ToUnixTimeSeconds() },
            new StoreModel { Id = "store-4", Title = "Home Essentials", Content = "Everything you need for a cozy home.", Views = 75, TimeStamp = (int)DateTimeOffset.UtcNow.AddSeconds(-10800).ToUnixTimeSeconds() },
            new StoreModel { Id = "store-5", Title = "Outdoor Adventures", Content = "Gear up for your next outdoor adventure.", Views = 125, TimeStamp = (int)DateTimeOffset.UtcNow.AddSeconds(-14400).ToUnixTimeSeconds() }
        };

            _memoryCache.Set("StoreData", _stores);

            _controller = new StoreController(_memoryCache);
        }

        public void Dispose()
        {
            // We need to do this to avoid POST API tests from affecting other tests
            _memoryCache.Set("StoreData", new List<StoreModel>(_stores));
        }

        [Theory]
        [InlineData("EQUAL(id,\"store-1\")", new string[] { "store-1" })]
        [InlineData("NOT(EQUAL(id,\"store-2\"))", new string[] { "store-1", "store-3", "store-4", "store-5" })]
        [InlineData("AND(EQUAL(id,\"store-2\"),GREATER_THAN(views,130))", new string[] { "store-2" })]
        [InlineData("GREATER_THAN(views,150)", new string[] { "store-2", "store-3" })]
        [InlineData("LESS_THAN(views,400)", new string[] { "store-1", "store-2", "store-3", "store-4", "store-5" })]
        [InlineData("OR(EQUAL(id,\"store-1\"),GREATER_THAN(views,200))", new string[] { "store-1", "store-3" })]
        public void GetStores_WithQuery_ReturnsExpectedResults(string query, string[] expectedIds)
        {

            var result = _controller.GetStores(query).Result as OkObjectResult;

            Assert.NotNull(result);
            var stores = result.Value as IEnumerable<StoreModel>;
            var storeIds = stores.Select(store => store.Id).ToArray();

            Assert.Equal(expectedIds.Length, storeIds.Length);
            foreach (var id in expectedIds)
            {
                Assert.Contains(id, storeIds);
            }
        }

        [Fact]
        public void GetStores_WithInvalidQuery_ReturnsBadRequest()
        {
            var invalidQuery = "OR((EQUAL(id,\"store-1\"),GREATER_THAN(views,200))";

            var result = _controller.GetStores(invalidQuery).Result;

            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        [Fact]
        public void AddStore_WithNewStore_ReturnsOkAndAddsStore()
        {
            var updatedStore = new StoreModel
            {
                Id = "store-6",
                Title = "New Store",
                Content = "Updated Content",
                Views = 150,
                TimeStamp = (int)DateTimeOffset.UtcNow.AddSeconds(-14400).ToUnixTimeSeconds()
            };


            var result = _controller.AddStore(updatedStore) as OkObjectResult;


            Assert.NotNull(result);
            Assert.Equal(updatedStore, result.Value);

            _memoryCache.TryGetValue("StoreData", out List<StoreModel> stores);
            var store = stores.Find(x => x.Id == "store-6");
            Assert.NotNull(store);
            Assert.Equal("New Store", store.Title);
        }

        [Fact]
        public void AddStore_WithExistingStoreId_UpdatesStore()
        {
            var updatedStore = new StoreModel
            {
                Id = "store-1",
                Title = "Updated Store",
                Content = "Updated Content",
                Views = 150,
                TimeStamp = (int)DateTimeOffset.UtcNow.AddSeconds(-14400).ToUnixTimeSeconds()
            };


            var result = _controller.AddStore(updatedStore) as OkObjectResult;


            Assert.NotNull(result);
            Assert.Equal(updatedStore, result.Value);

            _memoryCache.TryGetValue("StoreData", out List<StoreModel> stores);
            var store = stores.Find(x => x.Id == "store-1");
            Assert.NotNull(store);
            Assert.Equal("Updated Store", store.Title);
        }
    }
}
