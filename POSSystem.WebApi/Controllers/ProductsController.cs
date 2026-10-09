using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POSSystem.Application.Services;
using POSSystem.Application.DTOs;
using POSSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace POSSystem.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<IReadOnlyList<Product>> GetAllProducts([FromQuery] ProductSearchRequest request)
        {
            var products = _productService.SearchProducts(request.Search, request.SortBy, request.SortOrder);
            return Ok(products);
        }

        [HttpGet("{code}")]
        [AllowAnonymous]
        public ActionResult<Product> GetProductByCode(string code)
        {
            var product = _productService.GetProductByCode(code);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        [Authorize(Policy = "EmployeeOnly")]
        public ActionResult AddProduct([FromBody] AddProductRequest request)
        {

            var product = new Product(0, request.Code, request.Name, request.Price, request.Stock);
            try
            {
                _productService.AddProduct(product);

            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }


            return CreatedAtAction(
                nameof(GetProductByCode),
                new { code = product.Code },
                product
            );
        }

        [HttpPut("{code}")]
        [Authorize(Policy = "EmployeeOnly")]
        public ActionResult UpdateProduct(string code, [FromBody] UpdateProductRequest request)
        {
            var product = _productService.GetProductByCode(code);

            if (product == null)
            {
                return NotFound();
            }
            
            product.Update(request.Name, request.Price, request.Stock);

            _productService.UpdateProduct(product);

            return Ok(product);
        }

        [HttpDelete("{code}")]
        [Authorize(Policy = "EmployeeOnly")]
        public ActionResult DeleteProduct(string code)
        {
            var deleted = _productService.DeleteProduct(code);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
