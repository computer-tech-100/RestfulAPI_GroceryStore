using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Core.Contexts;
using MyApp.Core.Models;

namespace MyApp.Core.Services
{
    public class CartService: ICartService
    {
        private ShoppingCartContext _context;//Create object of ShoppingCartContext
        public CartService(ShoppingCartContext context)
        {
            _context=context;
        }
        
        public Cart GetCart()
        {
           //show the list of cart items (each cart item is a Product and each Product has a Category)
            var Cart_Items =_context.CartItems.Include(i=>i.Product).ToList();
            var MyProduct =_context.Products.Include(i=>i.Category).ToList();

            Cart MyCart = new Cart//create object 
            {
                //MyCart contains list of cart items and Grand Total
                AllCartItems = Cart_Items,
                GrandTotal = Cart_Items.Sum(x => x.Price * x.Quantity)//calculate Grandtotal  
            };

            return MyCart;
        }
    }
    
    public interface ICartService
    {
        Cart GetCart();
    }   
}