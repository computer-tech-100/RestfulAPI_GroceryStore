using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyApp.Core.Contexts;
using MyApp.Core.Models.DataTransferObjects;
using MyApp.Core.Models.DbEntities;
using MyApp.Core.Services;
using EntityFrameworkCore3Mock;//DbContextMock
using Xunit;
using System.Threading.Tasks;

namespace MyApp.UnitTests
{
    public class CartServiceTest
    {
        //Create some dummy options that is type of ShoppingCartContext
        public DbContextOptions<ShoppingCartContext> myDummyOptions { get; } = new DbContextOptionsBuilder<ShoppingCartContext>().Options;

        //Test GetMyCart() method
        [Fact]
        public void GetMyCart_WhenCalled_ReturnsAllCartItems()
        {
            
            //Arrange
            var myDbContextMoq = new DbContextMock<ShoppingCartContext>(myDummyOptions);

             CartItem my_CartItem1 = new CartItem()
            {
                ProductId = 1,
                Price = 3,
                Quantity = 2   
            };

            CartItem my_CartItem2 = new CartItem()
            {
                ProductId = 2,
                Price = 2,
                Quantity = 5  
            };

            List<CartItem> All_Cart_Items = new List<CartItem>();
            All_Cart_Items.Add(my_CartItem1);
            All_Cart_Items.Add(my_CartItem2);
            
            myDbContextMoq.CreateDbSetMock(x => x.Carts, new[]
            {
                new Cart { CartId = 1, CartName = "My Cart",  AllCartItems = All_Cart_Items, GrandTotal = 16 }
            });

            //Act
            //Pass myDbContextMoq.Object to the CartService class
            CartService service = new CartService(myDbContextMoq.Object);

            //Call GetMyCart() function
            var result = service.GetMyCart();

            //Assert
            Assert.NotNull(result);

        }
    }
}