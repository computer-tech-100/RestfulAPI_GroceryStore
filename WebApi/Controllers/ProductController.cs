using Microsoft.AspNetCore.Mvc; // controller class
using System.Collections.Generic;//for retrieving data from database
using System.Linq;//ToList()
using MyApp.Core.Models;
using MyApp.Core.Contexts;
using System.Threading.Tasks;//Task
using Microsoft.EntityFrameworkCore;//Include()

namespace MyApp.WebApi.Controllers
{

   [Route("api/[controller]")]//i.e api/Product --> name of our controller
   [ApiController]//This is Api Controller
    public class ProductController: Controller
    {
        private ShoppingCartContext x;//Create object of ShoppingCartContext
        
        //Constructor and dependency injection (constructor injection)
        public ProductController(ShoppingCartContext y)
        {
            x=y;
        }

        //Get list of all of the Products
        [HttpGet]
        public ActionResult<IEnumerable<Product>>Get()
        {
            //Retrieves all products from database and returns list of all of the products including the related category
            return x.Products.Include(i=>i.Category).ToList();
        }
  
        //GET/id
        [HttpGet("{id}")] //Get only one Product by specifiying it's id
        public ActionResult<Product>GetById(int id)
        {
            // Invalid id is negative id
            if(id<=0)
            {
                return NotFound();
            }

            //If id is valid (i.e positive) then we check if the Product exists in database
            else
            {
                //check if product exists in database 
                Product MyProduct =x.Products.Include(i=>i.Category).FirstOrDefault(p=>p.ProductId==id);

                //If product does not exists then return NotFound()
                if(MyProduct==null)
                {
                    return NotFound();
                }
                //If product exists then return it
                else
                {
                    return Ok(MyProduct);
                }
            }   
        }

        //POST
        //Add product to database
        //First check if entered data is valid if valid check the ModelState
        //If ModelState is valid then add product to database
        [HttpPost]
        public async Task<ActionResult>Post(Product P)
        {
            if (P == null)
            {
                return NotFound();
            }
            
            //When ModelState is Valid means then it is possible to correctly bind incoming values from request to the model then we add the product to database
            //Otherwise we return BadRequest()
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);//400 status code   
            }
           
            await x.Products.AddAsync(P); //Add Product to Products table
            await x.SaveChangesAsync();//Save all the changes
            return Ok(P); //Finally return the newly added Product for user to see it
        }
        
        //Put/api/product/id
        //First check if valid data is entered if valid then check if ModelState is valid
        //If ModelState is valid then check if Product exists in database if exits then update it, and save the changes
        [HttpPut("{id}")]
        public async Task<ActionResult>Put(Product c)
        {
            //When entered data is not valid
            if(c ==null)
            {
                return NotFound();
            }
            //When valid data is entered, we check if ModelState is valid
            else
            {
                if(ModelState.IsValid)
                {
                    //Check if Product exists or not
                    Product existingProduct=x.Products.Include(i=>i.Category).FirstOrDefault(s=>s.ProductId==c.ProductId);

                    if (existingProduct==null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        //Edit the properties of Product
                        existingProduct.ProductName=c.ProductName;
                        existingProduct.Price=c.Price;
                        await x.SaveChangesAsync();//Save the changes
                        return Ok(existingProduct);//Return newly updated Product
                    }
                }
                //When ModelState is not valid then we return BadRequest()
                else
                {
                    return BadRequest(ModelState);//400 status code   
                }
            }
        }

        //When we want to delete a Product first we have to check if entered id is valid or not
        //If valid then check if Product exits or not if exists then delete it
        //int? means we make id nullable
        [HttpDelete("{id}")]
        public async Task<ActionResult>Delete( int? id)
        {
            //Invalid data
            if(id==null)
            {
                return NotFound();
            }
            else
            {
                //Check if Product exists
                Product c =x.Products.FirstOrDefault(n=>n.ProductId==id);

                if(c==null)
                {
                    return NotFound();
                }
                //If product does exists then delete it and save the changes
                else
                {
                    x.Products.Remove(c);
                    await x.SaveChangesAsync();
                    return Ok();
                }
            }
        }
    }
}