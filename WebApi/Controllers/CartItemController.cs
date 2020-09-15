using Microsoft.AspNetCore.Mvc; // controller class
using System.Collections.Generic;//for retrieving data from database
using System.Linq;//ToList()
using MyApp.Core.Models;
using MyApp.Core.Contexts;
using System.Threading.Tasks;//Task
using Microsoft.EntityFrameworkCore;//Include()
using System;
using MyApp.Core.Services;

namespace MyApp.WebApi.Controllers
{

   [Route("api/[controller]")]//i.e api/CartItem --> name of our controller
   [ApiController]//This is Api Controller
    public class CartItemController: Controller
    {
        private ICartItemService _service;
        private ShoppingCartContext _context;//Create object of ShoppingCartContext
        
        //Constructor and dependency injection (constructor injection)
        public CartItemController(ShoppingCartContext context, ICartItemService service)
        {
            _context = context;
            _service = service;
        }

        //Get list of all CartItems 
        [HttpGet]
        public ActionResult<IEnumerable<CartItem>> GetCartItems()
        {
            return _service.GetAllCartItems();
        }
        
        //GET/id
        //Get only one CartItem by specifiying it's id 
        //(id is ProductId for a CartItem i.e a CartItem is a Product and it is recognized by ProductId)
        [HttpGet("{id}")] 
        public ActionResult<CartItem> GetById(int id)
        {
            //Invalid id is negative id
            if(id<=0)
            {
                return NotFound();
            }
             
            if(_service.GetCartItem(id)==null)
            {
                return NotFound();
            }

            _service.GetCartItem(id);
            return Ok(_service.GetCartItem(id));
         
        }
        
        //POST
        //Add CartItem to the Cart
        //First check if entered data is valid if valid check the ModelState
        //If ModelState is valid then add cartItem to the Cart
        [HttpPost]
        public async Task<ActionResult> Add_To_Cart (CartItem cartItem)
        {
            if (cartItem == null)
            {
                return NotFound();
            }

            //When ModelState is Valid means then it is possible to correctly bind incoming values from request to the model then we add the CartItem to database
            //Otherwise we return BadRequest()
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);//400 status code 
                
            }
           
            await _service.CreateCartItem(cartItem);
            return Ok(cartItem); //Finally return the newly added CartItem for user to see it
        }
 
        //Put/api/CartItem/id
        //First check if valid data is entered if valid then check if ModelState is valid
        //If ModelState is valid then check if CartItem exists in database if exits then update it, and save the changes
        [HttpPut("{id}")]
        public async Task<ActionResult> Edit_CartItem (CartItem cartItem)
        {
            
            //When entered data is not valid
            if(cartItem ==null)
            {
                return NotFound();
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);//400 status code  
            }

            if (_service.UpdateCartItem(cartItem)==null)
            {
                return NotFound();
            }

            await _service.UpdateCartItem(cartItem); 
            return Ok(cartItem);//Return newly updated CartItem
                     
        }
        
        //When we want to remove a CartItem first we have to check if entered id is valid or not
        //If valid then check if CartItem exits or not if exists then delete it
        //int? means we make id nullable
        [HttpDelete("{id}")]
        public async Task<ActionResult> Remove_A_CartItem_From_The_Cart( int? id)
        {
            //Invalid data
            if(id==null)
            {
                return NotFound();
            }
            
            //Check if cartItem exists
            CartItem c =_context.CartItems.FirstOrDefault(n=>n.ProductId==id);
            if(c==null)
            {
                return NotFound();
            }

            await _service.DeleteCartItem(id);
            return Ok();
            
        }
    }
}