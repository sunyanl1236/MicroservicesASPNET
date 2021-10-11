using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    // Route becomes to api/v1/catalog (remove the controller after)
    public class CatalogController : ControllerBase
    {
        // Repository is the middle layer, controller use it to change the data in database
        private readonly IProductRepository _repository;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductRepository repository, ILogger<CatalogController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        // ProducesResponseType: producess response. 
        // ProducesResponseTypeAttribute(Int32) or ProducesResponseTypeAttribute(Type, Int32)
        // this method must return ActionResult
        // since methods in repository is async, so use async here
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts() {
            var products = await _repository.GetProducts();
            return Ok(products);
        }

        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> GetProductById(string id) {
            var product = await _repository.GetProduct(id);
            if (product == null) {
                _logger.LogError($"Product with id: {id}, not found.");
                return NotFound();
            }
            return Ok(product);
        }

        // if exist more than one HttpGet, can separate then by parameter in HttpGet or Route
        [Route("[action]/{category}", Name = "GetProductByCategory")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category) {
            var products = await _repository.GetProductsByCategory(category);
            return Ok(products);
        }

        // [FromBody] get product info from http request body
        [HttpPost]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> CreateProduct([FromBody] Product product) {
            await _repository.CreateProduct(product);
            // CreatedAtRoute(string routeName, object routeValues, object content)
            // Creates a CreatedAtRouteResult (201 Created) with the specified values and redirect to url
            // in this case, redirect to GetProductById method
            // routeName: The name of the route to use for generating the URL.
            // routeValues: The route data to use for generating the URL.
            // content: The content value to format in the entity body.
            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        // if don't have specific ActionResult, use IActionResult
        public async Task<IActionResult> UpdateProduct([FromBody] Product product) {
            return Ok(await _repository.UpdateProduct(product));
        }

        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteProduct(string id) {
            return Ok(await _repository.DeleteProduct(id));
        }
    }
}
