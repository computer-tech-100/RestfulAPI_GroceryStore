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
    public class ProductServiceTest
    {
        //Create some dummy options that is type of ShoppingCartContext
        public DbContextOptions<ShoppingCartContext> myDummyOptions { get; } = new DbContextOptionsBuilder<ShoppingCartContext>().Options;

        //Test GetProducts() method
        [Fact]
        public async Task GetProducts_WhenCalled_ReturnsAllProducts()
        {
            //Arrange
            var myDbContextMoq = new DbContextMock<ShoppingCartContext>(myDummyOptions);

            //Create list of Products
            myDbContextMoq.CreateDbSetMock(x => x.Products, new[]
            {
                new Product { ProductId = 1, ProductName = "Milk", Price = 3, CategoryId = 1 },
                new Product { ProductId = 2, ProductName = "Oranges", Price = 2, CategoryId = 2 },
                new Product { ProductId = 3, ProductName = "White T-Shirts", Price = 21, CategoryId = 3 },
            });

            //Act
            //Pass myDbContextMoq.Object to the ProductService class
            ProductService service = new ProductService(myDbContextMoq.Object);

            //Call GetProducts() function
            var result = await service.GetProducts();

            //Assert
            Assert.NotNull(result);

        }

        //Test GetProduct() method
        [Fact]
        public void GetProduct_WhenExistingProductIdPassed_ReturnsRightItem()
        {
            //Arrange
            var myDbContextMoq = new DbContextMock<ShoppingCartContext>(myDummyOptions);
            myDbContextMoq.CreateDbSetMock(x => x.Products, new[]
            {
                new Product { ProductId = 1, ProductName = "Milk", Price = 3, CategoryId = 1 },
                new Product { ProductId = 2, ProductName = "Oranges", Price = 2, CategoryId = 2 },
            });

            ProductService service = new ProductService(myDbContextMoq.Object);

            //Act
            var result1 = service.GetProduct(1);
            var result2 = service.GetProduct(2);

            //Assert
            Assert.Equal("Milk",result1.ProductName);
            Assert.Equal("Oranges",result2.ProductName);

        }

        //Test CreateProduct() method
        [Fact]
        public async Task CreateProduct_AddsNewProductToProductsTable()
        {
            //Arrange
            var myDbContextMoq = new DbContextMock<ShoppingCartContext>(myDummyOptions);

            //Create list of Products that contains only two Products : "Milk" and "Oranges"
            myDbContextMoq.CreateDbSetMock(x => x.Products, new[]
            {
                new Product { ProductId = 1, ProductName = "Milk", Price = 3, CategoryId = 1 },
                new Product { ProductId = 2, ProductName = "Oranges", Price = 2, CategoryId = 2 },
            });
        
            //We want to add the White T-Shirts to our list of Products
            //Since CreateProduct() method accepts type ProductDTO we use that type here for our new Product
            ProductDTO testDataDTO = new ProductDTO()
            {
                ProductId = 3,
                ProductName = "T-Shirts",
                Price = 21,
                CategoryId = 3  
            };

            ProductService service = new ProductService(myDbContextMoq.Object);

            //Act
            await service.CreateProduct(testDataDTO);//call CreateProduct() function and pass the testDataDTO

            //Assert
            //The size of the Products list increases to 3 because CreateProduct() method added testDataDTO
            Assert.Equal(3,myDbContextMoq.Object.Products.Count());

        }
         
        //Test UpdateProduct() method
        [Fact]
        public async Task UpdateProduct_EditsTheProduct_AndAddsTheUpdatedProductToProductsTable()
        {
            //Arrange
            var myDbContextMoq = new DbContextMock<ShoppingCartContext>(myDummyOptions);

            //Create list of Products that contains only two Products : "Milk" and "Oranges"
            myDbContextMoq.CreateDbSetMock(x => x.Products, new[]
            {
                new Product { ProductId = 1, ProductName = "Milk", Price = 3, CategoryId = 1 },
                new Product { ProductId = 2, ProductName = "Oranges", Price = 2, CategoryId = 2 },
            });

        
            ProductDTO testDataDTO = new ProductDTO()
            {
                ProductId = 1,
                ProductName = "Modified Name",
                Price = 3,
                CategoryId = 1
            };

            ProductService service = new ProductService(myDbContextMoq.Object);

            //Act
            //for example we want to update the Product "Milk"
            await service.UpdateProduct(testDataDTO);
            Product productToBeUpdated = myDbContextMoq.Object.Products.FirstOrDefault(x => x.ProductId == 1);

            //Assert
            //ProductName changed from "Milk" to "Modified Name"
            Assert.Equal("Modified Name",productToBeUpdated.ProductName);

        }
           
        //Test DeleteProduct() method
        [Fact]
        public async Task DeleteProduct_RemovesThatProductFromProductsTable()
        {
            //Arrange
            var myDbContextMoq = new DbContextMock<ShoppingCartContext>(myDummyOptions);

            //Create list of Products that contains only two Products : "Milk" and "Oranges"
            myDbContextMoq.CreateDbSetMock(x => x.Products, new[]
            {
                new Product { ProductId = 1, ProductName = "Milk", Price = 3, CategoryId = 1 },
                new Product { ProductId = 2, ProductName = "Oranges", Price = 2, CategoryId = 2 },
            });

            ProductService service = new ProductService(myDbContextMoq.Object);

            //Act
            //for example we want to delete Product "Milk"
            await service.DeleteProduct(1);//remove Product "Milk"
             
            //Assert
            //removing Product "Milk" causes that our list size becomes 1
            Assert.Equal(1,myDbContextMoq.Object.Products.Count());

        }   
    }
}