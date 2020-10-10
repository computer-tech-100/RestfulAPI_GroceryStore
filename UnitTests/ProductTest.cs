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
namespace MyApp.UnitTests
{

    public class TestProduct
    {
        //dummy data
        private List<ProductDTO> MyProductList()
        {
            var productList = new List<ProductDTO>();
            Category the_Category = new Category()
            {
                CategoryId = 2,
                CategoryName ="Fruits"
            };

            productList.Add(new ProductDTO()
            {
                ProductId = 1,
                ProductName = "Oranges",
                Price=2,
                CategoryId = 2,
                Category = the_Category
            });
           
            return productList;  
        }

        //Test Get() Method
        [Fact]
        public async Task Get_WhenCalled_ReturnsAllProducts()
        {
            
            //Arrange
            Mock <IProductService> moqRepo = new Mock <IProductService>();//Mock is type of our Interface
            //context.ProductTestData();//We make sure that dummy data has been added
            moqRepo.Setup(repo => repo.GetProducts()).ReturnsAsync(MyProductList());//access the function inside the service class and specify what it returns
            ProductController controller = new ProductController(moqRepo.Object);//pass moq object inside controller

            //Act
            ActionResult <List<ProductDTO>> results = await controller.Get();//call Get() function inside Procuct controller

            //Assert
            Assert.NotNull(results);//make sure that Get Method returns value 
            
        }

        //Test GetById() Method
        //When valid Id is passed
        [Fact]
        public void GetById_ExistingIntIdPassed_ReturnsOkResult()
        {
            
            //Arrange
            Mock <IProductService> moqRepo = new Mock <IProductService>();//Mock is type of our Interface
            moqRepo.Setup(repo => repo.GetProduct(1)).Returns(MyProductList().FirstOrDefault(i => i.ProductId == 1));//access the function inside the service class and specify what it returns
            ProductController controller = new ProductController(moqRepo.Object);//pass moq object inside controller
            
            //Act
            ActionResult <ProductDTO> okResult = controller.GetById(1);//1 is valid Id 
        
            //Assert
            Assert.IsType<OkObjectResult>(okResult.Result);//When Id is valid the result is type of OkObjectResult
            
        }

        //Test GetById() Method
        //When Invalid Id is passed
        [Fact]
        public void GetById_InvalidIdPassed_ReturnsNotFoundResult() 
        {
            //Arrange
            Mock <IProductService> moqRepo = new Mock <IProductService>();//Mock is type of our Interface
            moqRepo.Setup(repo => repo.GetProduct(-1)).Returns(MyProductList().FirstOrDefault(i => i.CategoryId == -1));//access the function inside the service class and specify what it returns
            ProductController controller = new ProductController(moqRepo.Object);//pass moq object inside controller

            //Act
            ActionResult <ProductDTO> not_Found_Result = controller.GetById(-1);// -1 is Invalid Id

            //Assert
            Assert.IsType<NotFoundResult>(not_Found_Result.Result);
            
        }

        //Test GetById() Method
        //When GetById() returns the correct item
        [Fact]
        public void GetById_ExistingIntIdPassed_ReturnsRightItem()
        {
            //Arrange
            Mock <IProductService> moqRepo = new Mock <IProductService> ();//Mock is type of our Interface
            
            moqRepo.Setup(repo => repo.GetProduct(1)).Returns(MyProductList().FirstOrDefault(i => i.ProductId == 1));//access the function inside the service class and specify what it returns
            ProductController controller = new ProductController(moqRepo.Object);//pass moq object inside controller

            //Act
            OkObjectResult okResult = controller.GetById(1).Result as OkObjectResult;
        
            //Assert
            Assert.Equal("Oranges", (okResult.Value as ProductDTO).ProductName);  
            
        }

        //Test Post() Method 
        //When Invalid object is passed 
        [Fact]
        public async Task ProductModelValidation_ProductNameRequired()
        {
            //Arrange
            Mock <IProductService> moqRepo = new Mock <IProductService>();//Mock is type of our Interface

            //This Product does not contain ProductName hence the Product is invalid
            ProductDTO productNameIsMissing = new ProductDTO();
           
            moqRepo.Setup(Repo => Repo.CreateProduct(productNameIsMissing));
            ProductController controller = new ProductController(moqRepo.Object);//pass moq object inside controller
            controller.ModelState.AddModelError("ProductName","Required");

            //Act
            ActionResult result = await controller.Post(productNameIsMissing);

            //Assert
            BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
            
        }

