using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using Wix.Controllers;
using Wix.Models;
using Wix.Services;
using Wix.ViewModels;
using Xunit;

namespace Wix_Unit_Tests.Controllers
{
    public class StoreControllerTests
    {
        private readonly Mock<IStoreService> _mockStoreService;
        private readonly StoreController _controller;

        public StoreControllerTests()
        {
            _mockStoreService = new Mock<IStoreService>();
            _controller = new StoreController(_mockStoreService.Object);
        }



        [Fact]
        public void AddStore_ValidStore_ReturnsCreatedStore()
        {
            var store = new StoreModel { Id = "store-2", Title = "New Store", Content = "New Content", Views = 150, TimeStamp = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds() };
            _mockStoreService.Setup(service => service.AddStore(store)).Verifiable();

            var actionResult = _controller.AddStore(store);

            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
            Assert.Equal(store, createdResult.Value);
            _mockStoreService.Verify(service => service.AddStore(store), Times.Once);
        }

        [Fact]
        public void AddStore_InvalidStore_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Title", "Title is required");
            var store = new StoreModel { Id = "store-3", Title = "", Content = "Content without title" };

            var actionResult = _controller.AddStore(store);

            Assert.IsType<BadRequestObjectResult>(actionResult);
        }


        [Fact]
        public void DeleteStore_ExistingId_ReturnsNoContent()
        {
            _mockStoreService.Setup(service => service.DeleteStore("store-1")).Verifiable();

            var actionResult = _controller.DeleteStore("store-1");

            Assert.IsType<NoContentResult>(actionResult);
            _mockStoreService.Verify(service => service.DeleteStore("store-1"), Times.Once);
        }

        [Fact]
        public void DeleteStore_NonExistingId_ReturnsNotFound()
        {
            _mockStoreService.Setup(service => service.DeleteStore("store-99")).Throws(new KeyNotFoundException());

            var actionResult = _controller.DeleteStore("store-99");

            Assert.IsType<NotFoundObjectResult>(actionResult);
        }


    }
}
