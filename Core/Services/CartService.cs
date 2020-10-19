using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Core.Contexts;
using MyApp.Core.Models;
using MyApp.Core.Models.DataTransferObjects;

namespace MyApp.Core.Services
{
    public class CartService: ICartService
    {
        private ShoppingCartContext _context;//Create object of ShoppingCartContext
        public CartService(ShoppingCartContext context)
        {
            _context = context;
        }
        
        public async Task <CartDTO> GetMyCart()
        {
            //show the list of cart items (each cart item is a Product and each Product has a Category)
             var cartItems = (await _context.CartItems.Include(i => i.Product).Include(i => i.Product.Category).ToListAsync()).Select(c => new CartItemDTO
            {
                ProductId = c.ProductId,
                Product = c.Product,
                Price = c.Price,
                Quantity = c.Quantity

            }).ToList();

            //myCart contains list of cart items and Grand Total
            CartDTO myCart = new CartDTO()//create object 
            {
            
                AllCartItems =  cartItems,
                GrandTotal = cartItems.Sum(x => x.Price * x.Quantity)//calculate Grandtotal  
            };

            return myCart;
        }
    }
    
    public interface ICartService
    {
        Task<CartDTO> GetMyCart();
    }   
}