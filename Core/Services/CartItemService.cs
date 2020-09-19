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
    public class CartItemService: ICartItemService
    {
        private ShoppingCartContext _context;//Create object of ShoppingCartContext
        public CartItemService(ShoppingCartContext context)
        {
            _context = context;
        }
        
        public ActionResult<IEnumerable<CartItem>> GetAllCartItems()
        {
            //Retrieves all products (along with their related Category) that are CartItems  
            List <CartItem> myCart = _context.CartItems.Include(i => i.Product).ToList();
            List <Product> myProduct = _context.Products.Include(i => i.Category).ToList();//Each Product must show it's related category
            return myCart; 
        }

        public CartItem GetCartItem(int id)
        {
           //check if CartItem exists in database (CartItem is a Product Each Product has its related category)
            CartItem my_CartItem = _context.CartItems.Include(i => i.Product).FirstOrDefault(p => p.ProductId == id);
            List <Product> MyProduct = _context.Products.Include(i => i.Category).ToList();//Each Product should show it's related Category
            return my_CartItem;
        }

        public async Task <CartItem> CreateCartItem(CartItem cartItem)
        {
            await _context.CartItems.AddAsync(cartItem); //Add CartItem to CartItems table
            await _context.SaveChangesAsync();//Save all the changes 
            List <CartItem> my_CartItem = _context.CartItems.Include(i => i.Product).ToList();
            List <Product> myProduct = _context.Products.Include(i => i.Category).ToList();//Each Product should show it's related Category
            return cartItem;   
        }  

        public async Task<CartItem> UpdateCartItem(CartItem cartItem)
        {
            CartItem existingCartItem = _context.CartItems.Include(i => i.Product).FirstOrDefault(s => s.ProductId == cartItem.ProductId);
            List <Product> product = _context.Products.Include(i => i.Category).ToList();
            existingCartItem.Quantity = cartItem.Quantity;
            await _context.SaveChangesAsync();
            return existingCartItem;
        }
        public async Task DeleteCartItem(int? id)
        {
            CartItem cartItem = _context.CartItems.FirstOrDefault(n => n.ProductId == id);
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();  
        }
    }

    public interface ICartItemService
    {
        ActionResult <IEnumerable<CartItem>> GetAllCartItems();
        CartItem GetCartItem(int id);
        Task <CartItem> CreateCartItem(CartItem cartItem);
        Task <CartItem> UpdateCartItem(CartItem cartItem);
        Task DeleteCartItem(int? id);
    }
}