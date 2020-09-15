using Microsoft.AspNetCore.Mvc; // controller class
using System.Collections.Generic;//for retrieving data from database
using System.Linq;//ToList()
using MyApp.Core.Models;
using MyApp.Core.Contexts;
using System.Threading.Tasks;//Task
using Microsoft.EntityFrameworkCore;//Include()
//using static MyApp.Core.Services.CartService;
using MyApp.Core.Services;

namespace MyApp.WebApi.Controllers
{

   [Route("api/[controller]")]//i.e api/Cart --> name of our controller
   [ApiController]//This is Api Controller
    public class CartController: Controller
    {
        private ICartService _service;
        private ShoppingCartContext _context;//Create object of ShoppingCartContext
        
        //Constructor and dependency injection (constructor injection)
        public CartController(ShoppingCartContext context, ICartService service)
        {
            _context = context;
            _service = service;
        }
        
        //Get list of all the Products inside cart
       [HttpGet]
        public Cart GetCart()
        {
            return _service.GetCart();
        }     
    }
}