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
    public class ProductService: IProductService
    {
        private ShoppingCartContext _context;//Create object of ShoppingCartContext
        public ProductService(ShoppingCartContext context)
        {
            _context = context;
        }
        
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            //Retrieves all products from database and returns list of all of the products including the related category
            return _context.Products.Include(i => i.Category).ToList();
        }

        public Product GetProduct(int id)
        {
            //If id exists then FirstOrDefault will get the first Product that matches to the user's id and stores it in MyProduct 
            Product myProduct = _context.Products.Include(i => i.Category).FirstOrDefault(p => p.ProductId == id);
            return myProduct;
        }

        public async Task <Product> CreateProduct(Product product)
        {
            await _context.Products.AddAsync(product);//Add Product to Products table
            await _context.SaveChangesAsync();//Save all the changes
            _context.Products.Include(i => i.Category).ToList();
            return product;         
        }  

        public async Task<Product> UpdateProduct(Product product)
        {
            Product existingProduct = _context.Products.Include(i => i.Category).FirstOrDefault(s => s.ProductId == product.ProductId);
            existingProduct.ProductName = product.ProductName;
            existingProduct.Price = product.Price; 
            await _context.SaveChangesAsync();
            return existingProduct;      
        }
        public async Task DeleteProduct(int? id)
        {
            //Check if Product exists
            Product c = _context.Products.FirstOrDefault(n => n.ProductId == id);
            _context.Products.Remove(c);
            await _context.SaveChangesAsync();  
        }
    }

    public interface IProductService
    {
        ActionResult <IEnumerable<Product>> GetProducts();
        Product GetProduct(int id);
        Task <Product> CreateProduct(Product product);
        Task<Product> UpdateProduct(Product product);
        Task DeleteProduct(int? id);
    }
}