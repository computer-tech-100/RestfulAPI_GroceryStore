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
using MyApp.Core.Services;

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
            using (var context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                context.CartItemTestData();//We make sure that dummy data has been added
                var controller = new CartItemController(context, new CartItemService(context));//pass context and CartItemService inside controller

                //Act
                var results = controller.GetCartItems();//call Get() function inside CartItem controller

                //Assert
                Assert.NotNull(results);//make sure that Get Method reurns value 
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
                context.CartItemTestData();//Add dummy data
                 var controller = new CartItemController(context, new CartItemService(context));//pass context and CartItemService inside controller

                //Act
                var okResult = controller.GetById(1);//1 is valid Id 
            
                //Assert
                Assert.IsType<OkObjectResult>(okResult.Result);//When Id is valid the result is type of OkObjectResult
            }
        }

        //Test GetById() Method
        //When Invalid Id is passed
        [Fact]
        public void GetById_InvalidIdPassed_ReturnsNotFoundResult() 
        {
            using (var context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                var controller = new CartItemController(context, new CartItemService(context));//pass context and CartItemService inside controller

                //Act
                var not_Found_Result = controller.GetById(-1);// -1 is Invalid Id

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
                // Arrange
                context.CartItemTestData();
                var controller = new CartItemController(context, new CartItemService(context));//pass context and CartItemService inside controller
            
                //Act
                var okResult = controller.GetById(1).Result as OkObjectResult;
            
                //Assert
                Assert.Equal(5, (okResult.Value as CartItem).Quantity);
            }
        }

        //Test Post() Method 
        //When Invalid object is passed 
        [Fact]
        public void CartItemModelValidation_PriceRequired()
        {
            using (var context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                List<ValidationResult> result = new List<ValidationResult>(); 

                //example of Invalid CartItem because Price which is a required field is missed 
                CartItem priceIsMissing = new CartItem()
                {
                    ProductId=1,
                    Quantity=2      
                };
            
                //Act
                bool isValid = Validator.TryValidateObject(priceIsMissing, new ValidationContext(priceIsMissing), result);
               
                //Assert
                Assert.False(isValid);
                Assert.Equal(1, result.Count);//one error 
                Assert.Equal("Price", result[0].MemberNames.ElementAt(0)); 
                Assert.Equal("The Price field is required.", result[0].ErrorMessage);
            }
        }

        //Test Post() Method
        //When valid Object is Passed
        [Fact]
        public async Task Post_ValidObject_ReturnsOkResult()
        {
            using (var context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
               var controller = new CartItemController(context, new CartItemService(context));//pass context and CartItemService inside controller

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
                var createdResponse = await controller.Add_To_Cart(The_CartItem);
            
                //Assert
                Assert.IsType<OkObjectResult>(createdResponse);
            }
        }

        //Test Put() Method
        //When Non Existing CartItem is Passed
        [Fact]
        public async Task Put_NotExistingCartItemPassed_ReturnsNotFoundResponse()
        {
            using (var context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                var controller = new CartItemController(context, new CartItemService(context));//pass context and CartItemService inside controller
        
                //Act
                var badResponse = await controller.Edit_CartItem(null);//non existing CartItem is paased
            
                //Assert
                Assert.IsType<NotFoundResult>(badResponse);
            }
        }

        //Test Put() Method
        //When Existing CartItem is Passed
        [Fact]
        public async Task Put_ExistingCartItemPassed_ReturnsOkResult()
        {
            using (var context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                CartItem cartItem=context.CartItemTestData();
                var controller = new CartItemController(context, new CartItemService(context));//pass context and CartItemService inside controller

                //Act
                var okResponse = await controller.Edit_CartItem(cartItem);//existing CartItem is passed
            
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
                var controller = new CartItemController(context, new CartItemService(context));//pass context and CartItemService inside controller
            
                //Act
                var badResponse = await controller.Remove_A_CartItem_From_The_Cart(null);
            
                //Assert
                Assert.IsType<NotFoundResult>(badResponse);
            }
        }

        //Test Delete() Method
        //When Non Existing Id is Passed
        [Fact]
        public async Task Remove_NotExistingIntIdPassed_ReturnsNotFoundResponse()
        {
            using (var context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                var controller = new CartItemController(context, new CartItemService(context));//pass context and CartItemService inside controller
            
                //Act
                var badResponse = await controller.Remove_A_CartItem_From_The_Cart(12);
            
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
                context.CartItemTestData();//Add dummy data
                var controller = new CartItemController(context, new CartItemService(context));//pass context and CartItemService inside controller
        
                //Act
                var okResponse = await controller.Remove_A_CartItem_From_The_Cart(1);

                //Assert
                Assert.IsType<OkResult>(okResponse);  
            } 
        } 
    } 
}