using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProduct productInterface) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var products = await productInterface.GetAllAsync();
            if (!products.Any())
                return  NotFound("No products detected in the database ");

            var (_, list) = ProductConversion.FromEntity(null!, products);
            return list!.Any() ? Ok(list) : NotFound("No product found");
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await productInterface.FindByIdAsync(id);
            if (product is null)
                return NotFound("Product doesn't find in database");
            var (_product, _) = ProductConversion.FromEntity(product, null!);
            return _product is null ? NotFound("No product found") : Ok(_product) ;
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO product)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var getEntity = ProductConversion.ToEntity(product);
            var response =  await productInterface.CreateAsync(getEntity);

            return response.Flag is true ? Ok(response.Flag) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO product)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var getEntity = ProductConversion.ToEntity(product);
            var response = await productInterface.UpdateAsync(getEntity);
            return response.Flag is true ? Ok(response.Flag) : BadRequest(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Response>> DeleteProduct(int id)
        {
            var product = await productInterface.FindByIdAsync(id);
            if (product is null)
                return NotFound("Product doesn't find in database");
            var response = await productInterface.DeleteAsync(product);
            return response.Flag is true ? Ok(response.Flag) : BadRequest(response);

        }
    }
}
