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
namespace MyApp.UnitTests{
    public class TestCartItem
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
        public void Get_WhenCalled_ReturnsAllCartItems()
        {
            using (var Context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Context.CartItemTestData();//We make sure that dummy data has been added
                var Controller = new CartItemController(Context);//pass context inside controller

                //Act
                var Results = Controller.GetCartItems();//call Get() function inside CartItem controller

                //Assert
                Assert.NotNull(Results);//make sure that Get Method reurns value 
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
                Context.CartItemTestData();//Add dummy data
                var Controller = new CartItemController(Context);//pass Context inside controller

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
                var Controller = new CartItemController(Context);//pass context inside controller

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
                // Arrange
                Context.CartItemTestData();
                var Controller = new CartItemController(Context);//pass context inside controller
            
                //Act
                var OkResult = Controller.GetById(1).Result as OkObjectResult;
            
                //Assert
                Assert.Equal(5, (OkResult.Value as CartItem).Quantity);
            }
        }

        //Test Post() Method 
        //When Invalid object is passed 
        [Fact]
        public void CartItemModelValidation_ProductRequired()
        {
            using (var Context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                List<ValidationResult> result = new List<ValidationResult>(); 

                //This CartItem does not contain Product field hence the CartItem is invalid
                var ProductIsMissing = new CartItem()
                {
                    //Product is missing here hence this is not a valid CartItem
                    Price=2,
                    Quantity=5           
                };
            
                //Act
                bool isValid = Validator.TryValidateObject(ProductIsMissing, new ValidationContext(ProductIsMissing), result);
               
                //Assert
                Assert.False(isValid);
                Assert.Equal(1, result.Count);//one error 
                Assert.Equal("Product", result[0].MemberNames.ElementAt(0)); 
                Assert.Equal("The Product field is required.", result[0].ErrorMessage); 
            }
        }

        //Test Post() Method
        //When valid Object is Passed
        [Fact]
        public async Task Post_ValidObject_ReturnsOkResult()
        {
            using (var Context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                var _Controller = new CartItemController(Context);//pass context inside controller

                Category The_Category_Test_Data = new Category()
                {
                    CategoryId = 2,
                    CategoryName="Items"
                };
                    
                Product x  = new Product()
                {
                    
                    ProductId = 3,
                    ProductName="Chocolate",
                    Price=3,
                    CategoryId=2,
                    Category=The_Category_Test_Data
                };

                CartItem The_CartItem = new CartItem()
                {
                    ProductId = 3,
                    Product=x,
                    Price=3,
                    Quantity=2
                };

                //Act
                var CreatedResponse = await _Controller.Add_To_Cart(The_CartItem);
            
                //Assert
                Assert.IsType<OkObjectResult>(CreatedResponse);
            }
        }

        //Test Put() Method
        //When Non Existing CartItem is Passed
        [Fact]
        public async Task Put_NotExistingCartItemPassed_ReturnsNotFoundResponse()
        {
            using (var Context = new ShoppingCartContext(CreateNewContext()))
            {
        
                var _Controller = new CartItemController(Context);//pass context inside controller
            
                //Act
                var BadResponse = await _Controller.Edit_CartItem(null);//non existing CartItem is paased
            
                //Assert
                Assert.IsType<NotFoundResult>(BadResponse);
            }
        }

        //Test Put() Method
        //When Existing CartItem is Passed
        [Fact]
        public async Task Put_ExistingCartItemPassed_ReturnsOkResult()
        {
            using (var Context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                CartItem X=Context.CartItemTestData();
                var _Controller = new CartItemController(Context);//pass context inside controller

                //Act
                var OkResponse = await _Controller.Edit_CartItem(X);//existing CartItem is passed
            
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
                var _Controller = new CartItemController(Context);//pass context inside controller
            
                //Act
                var BadResponse = await _Controller.Remove_A_CartItem_From_The_Cart(null);
            
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
                var _Controller = new CartItemController(Context);//pass context inside controller
            
                //Act
                var BadResponse = await _Controller.Remove_A_CartItem_From_The_Cart(12);
            
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
                Context.CartItemTestData();//Add dummy data
                var _Controller = new CartItemController(Context);//pass context inside controller
        
                //Act
                var OkResponse = await _Controller.Remove_A_CartItem_From_The_Cart(1);

                //Assert
                Assert.IsType<OkResult>(OkResponse);  
            } 
        } 
    } 
}