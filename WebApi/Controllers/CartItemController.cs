using Microsoft.AspNetCore.Mvc; // controller class
using System.Collections.Generic;//for retrieving data from database
using System.Linq;//ToList()
using MyApp.Core.Models;
using MyApp.Core.Contexts;
using System.Threading.Tasks;//Task
using Microsoft.EntityFrameworkCore;//Include()
using System;

namespace MyApp.WebApi.Controllers
{

   [Route("api/[controller]")]//i.e api/CartItem --> name of our controller
   [ApiController]//This is Api Controller
    public class CartItemController: Controller
    {
        private ShoppingCartContext x;//Create object of ShoppingCartContext
        
        //Constructor and dependency injection (constructor injection)
        public CartItemController(ShoppingCartContext y)
        {
            x=y;
        }

        //Get list of all CartItems 
        [HttpGet]
        public ActionResult<IEnumerable<CartItem>>GetCartItems()
        {
            //Retrieves all products (along with their related Category) that are CartItems  
            var MyCart =x.CartItems.Include(i=>i.Product).ToList();
            var MyProduct =x.Products.Include(i=>i.Category).ToList();//Each Product must show it's related category
            return MyCart;    
        }
        
        //GET/id
        //Get only one CartItem by specifiying it's id 
        //(id is ProductId for a CartItem i.e a CartItem is a Product and it is recognized by ProductId)
        [HttpGet("{id}")] 
        public ActionResult<CartItem>GetById(int id)
        {
            //Invalid id is negative id
            if(id<=0)
            {
                return NotFound();
            }
            //If id is valid (i.e positive) then we check if the CartItem exists in database
            else
            {
                //check if CartItem exists in database (CartItem is a Product Each Product has its related category)
                var My_CartItem =x.CartItems.Include(i=>i.Product).FirstOrDefault(p=>p.ProductId==id);
                var MyProduct =x.Products.Include(i=>i.Category).ToList();//Each Product should show it's related Category
                if(My_CartItem==null)
                {
                    return NotFound();
                }

                //If CartItem exists then return it
                else
                {
                    return Ok(My_CartItem);
                }
            }      
        }
        
        //POST
        //Add CartItem to the Cart
        //First check if entered data is valid if valid check the ModelState
        //If ModelState is valid then add cartItem to the Cart
        [HttpPost]
        public async Task<ActionResult>Add_To_Cart(CartItem c)
        {
            if (c == null)
            {
                return NotFound();
            }

            //When ModelState is Valid means then it is possible to correctly bind incoming values from request to the model then we add the CartItem to database
            //Otherwise we return BadRequest()
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);//400 status code 
                
            }
           
            await x.CartItems.AddAsync(c); //Add CartItem to CartItems table
            await x.SaveChangesAsync();//Save all the changes
            return Ok(c); //Finally return the newly added CartItem for user to see it
        }
 
        //Put/api/CartItem/id
        //First check if valid data is entered if valid then check if ModelState is valid
        //If ModelState is valid then check if CartItem exists in database if exits then update it, and save the changes
        [HttpPut("{id}")]
        public async Task<ActionResult>Edit_CartItem(CartItem c)
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
                    //Check if CartItem exists or not
                    CartItem existingCartItem=x.CartItems.Include(i=>i.Product).FirstOrDefault(s=>s.ProductId==c.ProductId);
                    var p=x.Products.Include(i=>i.Category).ToList();

                    if (existingCartItem==null)
                    {
                        return NotFound();
                    }
                    else 
                    {
                        //Edit Quantity 
                        existingCartItem.Quantity=c.Quantity;
                        await x.SaveChangesAsync();//Save the changes
                        return Ok(existingCartItem);//Return newly updated CartItem
                    }
                }
                    //When ModelState is not valid then we return BadRequest()
                else
                {
                    return BadRequest(ModelState);//400 status code   
                }
            }
        }
        
        //When we want to remove a CartItem first we have to check if entered id is valid or not
        //If valid then check if CartItem exits or not if exists then delete it
        //int? means we make id nullable
        [HttpDelete("{id}")]
        public async Task<ActionResult>Remove_A_CartItem_From_The_Cart( int? id)
        {
            //Invalid data
            if(id==null)
            {
                return NotFound();
            }
            else
            {
                //Check if cartItem exists
                CartItem c =x.CartItems.FirstOrDefault(n=>n.ProductId==id);
                if(c==null)
                {
                    return NotFound();
                }
                //If CartItem exists then delete it and save the changes
                else
                {
                    x.CartItems.Remove(c);
                    await x.SaveChangesAsync();
                    return Ok();
                }
            }
        }
    }
}