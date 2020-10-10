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
using AutoMapper;



namespace MyApp.Core.Services
{
    public class CategoryService: ICategoryService
    {
        private ShoppingCartContext _context;//Create object of ShoppingCartContext
        public CategoryService(ShoppingCartContext context)//service references the context
        {
            _context = context;
        }
        
        public async Task<List<CategoryDTO>> GetCategories()
        {  
            //Retrieves all Categories from database and returns list of all of the categories
            //We translate our model to CategoryDTO object using Select
            return (await _context.Categories.ToListAsync()).Select(c => new CategoryDTO
            {
                CategoryId = c.CategoryId, 
                CategoryName = c.CategoryName
                
            }).ToList();
        }

        public CategoryDTO GetCategory(int id)
        {
            //If id exists then FirstOrDefault will get the first category that matches to the user's id and stores it in GetCategoryById 
            CategoryDTO getCategoryById = _context.Categories.Select(c => new CategoryDTO
            {
                CategoryId = c.CategoryId, 
                CategoryName = c.CategoryName
                
            }).FirstOrDefault(p => p.CategoryId == id);

            return getCategoryById;
        }

        public async Task <CategoryDTO> CreateCategory(CategoryDTO category)
        {
            //create new object
            Category myCategory = new Category()
            {
                //We do not pass CategoryId here becuase Identity column auto generates CategoryId While creating a new category
                //In this case we do not have collision in our database
                CategoryName = category.CategoryName
            };
            await _context.Categories.AddAsync(myCategory);//Add myCategory to Categories table

            await _context.SaveChangesAsync();//Save all the changes

            //Now when we added myCategory to database and saved the changes, then CategoryId is generated automatically
            //i.e myCategory.CategoryId is auto generated
            //we want to pass CategoryId back with our DTO
            //this is how we add CategoryId to our DTO and return it
            category.CategoryId = myCategory.CategoryId;
            
            return category;         
        }  

        public async Task<CategoryDTO> UpdateCategory(CategoryDTO category)
        {
            var my_category = _context.Categories.Single(e => e.CategoryId == category.CategoryId);
    
            my_category.CategoryName = category.CategoryName;
            
            await _context.SaveChangesAsync();
            
            return category;

        }
        public async Task DeleteCategory(int? id)
        {
            var category = _context.Categories.Single(e => e.CategoryId == id);

            _context.Categories.Remove(category);//Remove category

            await _context.SaveChangesAsync();//Save the changes
           
        }
    }

    public interface ICategoryService
    {
        Task<List<CategoryDTO>> GetCategories();
        CategoryDTO GetCategory(int id);
        Task <CategoryDTO> CreateCategory(CategoryDTO category);
        Task<CategoryDTO> UpdateCategory(CategoryDTO category);
        Task DeleteCategory(int? id);
    }
}