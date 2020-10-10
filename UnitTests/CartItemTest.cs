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
using Moq;
using MyApp.Core.Models.DataTransferObjects;
using MyApp.Core.Models.DbEntities;

//The app should fail gracefully
//Consider all possible aspects that user : test cases with all possible input and output
namespace MyApp.UnitTests{
    public class TestCartItem
    {
        //dummy data
        private List<CartItemDTO> MyCartItemList()
        {
            var myCartItemList = new List<CartItemDTO>();

            Category category = new Category()
            {
                CategoryId = 2,
                CategoryName ="Fruits"
            };

            Product product = new Product()
            {
                ProductId = 1,
                ProductName = "Oranges",
                Price=2,
                CategoryId = 2,
                Category = category
            };

            myCartItemList.Add(new CartItemDTO{
                ProductId = 1,
                Product = product,
                Price = 2,
                Quantity = 5 
            });

            return myCartItemList;  
        }

        //Test Get() Method
        [Fact]
        public async Task Get_WhenCalled_ReturnsAllCartItems()
        {
            //Arrange
            Mock <ICartItemService> moqRepo = new Mock <ICartItemService> ();//Mock is type of our Interface
            moqRepo.Setup(repo => repo.GetAllCartItems()).ReturnsAsync(MyCartItemList());//access the function inside the service class and specify what it returns
            CartItemController controller = new CartItemController(moqRepo.Object);//pass moq object inside controller

            //Act
            ActionResult <IEnumerable<CartItemDTO>> results = await controller.GetCartItems();//call Get() function inside CartItem controller

            //Assert
            Assert.NotNull(results);//make sure that Get Method reurns value 
      
        }
        
        //Test GetById() Method
        //When valid Id is passed
        [Fact]
        public void GetById_ExistingIntIdPassed_ReturnsOkResult()
        {
           
            //Arrange
            Mock <ICartItemService> moqRepo = new Mock <ICartItemService>();//Mock is type of our Interface
            moqRepo.Setup(repo => repo.GetCartItem(1)).Returns(MyCartItemList().FirstOrDefault(i => i.ProductId == 1));//access the function inside the service class and specify what it returns
            CartItemController controller = new CartItemController(moqRepo.Object);//pass moq object inside controller

            //Act
            ActionResult <CartItemDTO> okResult = controller.GetById(1);//1 is valid Id 
        
            //Assert
            Assert.IsType<OkObjectResult>(okResult.Result);//When Id is valid the result is type of OkObjectResult
            
        }

        //Test GetById() Method
        //When Invalid Id is passed
        [Fact]
        public void GetById_InvalidIdPassed_ReturnsNotFoundResult() 
        {
            //Arrange
            Mock <ICartItemService> moqRepo = new Mock <ICartItemService>();//Mock is type of our Interface
            moqRepo.Setup(repo => repo.GetCartItem(-1)).Returns(MyCartItemList().FirstOrDefault(i => i.ProductId == -1));//access the function inside the service class and specify what it returns
            CartItemController controller = new CartItemController(moqRepo.Object);//pass moq object inside controller

            //Act
            ActionResult <CartItemDTO> not_Found_Result = controller.GetById(-1);// -1 is Invalid Id

            //Assert
            Assert.IsType<NotFoundResult>(not_Found_Result.Result);
            
        }

        //Test GetById() Method
        //When GetById() returns the correct item
        [Fact]
        public void GetById_ExistingIntIdPassed_ReturnsRightItem()
        {
            //Arrange
            Mock <ICartItemService> moqRepo = new Mock <ICartItemService>();//Mock is type of our Interface
            moqRepo.Setup(repo => repo.GetCartItem(1)).Returns(MyCartItemList().FirstOrDefault(i => i.ProductId == 1));//access the function inside the service class and specify what it returns
            CartItemController controller = new CartItemController(moqRepo.Object);//pass moq object inside controller
        
            //Act
            OkObjectResult okResult = controller.GetById(1).Result as OkObjectResult;
        
            //Assert
            Assert.Equal(5, (okResult.Value as CartItemDTO).Quantity);
            
        }

