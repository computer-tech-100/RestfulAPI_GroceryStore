using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Core.Contexts;
using MyApp.Core.Models.DataTransferObjects;
using MyApp.Core.Models.DbEntities;

namespace MyApp.Core.Services
{
    public class ProductService: IProductService
    {
        private ShoppingCartContext _context;//Create object of ShoppingCartContext
        public ProductService(ShoppingCartContext context)
        {
            _context = context;
        }
        
        public async Task<List<ProductDTO>> GetProducts()
        {
            //Retrieves all products from database and returns list of all of the products including the related category
            //We translate our model to ProductDTO object using Select
            return (await _context.Products.Include(i => i.Category).ToListAsync()).Select(c => new ProductDTO
            {
                ProductId = c.ProductId,
                ProductName = c.ProductName,
                Price = c.Price,
                CategoryId = c.CategoryId,
                Category = c.Category

            }).ToList();
        }

        public ProductDTO GetProduct(int id)
        {
            //If id exists then FirstOrDefault will get the first Product that matches to the user's id and stores it in MyProduct
            ProductDTO myProduct = _context.Products.Include(i => i.Category).Select(c => new ProductDTO
            {
                ProductId = c.ProductId,
                ProductName = c.ProductName,
                Price = c.Price,
                CategoryId = c.CategoryId,
                Category = c.Category
                
            }).FirstOrDefault(p => p.ProductId == id);
            
            return myProduct;
        }

        public async Task CreateProduct(ProductDTO product)
        {

            Product myProduct = new Product()
            {
                //ProductId is Auto generated
                ProductName = product.ProductName,
                Price = product.Price,
                CategoryId = product.CategoryId,
                Category = product.Category
            };
        
            await _context.Products.AddAsync(myProduct);//Add Product to Products table

            await _context.SaveChangesAsync();//Save all the changes

            await _context.Products.Include(i => i.Category).ToListAsync();
      
            product.ProductId = myProduct.ProductId;

            product.Category = myProduct.Category;//this makes the Category not become null
        
        }  

        public async Task<ProductDTO> UpdateProduct(ProductDTO product)
        {

            var existingProduct = _context.Products.Include(i => i.Category).Single(e => e.ProductId == product.ProductId);
    
            existingProduct.ProductName = product.ProductName;

            existingProduct.Price = product.Price; 

            product.Category = existingProduct.Category;
            
            await _context.SaveChangesAsync();
            
            return product;
        }
        public async Task DeleteProduct(int? id)
        {
            //Check if Product exists
            var product = _context.Products.Single(e => e.ProductId == id);

            _context.Products.Remove(product);//Remove product

            await _context.SaveChangesAsync();//Save the changes
        }
    }

    public interface IProductService
    {
        Task<List<ProductDTO>> GetProducts();
        ProductDTO GetProduct(int id);
        Task CreateProduct(ProductDTO product);
        Task<ProductDTO> UpdateProduct(ProductDTO product);
        Task DeleteProduct(int? id);
    }
}