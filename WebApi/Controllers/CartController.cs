using Microsoft.AspNetCore.Mvc; // controller class
using System.Collections.Generic;//for retrieving data from database
using System.Linq;//ToList()
using MyApp.Core.Models;
using MyApp.Core.Contexts;
using System.Threading.Tasks;//Task
using Microsoft.EntityFrameworkCore;//Include()

namespace MyApp.WebApi.Controllers
{

   [Route("api/[controller]")]//i.e api/Cart --> name of our controller
   [ApiController]//This is Api Controller
    public class CartController: Controller
    {
        private ShoppingCartContext x;//Create object of ShoppingCartContext
        
        //Constructor and dependency injection (constructor injection)
        public CartController(ShoppingCartContext y)
        {
            x=y;
        }
        
        //Get list of all the Products inside cart
       [HttpGet]
        public ActionResult<IEnumerable<Cart>>GetCart()
        {
           //show the list of cart items (each cart item is a Product and each Product has a Category)
            var Cart_Items =x.CartItems.Include(i=>i.Product).ToList();
            var MyProduct =x.Products.Include(i=>i.Category).ToList();

            Cart MyCart = new Cart//create object 
            {
                //MyCart contains list of cart items and Grand Total
                AllCartItems = Cart_Items,
                GrandTotal = Cart_Items.Sum(x => x.Price * x.Quantity)//calculate Grandtotal  
            };
            
            return Ok(MyCart);
        }     
    }
}