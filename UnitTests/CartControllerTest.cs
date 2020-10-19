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
    public class CartControllerTest
    {
        private List<CartItemDTO> MyCartItemList()
        {
            var myCartItemList = new List<CartItemDTO>();
            Category category1 = new Category()
            {
                CategoryId = 1,
                CategoryName ="items"
            };

            Category category2 = new Category()
            {
                CategoryId = 2,
                CategoryName ="Fruits"
            };
            Product product1 = new Product()
            {
                ProductId = 1,
                ProductName = "Milk",
                Price=3,
                CategoryId = 1,
                Category = category1
            };

            Product product2 = new Product()
            {
                ProductId = 2,
                ProductName = "Oranges",
                Price=2,
                CategoryId = 2,
                Category = category2
            };

            myCartItemList.Add(new CartItemDTO{
                ProductId = 1,
                Product = product1,
                Price = 3,
                Quantity = 2
            });

            myCartItemList.Add(new CartItemDTO{
                ProductId = 2,
                Product = product2,
                Price = 2,
                Quantity = 5 
            });

            return myCartItemList;  
        }

        private CartDTO MyCart()
        {
            CartDTO myCart = new CartDTO()
            {
                CartId = 1,
                CartName = "My Cart",
                AllCartItems = MyCartItemList(),
                GrandTotal = 16 // --> (3*2)+(2*5)
            };
            return myCart;
        }

        //Test Get() Method
        [Fact]
        public async Task GetCart_WhenCalled_ReturnsAllCartItems_And_GrandTotal()
        {
            //Arrange
            Mock <ICartService> moqRepo = new Mock <ICartService>();//Mock is type of our Interface

            moqRepo.Setup(repo => repo.GetMyCart()).ReturnsAsync(MyCart());//access the function inside the service class and specify what it returns
            CartController controller = new CartController(moqRepo.Object);//pass moq object inside controller

            //Act
            var results = await controller.GetCart();//call Get() function inside Cart controller
        
            //Assert
            Assert.NotNull(results);//make sure that Get Method returns value 
            
        } 
    }
}