        //Test Post() Method
        //When valid Object is Passed
        [Fact]
        public async Task Post_ValidObject_ReturnsOkResult()
        {
           
            //Arrange
            Mock <ICartItemService> moqRepo = new Mock <ICartItemService>();//Mock is type of our Interface

            Category the_Category_Test_Data = new Category()
            {
                CategoryId = 2,
                CategoryName = "Items"
            };
                
            Product product  = new Product()
            {
                
                ProductId = 3,
                ProductName = "Chocolate",
                Price = 3,
                CategoryId = 2,
                Category = the_Category_Test_Data
            };

            CartItemDTO the_CartItem = new CartItemDTO()
            {
                ProductId = 3,
                Product = product,
                Price = 3,
                Quantity = 2
            };

            moqRepo.Setup(repo => repo.CreateCartItem(the_CartItem));//access the function inside the service class and specify what it returns
            CartItemController controller = new CartItemController(moqRepo.Object);//pass context and moq object inside controller

            //Act
            ActionResult createdResponse = await controller.Add_To_Cart(the_CartItem);
        
            //Assert
            Assert.IsType<OkObjectResult>(createdResponse);
        
        }
        

        //Test Put() Method
        //When Non Existing CartItem is Passed
        [Fact]
        public async Task Put_NotExistingCartItemPassed_ReturnsNotFoundResponse()
        {
            
            //Arrange
            Mock <ICartItemService> moqRepo = new Mock <ICartItemService>();//Mock is type of our Interface
            moqRepo.Setup(repo => repo.UpdateCartItem(null));//access the function inside the service class and specify what it returns
            CartItemController controller = new CartItemController(moqRepo.Object);//pass context and moq object inside controller
            
            //Act
            ActionResult badResponse = await controller.Edit_CartItem(null);//non existing CartItem is paased
        
            //Assert
            Assert.IsType<NotFoundResult>(badResponse);
            
        }

        //Test Put() Method
        //When Existing CartItem is Passed
        [Fact]
        public async Task Put_ExistingCartItemPassed_ReturnsOkResult()
        {
            //Arrange
            Mock <ICartItemService> moqRepo = new Mock <ICartItemService>();//Mock is type of our Interface
            Category category = new Category()
            {
                CategoryId = 3,
                CategoryName = "Fruits"
            };

            Product product = new Product()
            {
                ProductId = 1,
                ProductName = "Oranges",
                Price = 2,
                CategoryId = 3,
                Category = category
            };

            CartItemDTO cartItem = new CartItemDTO()
            {
                ProductId = 1,
                Product = product,
                Price = 2,
                Quantity = 5
            };
            moqRepo.Setup(repo => repo.UpdateCartItem(cartItem));//access the function inside the service class and specify what it returns
            CartItemController controller = new CartItemController(moqRepo.Object);//pass moq object inside controller
    
            //Act
            ActionResult okResponse = await controller.Edit_CartItem(cartItem);//existing CartItem is passed
        
            //Assert
            Assert.IsType<OkObjectResult>(okResponse);
            
        }

        //Test Delete() Method
        //When null is Passed
        [Fact]
        public async Task Remove_NullPassed_ReturnsNotFoundResponse()
        {
            
            //Arrange
            Mock <ICartItemService> moqRepo = new Mock <ICartItemService>();//Mock is type of our Interface
            moqRepo.Setup(repo => repo.DeleteCartItem(null));//access the function inside the service class and specify what it returns
            CartItemController controller = new CartItemController(moqRepo.Object);//pass moq object inside controller
        
            //Act
            ActionResult badResponse = await controller.Remove_A_CartItem_From_The_Cart(null);
        
            //Assert
            Assert.IsType<NotFoundResult>(badResponse);
            
        }

        //Test Delete() Method
        //When Existing Id is Passed
        [Fact]
        public async Task Remove_ExistingIntIdPassed_ReturnsOkResult()
        {
            
            //Arrange
            Mock <ICartItemService> moqRepo = new Mock <ICartItemService>();//Mock is type of our Interface
            moqRepo.Setup(repo => repo.DeleteCartItem(1));//access the function inside the service class and specify what it returns
            CartItemController controller = new CartItemController(moqRepo.Object);//pass context and moq object inside controller
    
            //Act
            ActionResult okResponse = await controller.Remove_A_CartItem_From_The_Cart(1);

            //Assert
            Assert.IsType<OkResult>(okResponse);  
            
        } 
    } 
}