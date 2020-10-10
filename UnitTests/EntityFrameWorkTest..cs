using MyApp.Core.Models.DbEntities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using MyApp.Core.Contexts;
using MyApp.Core.Models.DataTransferObjects;
using Moq;
using System.Linq;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Core.Services;
using System;
using System.Threading.Tasks;

namespace MyApp.UnitTests
{
    public class MoqEntityFramework
    {
        [Fact]
        public void MoqDbSet()
        {
            //Arrange
            //list of queryable objects
            List<Category> categories = new List<Category>() 
            {
            new Category() { CategoryId = 1, CategoryName = "Items" },
            new Category() { CategoryId = 2, CategoryName = "Fruits" }
            };

            Category category = new Category()
            {
                CategoryId = 3,
                CategoryName ="Clothes"
            };

            var queryable = categories.AsQueryable();

            var moqSet = new Mock<DbSet<Category>>();

            //Setup the mock
            //Mock IQueryable
            //Setup return values for Provider, Expression, ElementType, GetEnumerator methods to mock an IQueryable
            //When these methods are executed then the values that we setup in the Returns expression will be returned
            moqSet.As<IQueryable<Category>>().Setup(i => i.Provider).Returns(queryable.Provider);
            moqSet.As<IQueryable<Category>>().Setup(i => i.Expression).Returns(queryable.Expression);
            moqSet.As<IQueryable<Category>>().Setup(i => i.ElementType).Returns(queryable.ElementType);
            moqSet.As<IQueryable<Category>>().Setup(i=> i.GetEnumerator()).Returns(queryable.GetEnumerator);

            //Mock Add and Remove
            //Callback method is used (instead of Returns) because Add and Remove functions do not return anything
            moqSet.Setup(i => i.Add(It.IsAny<Category>())).Callback((Category myCategory) => categories.Add(myCategory));
            moqSet.Setup(i => i.Remove(It.IsAny<Category>())).Callback((Category myCategory) => categories.Remove(myCategory));

            //setup mocked version 
            var moqContext = new Mock<ShoppingCartContext>();
            moqContext.Setup(m => m.Categories).Returns(moqSet.Object);
            
        }
    }
}
