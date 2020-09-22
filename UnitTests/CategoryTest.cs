using Xunit;
using System.Collections.Generic;
using MyApp.WebApi.Controllers;
using MyApp.Core.Models;
using MyApp.Core.Contexts;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using MyApp.Core.Services;
using Moq;

//The app should fail gracefully
//Consider all possible aspects that user : test cases with all possible input and output
namespace MyApp.UnitTests
{
    public class TestCategory
    {
        //Entity Framework creates only one IServiceProvider for all of the contexts 
        //Hence our context is going to share same InMemory database
        //We want to get our context not to be shared between the tests
        //For this purpose we have to create a new context for each test
        //Like this : using (var Context = new ShoppingCartContext(CreateNewContext())) { here we implement the AAA pattern }
        private static DbContextOptions<ShoppingCartContext> CreateNewContext()
        {
            //Create a new service provider and new InMemory database 
            ServiceProvider myServiceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

            //Context uses InMemory database and the new service provider 
            DbContextOptionsBuilder <ShoppingCartContext> my_Builder = new DbContextOptionsBuilder<ShoppingCartContext>();
            my_Builder.UseInMemoryDatabase("Data Source=MyShoppingCart.db")
            .UseInternalServiceProvider(myServiceProvider);
            return my_Builder.Options;
        }

        //Test Get() Method
        [Fact]
        public void Get_WhenCalled_ReturnsAllCategories()
        {
            using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Mock <ICategoryService> moqRepo = new Mock <ICategoryService>();//Mock is type of our Interface
                context.CategoryTestData();//We make sure that dummy data has been added
                moqRepo.Setup(repo => repo.GetCategories()).Returns(context.Categories);//access the function inside the service class and specify what it returns
                CategoryController controller = new CategoryController(context, moqRepo.Object);//pass context and moq object inside controller

                //Act
                ActionResult<IEnumerable<Category>> results = controller.Get();//call Get() function inside Category controller

                //Assert
                Assert.NotNull(results);//make sure that Get Method returns value
            }
        }

