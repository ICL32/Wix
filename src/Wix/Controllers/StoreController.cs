using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Wix.Models;
using Wix.Services;
using Wix.ViewModels;

namespace Wix.Controllers
{
    [ApiController]
    [Route("api/store")]
    public class StoreController : Controller
    {
        private readonly IStoreService _storeService;

        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet("query")]
        [EnableQuery]
        public ActionResult<IEnumerable<StoreModel>> GetStores([FromQuery] string query)
        {
            try
            {
                var stores = _storeService.GetStoresByQuery(query);
                return Ok(stores);
            }
            catch (Exception ex)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Bad Request",
                    Detail = ex.Message
                };
                return BadRequest(problemDetails);
            }
        }

        [HttpGet]
        public ActionResult<StoreViewModel> GetStores()
        {
            var stores = _storeService.GetAllStores();
            var viewModel = new StoreViewModel
            {
                Stores = stores
            };
            return Ok(viewModel);
        }

        [HttpGet("{id}")]
        public ActionResult<StoreModel> GetStoreById(string id)
        {
            var store = _storeService.GetStoreById(id);
            if (store == null)
            {
                return NotFound();
            }
            return Ok(store);
        }

        [HttpPost]
        public ActionResult AddStore([FromBody] StoreModel store)
        {
            if (store == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _storeService.AddStore(store);
                return CreatedAtAction(nameof(GetStoreById), new { id = store.Id }, store);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public ActionResult<StoreModel> UpdateStore(string id, [FromBody] StoreModel updatedStore)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _storeService.UpdateStore(id, updatedStore);
                return Ok(updatedStore);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteStore(string id)
        {
            try
            {
                _storeService.DeleteStore(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
