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
namespace MyApp.UnitTests
{

    public class TestProduct
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
        public void Get_WhenCalled_ReturnsAllProducts()
        {
            using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Mock <IProductService> moqRepo = new Mock <IProductService>();//Mock is type of our Interface
                context.ProductTestData();//We make sure that dummy data has been added
                moqRepo.Setup(repo => repo.GetProducts()).Returns(context.Products);//access the function inside the service class and specify what it returns
                ProductController controller = new ProductController(context, moqRepo.Object);//pass context and moq object inside controller

                //Act
                ActionResult <IEnumerable<Product>> results = controller.Get();//call Get() function inside Procuct controller

                //Assert
                Assert.NotNull(results);//make sure that Get Method returns value 
            }
        }

        //Test GetById() Method
        //When valid Id is passed
        [Fact]
        public void GetById_ExistingIntIdPassed_ReturnsOkResult()
        {
            using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Mock <IProductService> moqRepo = new Mock <IProductService>();//Mock is type of our Interface
                context.ProductTestData();//We make sure that dummy data has been added
                moqRepo.Setup(repo => repo.GetProduct(1)).Returns(context.Products.FirstOrDefault(i => i.ProductId == 1));//access the function inside the service class and specify what it returns
                ProductController controller = new ProductController(context, moqRepo.Object);//pass context and moq object inside controller
              
                //Act
                ActionResult <Product> okResult = controller.GetById(1);//1 is valid Id 
            
                //Assert
                Assert.IsType<OkObjectResult>(okResult.Result);//When Id is valid the result is type of OkObjectResult
            }
        }

        //Test GetById() Method
        //When Invalid Id is passed
        [Fact]
        public void GetById_InvalidIdPassed_ReturnsNotFoundResult() 
        {
            using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Mock <IProductService> moqRepo = new Mock <IProductService>();//Mock is type of our Interface
                context.ProductTestData();//We make sure that dummy data has been added
                moqRepo.Setup(repo => repo.GetProduct(-1)).Returns(context.Products.FirstOrDefault(i => i.ProductId == -1));//access the function inside the service class and specify what it returns
                ProductController controller = new ProductController(context, moqRepo.Object);//pass context and moq object inside controller

                //Act
                ActionResult <Product> not_Found_Result = controller.GetById(-1);// -1 is Invalid Id

                //Assert
                Assert.IsType<NotFoundResult>(not_Found_Result.Result);
            }
        }

        //Test GetById() Method
        //When GetById() returns the correct item
        [Fact]
        public void GetById_ExistingIntIdPassed_ReturnsRightItem()
        {
            using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Mock <IProductService> moqRepo = new Mock <IProductService> ();//Mock is type of our Interface
                context.ProductTestData();//We make sure that dummy data has been added
                moqRepo.Setup(repo => repo.GetProduct(1)).Returns(context.Products.FirstOrDefault(i => i.ProductId == 1));//access the function inside the service class and specify what it returns
                ProductController controller = new ProductController(context, moqRepo.Object);//pass context and moq object inside controller

                //Act
                OkObjectResult okResult = controller.GetById(1).Result as OkObjectResult;
            
                //Assert
                Assert.Equal("Oranges", (okResult.Value as Product).ProductName);  
            }
        }

        //Test Post() Method 
        //When Invalid object is passed 
        [Fact]
        public async Task ProductModelValidation_ProductNameRequired()
        {
            using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Mock <IProductService> moqRepo = new Mock <IProductService>();//Mock is type of our Interface

                //This Product does not contain ProductName hence the Product is invalid
                Product productNameIsMissing = new Product()
                {
                    ProductId = 2,
                    Price = 23,
                    CategoryId = 1
                };

                moqRepo.Setup(Repo => Repo.CreateProduct(productNameIsMissing));
                ProductController controller = new ProductController(context, moqRepo.Object);//pass context and moq object inside controller
                controller.ModelState.AddModelError("ProductName","Required");

                //Act
                ActionResult result = await controller.Post(productNameIsMissing);

                //Assert
                BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.IsType<SerializableError>(badRequestResult.Value);
            }
        }

        //Test Post() Method 
        //When valid object is passed
        [Fact]
        public async Task Post_ValidObject_ReturnsOkResult()
        {
            using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Mock <IProductService> moqRepo = new Mock <IProductService>();//Mock is type of our Interface

                 Category categoryTestData = new Category()
                {
                    CategoryId = 1, 
                    CategoryName = "Clothes"
                
                };
                Product product = new Product()
                {
                    ProductId = 2,
                    ProductName = "TShirts",
                    Price = 23,
                    CategoryId = 1,
                    Category = categoryTestData
                };

                moqRepo.Setup(repo => repo.CreateProduct(product));//access the function inside the service class and specify what it returns
                ProductController controller = new ProductController(context, moqRepo.Object);//pass context and moq object inside controller
            
                //Act
                ActionResult createdResponse = await controller.Post(product);
            
                //Assert
                Assert.IsType<OkObjectResult>(createdResponse);
            }
        }

        //Test Put() Method
        //When Non Existing Product is Passed
        [Fact]
        public async Task Put_NotExistingProductPassed_ReturnsNotFoundResponse()
        {
            using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Mock <IProductService> moqRepo = new Mock <IProductService>();//Mock is type of our Interface
                moqRepo.Setup(repo => repo.UpdateProduct(null));//access the function inside the service class and specify what it returns
                ProductController controller = new ProductController(context, moqRepo.Object);//pass context and moq object inside controller
            
                //Act
                ActionResult badResponse = await controller.Put(null);//non existing Product is paased
            
                //Assert
                Assert.IsType<NotFoundResult>(badResponse);
            }
        }

        //Test Put() Method
        //When Existing Product is Passed
        [Fact]
        public async Task Put_ExistingProductPassed_ReturnsOkResult()
        {
            using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Mock <IProductService> moqRepo = new Mock <IProductService>();//Mock is type of our Interface
                Product product = context.ProductTestData();
                moqRepo.Setup(repo => repo.UpdateProduct(product));//access the function inside the service class and specify what it returns
                ProductController controller = new ProductController(context, moqRepo.Object);//pass context and moq object inside controller
               
                //Act
                ActionResult okResponse = await controller.Put(product);//existing Product is passed

                //Assert
                Assert.IsType<OkObjectResult>(okResponse);
            }
        }

        //Test Delete() Method
        //When null is Passed
        [Fact]
        public async Task Remove_NullPassed_ReturnsNotFoundResponse()
        {
            using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Mock <IProductService> moqRepo = new Mock <IProductService>();//Mock is type of our Interface
                moqRepo.Setup(repo => repo.DeleteProduct(null));//access the function inside the service class and specify what it returns
                ProductController controller = new ProductController(context, moqRepo.Object);//pass context and moq object inside controller
        
                //Act
                ActionResult badResponse = await controller.Delete(null);//When null is passed
        
                //Assert
                Assert.IsType<NotFoundResult>(badResponse);
            }
        }

        //Test Delete() Method
        //When Non Existing Id is Passed
        [Fact]
        public async Task Remove_NotExistingIntIdPassed_ReturnsNotFoundResponse()
        {
            using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Mock <IProductService> moqRepo = new Mock <IProductService>();//Mock is type of our Interface
                context.ProductTestData();//We make sure that dummy data has been added
                moqRepo.Setup(repo => repo.DeleteProduct(10));//access the function inside the service class and specify what it returns
                ProductController controller = new ProductController(context, moqRepo.Object);//pass context and moq object inside controller
            
                //Act
                ActionResult badResponse = await controller.Delete(10);//When Non existing Id is passed
            
                //Assert
                Assert.IsType<NotFoundResult>(badResponse);
            }
        }

        //Test Delete() Method
        //When Existing Id is Passed
        [Fact]
        public async Task Remove_ExistingIntIdPassed_ReturnsOkResult()
        {
            using (ShoppingCartContext context = new ShoppingCartContext(CreateNewContext()))
            {
                //Arrange
                Mock <IProductService> moqRepo = new Mock <IProductService>();//Mock is type of our Interface
                context.ProductTestData();//We make sure that dummy data has been added
                moqRepo.Setup(repo => repo.DeleteProduct(1));//access the function inside the service class and specify what it returns
                ProductController controller = new ProductController(context, moqRepo.Object);//pass context and moq object inside controller
        
                //Act
                ActionResult okResponse = await controller.Delete(1);
        
                //Assert
                Assert.IsType<OkResult>(okResponse);
            }
        }
    }
}