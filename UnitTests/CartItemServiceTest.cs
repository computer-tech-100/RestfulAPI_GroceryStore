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
    public class CartItemServiceTest
    {
        //Create some dummy options that is type of ShoppingCartContext
        public DbContextOptions<ShoppingCartContext> myDummyOptions { get; } = new DbContextOptionsBuilder<ShoppingCartContext>().Options;

        //Test GetCartItems() method
        [Fact]
        public async Task GetCartItems_WhenCalled_ReturnsAllCartItems()
        {

            //Arrange
            var myDbContextMoq = new DbContextMock<ShoppingCartContext>(myDummyOptions);

            //Create list of CartItems
            myDbContextMoq.CreateDbSetMock(x => x.CartItems, new[]
            {
                new CartItem {  ProductId = 1, Price = 3, Quantity = 2 },
                new CartItem {  ProductId = 2, Price = 2, Quantity = 5 },
                new CartItem {  ProductId = 3, Price = 21, Quantity = 1 }
            });

            //Act
            //Pass myDbContextMoq.Object to the CartItemService class
            CartItemService service = new CartItemService(myDbContextMoq.Object);

            //Call GetAllCartItems() function
            var result = await service.GetAllCartItems();

            //Assert
            Assert.NotNull(result);

        }
        
        //Test GetCartItem() method
        [Fact]
        public void GetCartItem_WhenExistingProductIdPassed_ReturnsRightCartItem()
        {
            //Arrange
            var myDbContextMoq = new DbContextMock<ShoppingCartContext>(myDummyOptions);
            myDbContextMoq.CreateDbSetMock(x => x.CartItems, new[]
            {
                new CartItem {  ProductId = 1, Price = 3, Quantity = 2 },
                new CartItem {  ProductId = 2, Price = 2, Quantity = 5 }
            });
           
            CartItemService service = new CartItemService(myDbContextMoq.Object);

            //Act
            var result1 = service.GetCartItem(1);
            var result2 = service.GetCartItem(2);

            //Assert
            Assert.Equal(2, result1.Quantity);
            Assert.Equal(5, result2.Quantity);

        }

        //Test CreateCartItem() method
        [Fact]
        public async Task CreateCartItem_AddsNewProductToCartItemsTable()
        {
            //Arrange
            var myDbContextMoq = new DbContextMock<ShoppingCartContext>(myDummyOptions);

            //Create list of CartItems that contains only two Products
            myDbContextMoq.CreateDbSetMock(x => x.CartItems, new[]
            {
                new CartItem {  ProductId = 1, Price = 3, Quantity = 2 },
                new CartItem {  ProductId = 2, Price = 2, Quantity = 5 }
            });
        
            //We want to add third product to our list of CartItems
            //Since CreateCartItem() method accepts type CartItemDTO we use that type here for our new CartItem
            CartItemDTO testDataDTO = new CartItemDTO()
            {
                ProductId = 3, 
                Price = 21, 
                Quantity = 1
            };

            CartItemService service = new CartItemService(myDbContextMoq.Object);

            //Act
            await service.CreateCartItem(testDataDTO);//call CreateCartItem() function and pass the testDataDTO

            //Assert
            //The size of the CartItems list increases to 3 because CreateCartItem() method added testDataDTO
            Assert.Equal(3, myDbContextMoq.Object.CartItems.Count());

        }
         
        //Test UpdateCartItem() method
        [Fact]
        public async Task UpdateCartItem_EditsTheCartItem_AndAddsTheUpdatedCartItemToCartItemsTable()
        {
            //Arrange
            var myDbContextMoq = new DbContextMock<ShoppingCartContext>(myDummyOptions);
            
            //Create list of CartItems that contains only two Products
            myDbContextMoq.CreateDbSetMock(x => x.CartItems, new[]
            {
                new CartItem {  ProductId = 1, Price = 3, Quantity = 2 },
                new CartItem {  ProductId = 2, Price = 2, Quantity = 5 }
            });

            CartItemDTO testDataDTO = new CartItemDTO()
            {
                ProductId = 1, 
                Price = 3, 
                Quantity = 4 //we updated the Quantity
            };

            CartItemService service = new CartItemService(myDbContextMoq.Object);

            //Act
            //for example we want to update first CartItem
            await service.UpdateCartItem(testDataDTO);
            CartItem cartItmeToBeUpdated = myDbContextMoq.Object.CartItems.FirstOrDefault(x => x.ProductId == 1);

            //Assert
            //Quantity changed from 2 to 4
            Assert.Equal(4, cartItmeToBeUpdated.Quantity);

        }
           
        //Test DeleteCartItem() method
        [Fact]
        public async Task DeleteCartItem_RemovesThatCartItemFromCartItemsTable()
        {
            //Arrange
            var myDbContextMoq = new DbContextMock<ShoppingCartContext>(myDummyOptions);

            //Create list of CartItems that contains only two Products
            myDbContextMoq.CreateDbSetMock(x => x.CartItems, new[]
            {
                new CartItem {  ProductId = 1, Price = 3, Quantity = 2 },
                new CartItem {  ProductId = 2, Price = 2, Quantity = 5 }
            });

            CartItemService service = new CartItemService(myDbContextMoq.Object);

            //Act
            //for example we want to delete first CartItem
            await service.DeleteCartItem(1);//remove first CartItem
             
            //Assert
            //removing the first CartItem causes that our list size becomes 1
            Assert.Equal(1, myDbContextMoq.Object.CartItems.Count());

        }
    }
}