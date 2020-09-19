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

//The app should fail gracefully
//Consider all possible aspects that user : test cases with all possible input and output
namespace MyApp.UnitTests{
    public class TestCart
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
        public void GetCart_WhenCalled_ReturnsAllCartItems_And_GrandTotal()
        {
            using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Mock <ICartService> moqRepo = new Mock <ICartService>();//Mock is type of our Interface
                Cart cart =context.CartTestData();//We make sure that dummy data has been added
                moqRepo.Setup(repo => repo.GetMyCart()).Returns(cart);//access the function inside the service class and specify what it returns
                CartController controller = new CartController(context, moqRepo.Object);//pass context and moq object inside controller

                //Act
                var results = controller.GetCart();//call Get() function inside Cart controller
           
                //Assert
                Assert.NotNull(results);//make sure that Get Method returns value 
            } 
        } 
    }
}