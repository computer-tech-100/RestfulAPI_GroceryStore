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
    public class CategoryServiceTest
    {
        //Create some dummy options that is type of ShoppingCartContext
        public DbContextOptions<ShoppingCartContext> myDummyOptions { get; } = new DbContextOptionsBuilder<ShoppingCartContext>().Options;

        //Test GetCategories() method
        [Fact]
        public async Task GetCategories_WhenCalled_ReturnsAllCategories()
        {
            //Arrange
            var myDbContextMoq = new DbContextMock<ShoppingCartContext>(myDummyOptions);

            //Create list of Categories
            myDbContextMoq.CreateDbSetMock(x => x.Categories, new[]
            {
                new Category { CategoryId = 1, CategoryName = "Items" },
                new Category { CategoryId = 2, CategoryName = "Fruits" },
                new Category { CategoryId = 3, CategoryName = "Tshirts" }
            });

            //Act
            //Pass myDbContextMoq.Object to the CategoryService class
            CategoryService service = new CategoryService(myDbContextMoq.Object);

            //Call GetCategories() function
            var result = await service.GetCategories();

            //Assert
            Assert.NotNull(result);

        }

        //Test GetCategory() method
        [Fact]
        public void GetCategory_WhenExistingCategoryIdPassed_ReturnsRightItem()
        {
            //Arrange
            var myDbContextMoq = new DbContextMock<ShoppingCartContext>(myDummyOptions);
            myDbContextMoq.CreateDbSetMock(x => x.Categories, new[]
            {
                new Category { CategoryId = 1, CategoryName = "Items" },
                new Category { CategoryId = 2, CategoryName = "Fruits" }
            });

            CategoryService service = new CategoryService(myDbContextMoq.Object);

            //Act
            var result1 = service.GetCategory(1);
            var result2 = service.GetCategory(2);

            //Assert
            Assert.Equal("Items",result1.CategoryName);
            Assert.Equal("Fruits",result2.CategoryName);

        }

        //Test CreateCategory() method
        [Fact]
        public async Task CreateCategory_AddsNewCategoryToCategoriesTable()
        {
            //Arrange
            var myDbContextMoq = new DbContextMock<ShoppingCartContext>(myDummyOptions);

            //Create list of Categories that contains only two Categories : "Items" and "Fruits"
            myDbContextMoq.CreateDbSetMock(x => x.Categories, new[]
            {
                new Category { CategoryId = 1, CategoryName = "Items" },
                new Category { CategoryId = 2, CategoryName = "Fruits" }
            });
        
            //We want to add Hardwares to our list of Categories
            //Since CreateCategory() method accepts type CategoryDTO we use that type here for our new Category
            CategoryDTO testDataDTO = new CategoryDTO()
            {
                CategoryId = 3,
                CategoryName = "HardWares" 
            };

            CategoryService service = new CategoryService(myDbContextMoq.Object);

            //Act
            await service.CreateCategory(testDataDTO);//call CreateCategory() function and pass the testDataDTO

            //Assert
            //The size of the Categories list increases to 3 because CreateCategory() method added testDataDTO
            Assert.Equal(3,myDbContextMoq.Object.Categories.Count());

        }
         
        //Test UpdateCategory() method
        [Fact]
        public async Task UpdateCategory_EditsTheCategory_AndAddsTheUpdatedCategoryToCategoriesTable()
        {
            //Arrange
            var myDbContextMoq = new DbContextMock<ShoppingCartContext>(myDummyOptions);

            //create list of Categories that contains two Categories: "Items" and "Fruits"
            myDbContextMoq.CreateDbSetMock(x => x.Categories, new[]
            {
                new Category { CategoryId = 1, CategoryName = "Items" },
                new Category { CategoryId = 2, CategoryName = "Fruits" }
            });

        
            CategoryDTO testDataDTO = new CategoryDTO()
            {
                CategoryId = 1,
                CategoryName = "Modified Name" 
            };

            CategoryService service = new CategoryService(myDbContextMoq.Object);

            //Act
            //for example we want to update "Items" Category
            await service.UpdateCategory(testDataDTO);
            Category categoryToBeUpdated = myDbContextMoq.Object.Categories.FirstOrDefault(x => x.CategoryId == 1);

            //Assert
            //CategoryName changed from "Items" to "Modified Name"
            Assert.Equal("Modified Name",categoryToBeUpdated.CategoryName);

        }
           
        //Test DeleteCategory() method
        [Fact]
        public async Task DeleteCategory_RemovesThatCategoryFromCategoriesTable()
        {
            //Arrange
            var myDbContextMoq = new DbContextMock<ShoppingCartContext>(myDummyOptions);

            //create list of Categories that contains two Categories: Items and Fruits
            myDbContextMoq.CreateDbSetMock(x => x.Categories, new[]
            {
                new Category { CategoryId = 1, CategoryName = "Items" },
                new Category { CategoryId = 2, CategoryName = "Fruits" }
            });

            CategoryService service = new CategoryService(myDbContextMoq.Object);

            //Act
            //for example we want to delete "Items" Category
            await service.DeleteCategory(1);//remove "Items" Category
             
            //Assert
            //removing "items" Category causes that our list size becomes 1
            Assert.Equal(1,myDbContextMoq.Object.Categories.Count());

        }
    }
}