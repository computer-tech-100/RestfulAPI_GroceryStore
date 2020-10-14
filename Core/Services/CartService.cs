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
        
        public CartDTO GetMyCart()
        {/*
            //show the list of cart items (each cart item is a Product and each Product has a Category)
            var cart_Items = _context.CartItems.Include(i => i.Product).ToList().Select(c => new CartItemDTO
            {
                ProductId = c.ProductId,
                Product = c.Product,
                Price = c.Price,
                Quantity = c.Quantity

            }).ToList();
            */
            
           /*
            var myProduct = _context.Products.Include(i => i.Category).ToList().Select(c => new ProductDTO
            {
                ProductId = c.ProductId,
                ProductName = c.ProductName,
                Price = c.Price,
                CategoryId = c.CategoryId,
                Category = c.Category

            }).ToList();
            */
             List<CartItemDTO> cart_Items = new List<CartItemDTO>();

           

            CartDTO myCart = new CartDTO()//create object 
            {
                //MyCart contains list of cart items and Grand Total
                
                AllCartItems =  cart_Items,
                GrandTotal = cart_Items.Sum(x => x.Price * x.Quantity)//calculate Grandtotal  
            };

            return myCart;
        }
    }
    
    public interface ICartService
    {
        CartDTO GetMyCart();
    }   
}