        //Test GetById() Method
        //When valid Id is passed
        [Fact]
        public void GetById_ExistingIntIdPassed_ReturnsOkResult()
        {
            using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Mock <ICategoryService> moqRepo = new Mock <ICategoryService>(); //Mock is type of our Interface
                context.CategoryTestData();//We make sure that dummy data has been added
                moqRepo.Setup(repo => repo.GetCategory(1)).Returns(context.Categories.FirstOrDefault(i => i.CategoryId == 1));//access the function inside the service class and specify what it returns
                CategoryController controller = new CategoryController(context, moqRepo.Object);//pass context and moq object inside controller
            
                //Act
                ActionResult <Category> OkResult = controller.GetById(1);//1 is valid Id 
            
                //Assert
                Assert.IsType<OkObjectResult>(OkResult.Result);//When Id is valid the result is type of OkObjectResult
            }
        }

        //Test GetById() Method
        //When Invalid Id is passed
        [Fact]
        public void GetById_InvalidIdPassed_ReturnsNotFoundResult()//check if invalid Id is passed
        {
            using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Mock <ICategoryService> moqRepo = new Mock <ICategoryService>(); //Mock is type of our Interface
                context.CategoryTestData();//We make sure that dummy data has been added
                moqRepo.Setup(repo => repo.GetCategory(-1)).Returns(context.Categories.FirstOrDefault(i => i.CategoryId == -1));//access the function inside the service class and specify what it returns
                CategoryController controller = new CategoryController(context, moqRepo.Object);//pass context and moq object inside controller

                //Act
                ActionResult <Category> not_Found_Result = controller.GetById(-1);//-1 is Invalid Id

                //Assert
                Assert.IsType<NotFoundResult>(not_Found_Result.Result);
            }
        }
     
        //Test GetById() Method
        //When GetById() returns the correct item
        [Fact]
        public void GetById_ExistingIntIdPassed_ReturnsRightItem()
        {
            using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Mock <ICategoryService> moqRepo = new Mock <ICategoryService>();//Mock is type of our Interface
                context.CategoryTestData();//We make sure that dummy data has been added
                moqRepo.Setup(repo => repo.GetCategory(1)).Returns(context.Categories.FirstOrDefault(i => i.CategoryId == 1));//access the function inside the service class and specify what it returns
                CategoryController controller = new CategoryController(context, moqRepo.Object);//pass context and moq object inside controller
            
                //Act
                OkObjectResult okResult = controller.GetById(1).Result as OkObjectResult;
            
                //Assert
                Assert.Equal("Clothes", (okResult.Value as Category).CategoryName);
            }
        }

        //Test Post() Method 
        //When Invalid object is passed 
        [Fact]
        public async Task CategoryModelValidation_CategoryNameRequired()
        {
            using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Mock <ICategoryService> moqRepo = new Mock <ICategoryService>();//Mock is type of our Interface

                //This Category does not contain CategoryName hence the Category is invalid
                Category categoryNameIsMissing = new Category()
                {
                    CategoryId = 1,
                    //CategoryName = "Fruits" //--> is missing here
                };

                moqRepo.Setup(Repo=>Repo.CreateCategory(categoryNameIsMissing));
                CategoryController controller = new CategoryController(context, moqRepo.Object);//pass context and moq object inside controller
                controller.ModelState.AddModelError("CategoryName","Required");

                //Act
                ActionResult result = await controller.Post(categoryNameIsMissing);

                //Assert
                BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
                Assert.IsType<SerializableError>(badRequest.Value);  
            }
        }

        //Test Post() Method 
        //When valid object is passed 
        [Fact]
        public async Task Post_ValidObject_ReturnsOkResult()
        {
            using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Mock <ICategoryService> moqRepo = new Mock <ICategoryService>();//Mock is type of our Interface
                context.CategoryTestData();//We make sure that dummy data has been added

                Category testData = new Category()
                {
                    CategoryId = 2, 
                    CategoryName = "Items" 
                };

                moqRepo.Setup(repo => repo.CreateCategory(testData));//access the function inside the service class and specify what it returns

                CategoryController controller = new CategoryController(context, moqRepo.Object);//pass context and moq object inside controller
               
                //Act
                ActionResult createdResponse = await controller.Post(testData);
            
                //Assert
                Assert.IsType <OkObjectResult> (createdResponse);
            }
        }

        //Test Put() Method
        //When Non Existing Category is Passed 
        [Fact]
        public async Task Put_NotExistingCategoryPassed_ReturnsNotFoundResponse()
        {
            using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Mock <ICategoryService> moqRepo = new Mock <ICategoryService>();//Mock is type of our Interface
                moqRepo.Setup(repo => repo.UpdateCategory(null));//access the function inside the service class and specify what it returns
                CategoryController controller = new CategoryController(context, moqRepo.Object);//pass context and moq object inside controller
            
                //Act
                ActionResult badResponse = await controller.Put(null);//non existing Category is passed
            
                //Assert
                Assert.IsType <NotFoundResult>( badResponse);
            }
        }

        //Test Put() Method
        //When Existing Category is Passed 
        [Fact]
        public async Task Put_ExistingCategoryPassed_ReturnsOkResult()
        {
            using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Mock <ICategoryService> moqRepo = new Mock <ICategoryService>();//Mock is type of our Interface
                Category  category = context.CategoryTestData();
                moqRepo.Setup(repo => repo.UpdateCategory(category));//access the function inside the service class and specify what it returns
                CategoryController controller = new CategoryController(context, moqRepo.Object);//pass context and moq object inside controller
            
                //Act
                ActionResult okResponse = await controller.Put(category);//existing Category is passed
            
                //Assert
                Assert.IsType <OkObjectResult> (okResponse);
            }
        }
        
        //Test Delete() Method
        //When null is Passed
        [Fact]
        public async Task Remove_NullPassed_ReturnsNotFoundResponse()
        {
            using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Mock <ICategoryService> moqRepo = new Mock <ICategoryService>();//Mock is type of our Interface
                moqRepo.Setup(repo => repo.DeleteCategory(null));//access the function inside the service class and specify what it returns
                CategoryController controller = new CategoryController(context, moqRepo.Object);//pass context and moq object inside controller
       
                //Act
                ActionResult badResponse = await controller.Delete(null);

                //Assert
                Assert.IsType<NotFoundResult>(badResponse);
            }
        }

        //Test Delete() Method
        //When Non Existing Id is Passed
        [Fact]
        public async Task Remove_NotExistingIntIdPassed_ReturnsNotFoundResponse()
        {
            using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Mock <ICategoryService> moqRepo = new Mock <ICategoryService>();//Mock is type of our Interface
                context.CategoryTestData();//We make sure that dummy data has been added
                moqRepo.Setup(repo => repo.DeleteCategory(11));//access the function inside the service class and specify what it returns
                CategoryController controller = new CategoryController(context, moqRepo.Object);//pass context and moq object inside controller
            
                //Act
                ActionResult badResponse = await controller.Delete(11);
            
                //Assert
                Assert.IsType<NotFoundResult>(badResponse);
            }
        }

        //Test Delete() Method
        //When Existing Id is Passed
        [Fact]
        public async Task Remove_ExistingIntIdPassed_ReturnsOkResult()
        {
            using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Mock <ICategoryService> moqRepo = new Mock <ICategoryService>();//Mock is type of our Interface
                context.CategoryTestData();//We make sure that dummy data has been added
                moqRepo.Setup(repo => repo.DeleteCategory(1));//access the function inside the service class and specify what it returns
                CategoryController controller = new CategoryController(context, moqRepo.Object);//pass context and moq object inside controller
                
                //Act
                ActionResult okResponse = await controller.Delete(1);

                //Assert
                Assert.IsType<OkResult>(okResponse);
            }          
        }
    }
}