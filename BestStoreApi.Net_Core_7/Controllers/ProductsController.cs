using BestStoreApi.Net_Core_7.Models;
using BestStoreApi.Net_Core_7.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BestStoreApi.Net_Core_7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment env;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            this.context = context;
            this.env = env;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = context.Products;
            return Ok(products);

        }

        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        [HttpPost]
        public IActionResult CreateProduct([FromForm] ProductDto productDto)
        {
            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Image File is required");
                return BadRequest(ModelState);
            }
            //save image on the server
            string imageFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            imageFileName += Path.GetExtension(productDto.ImageFile.FileName);

            string imagesFolder = Path.Combine(env.WebRootPath, "images", "products");



            using (var stream = System.IO.File.Create(imagesFolder + imageFileName))
            {
                productDto.ImageFile.CopyTo(stream);
            }


            //save product in the database
            Product product = new Product()
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageFileName = imageFileName,
                CreatedAt = DateTime.Now
            };
            context.Products.Add(product);
            context.SaveChanges();

            return Ok();
        }


        [HttpPut("{id}")]

        public IActionResult UpdateProduct(int id, [FromForm] ProductDto productDto)
        {
            var product = context.Products.Find(id);
            if(product == null)
            {
                return NotFound();
            }

            string imageFileName = product.ImageFileName;
            if (productDto.ImageFile != null)
            {
                //save image on the server
                 imageFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");

                imageFileName += Path.GetExtension(productDto.ImageFile.FileName);

                string imagesFolder = Path.Combine(env.WebRootPath, "images", "products");

                using (var stream = System.IO.File.Create(imagesFolder + imageFileName))
                {
                    productDto.ImageFile.CopyTo(stream);
                }

                //delete image  from server
                System.IO.File.Delete(imagesFolder + product.ImageFileName);

            }

            //update the product in the database
            product.Name = productDto.Name;
            product.Brand = productDto.Brand;
            product.Category = productDto.Category;
            product.Price = productDto.Price;
            product.Description = productDto.Description?? "";
            product.ImageFileName = imageFileName;

            context.SaveChanges();

            return Ok(product);
               

        }


        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = context.Products.Find(id);

            if(product == null)
            {
                return NotFound();
            }

            //DELETE AN IMAGE ON THE SERVER

            string imagesFolder = Path.Combine(env.WebRootPath, "images", "products");
            System.IO.File.Delete(imagesFolder + product.ImageFileName);

            context.Products.Remove(product);
            context.SaveChanges();


            return Ok();

        }

    }
}
