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
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

//The app should fail gracefully
//Consider all possible aspects that user : test cases with all possible input and output
namespace MyApp.UnitTests
{

    public class TestProduct
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
        public void Get_WhenCalled_ReturnsAllProducts()
        {
            using (var Context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Context.ProductTestData();//We make sure that dummy data has been added
                var Controller = new ProductController(Context);//pass context inside controller

                //Act
                var Results = Controller.Get();//call Get() function inside Procuct controller

                //Assert
                Assert.NotNull(Results);//make sure that Get Method returns value 
            }
        }

        //Test GetById() Method
        //When valid Id is passed
        [Fact]
        public void GetById_ExistingIntIdPassed_ReturnsOkResult()
        {
            using (var Context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Context.ProductTestData();//add dummy data
                var Controller = new ProductController(Context);//pass Context inside controller
            
                //Act
                var OkResult = Controller.GetById(1);//1 is valid Id 
            
                //Assert
                Assert.IsType<OkObjectResult>(OkResult.Result);//When Id is valid the result is type of OkObjectResult
            }
        }

        //Test GetById() Method
        //When Invalid Id is passed
        [Fact]
        public void GetById_InvalidIdPassed_ReturnsNotFoundResult() 
        {
            using (var Context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                var Controller = new ProductController(Context);//pass context inside controller

                //Act
                var Not_Found_Result = Controller.GetById(-1);// -1 is Invalid Id

                //Assert
                Assert.IsType<NotFoundResult>(Not_Found_Result.Result);
            }
        }

        //Test GetById() Method
        //When GetById() returns the correct item
        [Fact]
        public void GetById_ExistingIntIdPassed_ReturnsRightItem()
        {
            using (var Context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Context.ProductTestData();//Add dummy data
                var Controller = new ProductController(Context);//pass context inside controller

                //Act
                var OkResult = Controller.GetById(1).Result as OkObjectResult;
            
                //Assert
                Assert.Equal("Oranges", (OkResult.Value as Product).ProductName);  
            }
        }

        //Test Post() Method 
        //When Invalid object is passed 
        [Fact]
        public void ProductModelValidation_ProductRequired()
        {
            using (var Context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                List<ValidationResult> result = new List<ValidationResult>(); 

                //This Product does not contain Category hence the Product is invalid
                var CategoryIsMissing = new Product()
                {
                    ProductId=2,
                    ProductName="TShirts",
                    Price=23
                };
            
                //Act
                bool isValid = Validator.TryValidateObject(CategoryIsMissing, new ValidationContext(CategoryIsMissing), result);
               
                //Assert
                Assert.False(isValid);
                Assert.Equal(1, result.Count);//one error 
                Assert.Equal("Category", result[0].MemberNames.ElementAt(0)); 
                Assert.Equal("The Category field is required.", result[0].ErrorMessage); 
            }
        }

        //Test Post() Method 
        //When valid object is passed
        [Fact]
        public async Task Post_ValidObject_ReturnsOkResult()
        {
            using (var Context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                var _Controller = new ProductController(Context);//pass context inside controller
                Category CategoryTestData = new Category()
                {
                    CategoryId = 1, 
                    CategoryName= "Clothes"
                
                };
                Product P = new Product()
                {
                    
                    ProductId = 2,
                    ProductName="TShirts",
                    Price=23,
                    CategoryId=1,
                    Category=CategoryTestData
                };
            
                //Act
                var CreatedResponse = await _Controller.Post(P);
            
                //Assert
                Assert.IsType<OkObjectResult>(CreatedResponse);
            }
        }

        //Test Put() Method
        //When Non Existing Product is Passed
        [Fact]
        public async Task Put_NotExistingProductPassed_ReturnsNotFoundResponse()
        {
            using (var Context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                var _Controller = new ProductController(Context);//pass context inside controller
            
                //Act
                var BadResponse = await _Controller.Put(null);//non existing Product is paased
            
                //Assert
                Assert.IsType<NotFoundResult>(BadResponse);
            }
        }

        //Test Put() Method
        //When Existing Product is Passed
        [Fact]
        public async Task Put_ExistingProductPassed_ReturnsOkResult()
        {
            using (var Context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Product X=Context.ProductTestData();
                var _Controller = new ProductController(Context);//pass context inside controller
            
                //Act
                var OkResponse = await _Controller.Put(X);//existing Product is passed

                //Assert
                Assert.IsType<OkObjectResult>(OkResponse);
            }
        }

        //Test Delete() Method
        //When null is Passed
        [Fact]
        public async Task Remove_NullPassed_ReturnsNotFoundResponse()
        {
            using (var Context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                var _Controller = new ProductController(Context);//pass context inside controller
            
                //Act
                var BadResponse = await _Controller.Delete(null);//When null is passed
        
                //Assert
                Assert.IsType<NotFoundResult>(BadResponse);
            }
        }

        //Test Delete() Method
        //When Non Existing Id is Passed
        [Fact]
        public async Task Remove_NotExistingIntIdPassed_ReturnsNotFoundResponse()
        {
            using (var Context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                var _Controller = new ProductController(Context);//pass context inside controller
            
                //Act
                var BadResponse = await _Controller.Delete(10);//When Non existing Id is passed
            
                //Assert
                Assert.IsType<NotFoundResult>(BadResponse);
            }
        }

        //Test Delete() Method
        //When Existing Id is Passed
        [Fact]
        public async Task Remove_ExistingIntIdPassed_ReturnsOkResult()
        {
            using (var Context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Context.ProductTestData();//Add dummy data
            
                var _Controller = new ProductController(Context);//pass context inside controller
        
                //Act
                var OkResponse1 = await _Controller.Delete(1);
        
                //Assert
                Assert.IsType<OkResult>(OkResponse1);
            }
        }
    }
}