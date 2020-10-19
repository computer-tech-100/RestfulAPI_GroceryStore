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
            var myCartItem = (await _context.CartItems.Include(i => i.Product).Include(i => i.Product.Category).ToListAsync()).Select(c => new CartItemDTO
            {
                ProductId = c.ProductId,
                Product = c.Product,
                Price = c.Price,
                Quantity = c.Quantity

            }).ToList();

            return myCartItem; 
        }
        
        public CartItemDTO GetCartItem(int id)
        {
           //check if CartItem exists in database (CartItem is a Product Each Product has its related category)
            var myCartItem = _context.CartItems.Include(i => i.Product).Include(x => x.Product.Category).Select(c => new CartItemDTO
            {
                ProductId = c.ProductId,
                Product = c.Product,
                Price = c.Price,
                Quantity = c.Quantity

            }).FirstOrDefault(p => p.ProductId == id);

            return myCartItem;
        }

        public async Task CreateCartItem(CartItemDTO cartItem)
        {
            CartItem myCartItem = new CartItem()
            {
                ProductId =cartItem.ProductId,
                Product = cartItem.Product,
                Price = cartItem.Price,
                Quantity = cartItem.Quantity
            };
            await _context.CartItems.AddAsync(myCartItem);//Add CartItem to CartItems table

            await _context.SaveChangesAsync();//Save all the changes 

            await _context.CartItems.Include(i => i.Product).Include(i => i.Product.Category).ToListAsync();//shows the Product entity(with it's Category) inside CartItem 
           
            cartItem.ProductId = myCartItem.ProductId;

            cartItem.Product = myCartItem.Product;//this makes product not to be null in postman
  
        }  

        public async Task<CartItemDTO> UpdateCartItem(CartItemDTO cartItem)
        {
            
            //check if CartItem exists in database (CartItem is a Product Each Product has its related category)
           
            var existingCartItem = _context.CartItems.Include(i => i.Product).Include(i=> i.Product.Category).Single(e => e.ProductId == cartItem.ProductId);
    
            existingCartItem.Quantity = cartItem.Quantity;

            cartItem.Product = existingCartItem.Product;//To show Product entity in postman

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

        Task CreateCartItem(CartItemDTO cartItem);

        Task <CartItemDTO> UpdateCartItem(CartItemDTO cartItem);

        Task DeleteCartItem(int? id);

    }
}