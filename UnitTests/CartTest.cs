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
        public void GetCart_WhenCalled_ReturnsAllCartItems_And_GrandTotal()
        {
            using (var Context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Context.CartTestData();//We make sure that dummy data has been added
                var Controller = new CartController(Context);//pass context inside controller

                //Act
                var Results = Controller.GetCart();//call Get() function inside Cart controller
                var OkResult = Controller.GetCart();

                //Assert
                Assert.NotNull(Results);//make sure that Get Method returns value 
                Assert.IsType<OkObjectResult>(OkResult.Result);
            } 
        } 
    }
}