        //Test Post() Method 
        //When valid object is passed
        [Fact]
        public async Task Post_ValidObject_ReturnsOkResult()
        {
            
            //Arrange
            Mock <IProductService> moqRepo = new Mock <IProductService>();//Mock is type of our Interface

            Category categoryTestData = new Category()
            {
                CategoryId = 1, 
                CategoryName = "Clothes"
            
            };
            ProductDTO product = new ProductDTO()
            {
                //ProductId should be auto generated because of Identity column
                ProductName = "TShirts",
                Price = 23,
                CategoryId = 1,
                Category = categoryTestData
            };

            moqRepo.Setup(repo => repo.CreateProduct(product));//access the function inside the service class and specify what it returns
            ProductController controller = new ProductController(moqRepo.Object);//pass moq object inside controller
        
            //Act
            ActionResult createdResponse = await controller.Post(product);
        
            //Assert
            var okResult = Assert.IsType<OkObjectResult>(createdResponse);
            Assert.True((okResult.Value as ProductDTO).ProductId > 0);
            
        }
        
        //Test Put() Method
        //When Non Existing Product is Passed
        [Fact]
        public async Task Put_NotExistingProductPassed_ReturnsNotFoundResponse()
        {
            
            //Arrange
            Mock <IProductService> moqRepo = new Mock <IProductService>();//Mock is type of our Interface
            moqRepo.Setup(repo => repo.UpdateProduct(null));//access the function inside the service class and specify what it returns
            ProductController controller = new ProductController(moqRepo.Object);//pass moq object inside controller
        
            //Act
            ActionResult badResponse = await controller.Put(null);//non existing Product is paased
        
            //Assert
            Assert.IsType<NotFoundResult>(badResponse);
            
        }

        //Test Put() Method
        //When Existing Product is Passed
        [Fact]
        public async Task Put_ExistingProductPassed_ReturnsOkResult()
        {
            //Arrange
            Mock <IProductService> moqRepo = new Mock <IProductService>();//Mock is type of our Interface
            Category the_Category = new Category()
            {
                CategoryId = 2,
                CategoryName ="Fruits"
            };
            ProductDTO product = new ProductDTO()
            {
                ProductId = 1,
                ProductName = "Oranges",
                Price=2,
                CategoryId = 2,
                Category = the_Category 
            };
            moqRepo.Setup(repo => repo.UpdateProduct(product));//access the function inside the service class and specify what it returns
            ProductController controller = new ProductController(moqRepo.Object);//pass moq object inside controller
            
            //Act
            ActionResult okResponse = await controller.Put(product);//existing Product is passed

            //Assert
            Assert.IsType<OkObjectResult>(okResponse);
            
        }

        //Test Delete() Method
        //When null is Passed
        [Fact]
        public async Task Remove_NullPassed_ReturnsNotFoundResponse()
        {
            //Arrange
            Mock <IProductService> moqRepo = new Mock <IProductService>();//Mock is type of our Interface
            moqRepo.Setup(repo => repo.DeleteProduct(null));//access the function inside the service class and specify what it returns
            ProductController controller = new ProductController(moqRepo.Object);//pass moq object inside controller
    
            //Act
            ActionResult badResponse = await controller.Delete(null);//When null is passed
    
            //Assert
            Assert.IsType<NotFoundResult>(badResponse);
            
        }

        //Test Delete() Method
        //When Existing Id is Passed
        [Fact]
        public async Task Remove_ExistingIntIdPassed_ReturnsOkResult()
        {
            //Arrange
            Mock <IProductService> moqRepo = new Mock <IProductService>();//Mock is type of our Interface
        
            moqRepo.Setup(repo => repo.DeleteProduct(1));//access the function inside the service class and specify what it returns
            ProductController controller = new ProductController(moqRepo.Object);//pass moq object inside controller
    
            //Act
            ActionResult okResponse = await controller.Delete(1);
    
            //Assert
            Assert.IsType<OkResult>(okResponse);
            
        }
    }
}