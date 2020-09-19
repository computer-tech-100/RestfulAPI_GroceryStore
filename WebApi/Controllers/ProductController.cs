using Microsoft.AspNetCore.Mvc; // controller class
using System.Collections.Generic;//for retrieving data from database
using System.Linq;//ToList()
using MyApp.Core.Models;
using MyApp.Core.Contexts;
using System.Threading.Tasks;//Task
using Microsoft.EntityFrameworkCore;//Include()
using MyApp.Core.Services;

namespace MyApp.WebApi.Controllers
{

   [Route("api/[controller]")]//i.e api/Product --> name of our controller
   [ApiController]//This is Api Controller
    public class ProductController: Controller
    {
        private IProductService _service;
        private ShoppingCartContext _context;//Create object of ShoppingCartContext
        
        //Constructor and dependency injection (constructor injection)
        public ProductController(ShoppingCartContext context, IProductService service)
        {
            _context = context;
            _service = service;
        }

        //Get list of all of the Products
        [HttpGet]
        public ActionResult <IEnumerable<Product>> Get()
        {
           return _service.GetProducts();
        }
  
        //GET/id
        [HttpGet("{id}")] //Get only one Product by specifiying it's id
        public ActionResult<Product> GetById(int id)
        {
            // Invalid id is negative id
            if(id <= 0)
            {
                return NotFound();
            }
            
            //If product does not exists then return NotFound()
            if(_service.GetProduct(id) == null)
            {
                return NotFound();
            }
                
            _service.GetProduct(id);
            return Ok(_service.GetProduct(id));
             
        }

        //POST
        //Add product to database
        //First check if entered data is valid if valid check the ModelState
        //If ModelState is valid then add product to database
        [HttpPost]
        public async Task<ActionResult> Post(Product product)
        {
            if (product == null)
            {
                return NotFound();
            }
            
            //When ModelState is Valid means then it is possible to correctly bind incoming values from request to the model then we add the product to database
            //Otherwise we return BadRequest()
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);//400 status code   
            }
           
            await _service.CreateProduct(product);
            return Ok(product);//Finally return the newly added Product for user to see it
        }
        
        //Put/api/product/id
        //First check if valid data is entered if valid then check if ModelState is valid
        //If ModelState is valid then check if Product exists in database if exits then update it, and save the changes
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(Product product)
        {
          
            //When entered data is not valid
            if(product == null)
            {
                return NotFound();
            }

            if(!ModelState.IsValid)
            {
              return BadRequest(ModelState);//400 status code   
            }

            Product existingProduct = _context.Products.Include(i => i.Category).FirstOrDefault(s => s.ProductId == product.ProductId);
        
            if (_service.UpdateProduct(product) == null)
            {
                return NotFound();
            }

            await _service.UpdateProduct(product);
         
            return Ok(existingProduct);//Return newly updated Product
        }

        //When we want to delete a Product first we have to check if entered id is valid or not
        //If valid then check if Product exits or not if exists then delete it
        //int? means we make id nullable
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int? id)
        {
            //Invalid data
            if(id == null)
            {
                return NotFound();
            }
       
            Product c =_context.Products.FirstOrDefault(n => n.ProductId == id);

            if(c == null)
            {
                return NotFound();
            }

            //If product exists then delete it and save the changes
            await _service.DeleteProduct(id);
            return Ok();
        }
    }
}