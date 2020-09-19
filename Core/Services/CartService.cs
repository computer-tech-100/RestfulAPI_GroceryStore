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
            _context = context;
        }
        
        public Cart GetMyCart()
        {
           //show the list of cart items (each cart item is a Product and each Product has a Category)
            List <CartItem> cart_Items = _context.CartItems.Include(i => i.Product).ToList();
            List <Product> myProduct = _context.Products.Include(i => i.Category).ToList();

            Cart myCart = new Cart//create object 
            {
                //MyCart contains list of cart items and Grand Total
                AllCartItems = cart_Items,
                GrandTotal = cart_Items.Sum(x => x.Price * x.Quantity)//calculate Grandtotal  
            };

            return myCart;
        }
    }
    
    public interface ICartService
    {
        Cart GetMyCart();
    }   
}