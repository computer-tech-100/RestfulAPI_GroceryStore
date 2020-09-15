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
            var MyServiceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

            //Context uses InMemory database and the new service provider 
            var My_Builder = new DbContextOptionsBuilder<ShoppingCartContext>();
            My_Builder.UseInMemoryDatabase("Data Source=MyShoppingCart.db")
            .UseInternalServiceProvider(MyServiceProvider);
            return My_Builder.Options;
        }

        //Test Get() Method
        [Fact]
        public void Get_WhenCalled_ReturnsAllCategories()
        {
            using (var context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                context.CategoryTestData();//We make sure that dummy data has been added
                var controller = new CategoryController(context, new CategoryService(context) );//pass context and CategoryService inside controller

                //Act
                var results = controller.Get();//call Get() function inside Category controller

                //Assert
                Assert.NotNull(results);//make sure that Get Method returns value
            }
        }

        //Test GetById() Method
        //When valid Id is passed
        [Fact]
        public void GetById_ExistingIntIdPassed_ReturnsOkResult()
        {
            using (var context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                context.CategoryTestData();
                var controller = new CategoryController(context, new CategoryService(context));//pass context and CategoryService inside controller
            
                //Act
                var OkResult = controller.GetById(1);//1 is valid Id 
            
                //Assert
                Assert.IsType<OkObjectResult>(OkResult.Result);//When Id is valid the result is type of OkObjectResult
            }
        }

        //Test GetById() Method
        //When Invalid Id is passed
        [Fact]
        public void GetById_InvalidIdPassed_ReturnsNotFoundResult()//check if invalid Id is passed
        {
            using (var context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                     var controller = new CategoryController(context, new CategoryService(context));//pass context and CategoryService inside controller

                //Act
                var not_Found_Result = controller.GetById(-1);//-1 is Invalid Id

                //Assert
                Assert.IsType<NotFoundResult>(not_Found_Result.Result);
        
            }
        }
     
        //Test GetById() Method
        //When GetById() returns the correct item
        [Fact]
        public void GetById_ExistingIntIdPassed_ReturnsRightItem()
        {
            using (var context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                context.CategoryTestData();
                var controller = new CategoryController(context, new CategoryService(context));//pass context and CategoryService inside controller
            
                //Act
                var okResult = controller.GetById(1).Result as OkObjectResult;
            
                //Assert
                Assert.Equal("Clothes", (okResult.Value as Category).CategoryName);
            }
        }

        //Test Post() Method 
        //When Invalid object is passed 
        [Fact]
        public void CategoryModelValidation_CategoryNameRequired()
        {
            using (var Context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                List<ValidationResult> result = new List<ValidationResult>(); 

                //This Category does not contain CategoryName hence the Category is invalid
                Category categoryNameIsMissing = new Category()
                {
                    CategoryId= 1,
                    //CategoryName = "Fruits" //--> is missing here
                };
            
                //Act
                bool isValid = Validator.TryValidateObject(categoryNameIsMissing, new ValidationContext(categoryNameIsMissing), result);
               
                //Assert
                Assert.False(isValid);
                Assert.Equal(1, result.Count);//one error 
                Assert.Equal("CategoryName", result[0].MemberNames.ElementAt(0)); 
                Assert.Equal("The CategoryName field is required.", result[0].ErrorMessage); 
            }
        }

        //Test Post() Method 
        //When valid object is passed 
        [Fact]
        public async Task Post_ValidObject_ReturnsOkResult()
        {
            using (var context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                var controller = new CategoryController(context, new CategoryService(context));//pass context and CategoryService inside controller
                Category TestData = new Category()
                {
                    CategoryId = 2, 
                    CategoryName= "Items" 
                };
            
                //Act
                var createdResponse = await controller.Post(TestData);
            
                //Assert
                Assert.IsType<OkObjectResult>(createdResponse);
            }
        }

        //Test Put() Method
        //When Non Existing Category is Passed 
        [Fact]
        public async Task Put_NotExistingCategoryPassed_ReturnsNotFoundResponse()
        {
            using (var context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                     var controller = new CategoryController(context, new CategoryService(context));//pass context and CategoryService inside controller
            
                //Act
                var badResponse = await controller.Put(null);//non existing Category is paased
            
                //Assert
                Assert.IsType<NotFoundResult>(badResponse);
            }
        }

        //Test Put() Method
        //When Existing Category is Passed 
        [Fact]
        public async Task Put_ExistingCategoryPassed_ReturnsOkResult()
        {
            using (var context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                     var controller = new CategoryController(context, new CategoryService(context) );//pass context and CategoryService inside controller
                Category  C =context.CategoryTestData();
            
                //Act
                var okResponse = await controller.Put(C);//existing Category is passed
            
                //Assert
                Assert.IsType<OkObjectResult>(okResponse);
            }
        }
        
        //Test Delete() Method
        //When null is Passed
        [Fact]
        public async Task Remove_NullPassed_ReturnsNotFoundResponse()
        {
            using (var context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                var controller = new CategoryController(context, new CategoryService(context) );//pass context and CategoryService inside controller
            
                //Act
                var badResponse = await controller.Delete(null);

                //Assert
                Assert.IsType<NotFoundResult>(badResponse);
            }
        }

        //Test Delete() Method
        //When Not Existing Id is Passed
        [Fact]
        public async Task Remove_NotExistingIntIdPassed_ReturnsNotFoundResponse()
        {
            using (var context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                var controller = new CategoryController(context, new CategoryService(context) );//pass context and CategoryService inside controller
            
                //Act
                var badResponse = await controller.Delete(11);
            
                //Assert
                Assert.IsType<NotFoundResult>(badResponse);
            }
        }

        //Test Delete() Method
        //When Existing Id is Passed
        [Fact]
        public async Task Remove_ExistingIntIdPassed_ReturnsOkResult()
        {
            using (var context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                context.CategoryTestData();
                var controller = new CategoryController(context, new CategoryService(context) );//pass context and CategoryService inside controller
                
                //Act
                var okResponse = await controller.Delete(1);

                //Assert
                Assert.IsType<OkResult>(okResponse);
            }          
        }
    }
}