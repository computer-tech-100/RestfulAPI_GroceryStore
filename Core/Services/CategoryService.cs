using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyApp.Core.Contexts;
using MyApp.Core.Models;

namespace MyApp.Core.Services
{
    public class CategoryService: ICategoryService
    {
        private ShoppingCartContext _context;//Create object of ShoppingCartContext
        public CategoryService(ShoppingCartContext context)
        {
            _context = context;
        }
        
        public ActionResult <IEnumerable<Category>> GetCategories()
        {
            return _context.Categories.ToList();//Retrieves all Categories from database and returns list of all of the categories
        }

        public Category GetCategory(int id)
        {
            //If id exists then FirstOrDefault will get the first category that matches to the user's id and stores it in GetCategoryById 
            Category getCategoryById = _context.Categories.FirstOrDefault(p => p.CategoryId == id);
            return getCategoryById;
        }

        public async Task <Category> CreateCategory(Category category)
        {
            await _context.Categories.AddAsync(category);//Add category to Categories table
            await _context.SaveChangesAsync();//Save all the changes
            return category;         
        }  

        public async Task<Category> UpdateCategory(Category category)
        {
            Category my_category  = _context.Categories.FirstOrDefault(s => s.CategoryId == category.CategoryId);
            my_category.CategoryName = category.CategoryName;
            await _context.SaveChangesAsync();
            return my_category;

        }
        public async Task DeleteCategory(int? id)
        {
            Category category = _context.Categories.FirstOrDefault(n => n.CategoryId == id);//Find category by id
            _context.Categories.Remove(category);//Remove category
            await _context.SaveChangesAsync();//Save the changes
           
        }
    }

    public interface ICategoryService
    {
        ActionResult <IEnumerable<Category>> GetCategories();
        Category GetCategory(int id);
        Task <Category> CreateCategory(Category category);
        Task<Category> UpdateCategory(Category category);
        Task DeleteCategory(int? id);
    }
}