using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyApp.Core.Contexts;
using MyApp.Core.Models.DataTransferObjects;
using MyApp.Core.Models.DbEntities;
using MyApp.Core.Services;

using Xunit;

namespace MyApp.UnitTests
{

    public class CategoryServiceTest
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
        private List<Category> MyCategoryList()
        {
            var categoryList = new List<Category>();

            categoryList.Add(new Category()
            {
                CategoryId =1,
                CategoryName = "Items"
            });
            categoryList.Add(new Category()
            {
                CategoryId =2,
                CategoryName = "Fruits"
            });

            return categoryList;  
        }

        //Test Get() Method
        [Fact]
        public void Get_WhenCalled_ReturnsAllCategories()
        {
         
            //using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            //{
            //Arrange
            Mock <ShoppingCartContext> moqContext = new Mock <ShoppingCartContext>();//Mock is type of our Interface

            var moqSet = new Mock<DbSet<Category>>();
             
            moqContext.Setup(m => m.Categories).Returns(moqSet.Object);
   
            CategoryService service = new CategoryService(moqContext.Object);//pass moq object inside CategoryService

            //Act
            var result = service.GetCategories();

            //Assert
            Assert.NotNull(result);
           //}

        }
    }
}