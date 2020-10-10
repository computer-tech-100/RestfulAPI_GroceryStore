using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Core.Contexts;
using MyApp.Core.Models;
using MyApp.Core.Models.DataTransferObjects;
using MyApp.Core.Models.DbEntities;

namespace MyApp.Core.Services
{
    public class CartItemService: ICartItemService
    {
        private ShoppingCartContext _context;//Create object of ShoppingCartContext
        public CartItemService(ShoppingCartContext context)
        {
            _context = context;
        }
        
        public async Task<List<CartItemDTO>> GetAllCartItems()
        {
            //Retrieves all products (along with their related Category) that are CartItems 
            //We translate our model to CartItemDTO object using Select
            var myCartItem = (await _context.CartItems.Include(i => i.Product).ToListAsync()).Select(c => new CartItemDTO
            {
                ProductId = c.ProductId,
                Product = c.Product,
                Price = c.Price,
                Quantity = c.Quantity

            }).ToList();
           
            var myProduct = (await _context.Products.Include(i => i.Category).ToListAsync()).Select(c => new ProductDTO
            {
                ProductId = c.ProductId,
                ProductName = c.ProductName,
                Price = c.Price,
                CategoryId = c.CategoryId,
                Category = c.Category

            }).ToList();

            return myCartItem; 
        }
        

        public CartItemDTO GetCartItem(int id)
        {
           //check if CartItem exists in database (CartItem is a Product Each Product has its related category)
            var myCartItem = _context.CartItems.Include(i => i.Product).Select(c => new CartItemDTO
            {
                ProductId = c.ProductId,
                Product = c.Product,
                Price = c.Price,
                Quantity = c.Quantity

            }).FirstOrDefault(p => p.ProductId == id);

            var myProduct = _context.Products.Include(i => i.Category).Select(c => new ProductDTO
            {
                ProductId = c.ProductId,
                ProductName = c.ProductName,
                Price = c.Price,
                CategoryId = c.CategoryId,
                Category = c.Category

            }).ToList();

            //CartItem my_CartItem = _context.CartItems.Include(i => i.Product).FirstOrDefault(p => p.ProductId == id);
            //List <Product> MyProduct = _context.Products.Include(i => i.Category).ToList();//Each Product should show it's related Category
            return myCartItem;
        }

        public async Task <CartItemDTO> CreateCartItem(CartItemDTO cartItem)
        {
            CartItem myCartItem = new CartItem()
            {
                Product = cartItem.Product,
                Price = cartItem.Price,
                Quantity = cartItem.Quantity
            };
            await _context.CartItems.AddAsync(myCartItem); //Add CartItem to CartItems table

            await _context.SaveChangesAsync();//Save all the changes 

            List <CartItem> my_CartItem = _context.CartItems.Include(i => i.Product).ToList();

            List <Product> myProduct = _context.Products.Include(i => i.Category).ToList();//Each Product should show it's related Category

            cartItem.ProductId = myCartItem.ProductId;

            return cartItem;   
        }  

        public async Task<CartItemDTO> UpdateCartItem(CartItemDTO cartItem)
        {
            var existingCartItem= _context.CartItems.Include(i => i.Product).Single(e => e.ProductId == cartItem.ProductId);
    
            List <Product> product = _context.Products.Include(i => i.Category).ToList();

            existingCartItem.Quantity = cartItem.Quantity;

            await _context.SaveChangesAsync();

            return cartItem;

        }
        public async Task DeleteCartItem(int? id)
        {
            var cartItem = _context.CartItems.Single(e => e.ProductId == id);

            _context.CartItems.Remove(cartItem);

            await _context.SaveChangesAsync();  
        }
    }

    public interface ICartItemService
    {
        Task<List<CartItemDTO>> GetAllCartItems();

        CartItemDTO GetCartItem(int id);

        Task <CartItemDTO> CreateCartItem(CartItemDTO cartItem);

        Task <CartItemDTO> UpdateCartItem(CartItemDTO cartItem);

        Task DeleteCartItem(int? id);

    }
}