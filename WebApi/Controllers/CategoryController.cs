using Microsoft.AspNetCore.Mvc; // controller class
using System.Collections.Generic;//for retrieving data from database / IEnumerable
using System.Linq;//ToList()
using MyApp.Core.Models;
using MyApp.Core.Contexts;
using System.Threading.Tasks;//Task
using MyApp.Core.Services;

namespace MyApp.WebApi.Controllers
{

   [Route("api/[controller]")]//i.e api/Category --> Category is name of our controller
   [ApiController]//This is Api Controller
    public class CategoryController: Controller
    {
        private ICategoryService _service;
        private ShoppingCartContext _context;//Create object of ShoppingCartContext
        
        //Constructor and dependency injection (constructor injection) to acess database and tables
        public CategoryController(ShoppingCartContext context, ICategoryService service)
        {
            _context=context;
            _service=service;
        }

        //Get list of all the categories
        [HttpGet]
        public ActionResult <IEnumerable<Category>> Get()
        {
            return _service.GetCategories();
        }

        //GET/id
        //Get only one category by specifiying it's id
        //In GetById first we check whether id is valid( i.e positive id ) or invalid (i.e negative id)
        //If id is positive then we check to see if the category that mathches to the user's id exists in database or not 
        //If it exists then return that category otherwise NotFound()
        [HttpGet("{id}")]
        public ActionResult <Category> GetById(int id)
        {
            //Negative Id is invalid
            if(id<=0)
            {
                return NotFound();
            }

            //if id is positive then we check to see if the id exists in database
            //Check to see if the user's id exists in database
            //If it exists then FirstOrDefault will get the first category that matches to the user's id and stores it in GetCategoryById 
            //Otherwise GetCategoryId will be null
            if(_service.GetCategory(id)==null)
            {
                return NotFound();
            }
            else
            { 
                return Ok(_service.GetCategory(id));
            }
        }

        //POST
        //Add Category to the database
        //Post method should be async 
        //For posting a category we need to check if the user entered the valid data 
        //If user entered valid data then we have to check if ModelState is valid or not
        //If ModelState is valid then we add Category to database
        [HttpPost]
        public async Task<ActionResult> Post (Category c)
        {
            //When user enters invalid data
            if (c == null)
            {
                return NotFound();
            }

            //When ModelState is Valid means then it is possible to correctly bind incoming values from request to the model then we add the category to database
            //Otherwise we return BadRequest()
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);//400 status code  
            }

            await _service.CreateCategory(c);
            return Ok(c);//Finally return the newly added category for user to see it
        }

        //Put/api/category/id
        //Put function should be async
        //For Put we have to check if user enters a valid data 
        //If user enteres valid data then we have to check to see if ModelState is valid or not
        //If ModelState is valid then we have to check if Category exists in database
        //If Category exists then we update it, and save the changes
        [HttpPut("{id}")]
        public async Task<ActionResult> Put (Category category)
        {
            //when user enteres an ivalid data then we return NotFound
            if( category ==null )
            {
                return NotFound();
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }
          
            if(_service.UpdateCategory(category)==null)
            {
                return NotFound();
            }

            await _service.UpdateCategory(category);
            return Ok(category);//Return newly updated Category
        }
        
        //When we want to delete a category first we have to check if entered id is valid or not
        //If valid then check if category exits or not if exists then delete it
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete (int? id)
        {
            //Invalid input
            if(id==null)
            {
                return NotFound();
            }
           
            Category category =_context.Categories.FirstOrDefault(n=>n.CategoryId==id);
            if(category==null)
            {
                return NotFound();
            }

            await _service.DeleteCategory(id);
            return Ok();    
        }
    }
}