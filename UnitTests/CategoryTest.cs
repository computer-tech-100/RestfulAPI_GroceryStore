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
using System.Data.Common;
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
    public class TestCategory
    {
        //dummy data
        private List<CategoryDTO> MyCategoryList()
        {
            var categoryList = new List<CategoryDTO>();

            categoryList.Add(new CategoryDTO()
            {
                CategoryId =1,
                CategoryName = "Items"
            });
            categoryList.Add(new CategoryDTO()
            {
                CategoryId =2,
                CategoryName = "Fruits"
            });

            return categoryList;  
        }
        

        //Test Get() Method
        [Fact]
        public async Task Get_WhenCalled_ReturnsAllCategories()
        {
            
            //Arrange
            Mock <ICategoryService> moqRepo = new Mock <ICategoryService>();//Mock is type of our Interface
            moqRepo.Setup(repo => repo.GetCategories()).ReturnsAsync(MyCategoryList());//access the function inside the service class and specify what it returns
            CategoryController controller = new CategoryController(moqRepo.Object);//pass moq object inside controller

            //Act
            var results = await controller.Get();//call Get() function inside Category controller

            //Assert
            Assert.NotNull(results);//make sure that Get Method returns value
            
        }

        //Test GetById() Method
        //When valid Id is passed
        [Fact]
        public void GetById_ExistingIntIdPassed_ReturnsOkResult()
        {
    
            //Arrange
            Mock <ICategoryService> moqRepo = new Mock <ICategoryService>(); //Mock is type of our Interface
            moqRepo.Setup(repo => repo.GetCategory(1)).Returns(MyCategoryList().FirstOrDefault(i => i.CategoryId == 1));//access the function inside the service class and specify what it returns
            CategoryController controller = new CategoryController(moqRepo.Object);//pass moq object inside controller
        
            //Act
            ActionResult <CategoryDTO> OkResult = controller.GetById(1);//1 is valid Id 
        
            //Assert
            Assert.IsType<OkObjectResult>(OkResult.Result);//When Id is valid the result is type of OkObjectResult
         
        }

        //Test GetById() Method
        //When Invalid Id is passed
        [Fact]
        public void GetById_InvalidIdPassed_ReturnsNotFoundResult()//check if invalid Id is passed
        {
           
            //Arrange
            Mock <ICategoryService> moqRepo = new Mock <ICategoryService>(); //Mock is type of our Interface
            moqRepo.Setup(repo => repo.GetCategory(-1)).Returns(MyCategoryList().FirstOrDefault(i => i.CategoryId == -1));//access the function inside the service class and specify what it returns
            CategoryController controller = new CategoryController(moqRepo.Object);//pass moq object inside controller

            //Act
            ActionResult <CategoryDTO> not_Found_Result = controller.GetById(-1);//-1 is Invalid Id

            //Assert
            Assert.IsType<NotFoundResult>(not_Found_Result.Result);
            
        }
     
        //Test GetById() Method
        //When GetById() returns the correct item
        [Fact]
        public void GetById_ExistingIntIdPassed_ReturnsRightItem()
        {
           
            //Arrange
            Mock <ICategoryService> moqRepo = new Mock <ICategoryService>();//Mock is type of our Interface
            moqRepo.Setup(repo => repo.GetCategory(1)).Returns(MyCategoryList().FirstOrDefault(i => i.CategoryId == 1));//access the function inside the service class and specify what it returns
            CategoryController controller = new CategoryController(moqRepo.Object);//pass moq object inside controller
        
            //Act
            OkObjectResult okResult = controller.GetById(1).Result as OkObjectResult;
        
            //Assert
            Assert.Equal("Items", (okResult.Value as CategoryDTO).CategoryName);
            
        }
       
        //Test Post() Method 
        //When Invalid object is passed 
        [Fact]
        public async Task CategoryModelValidation_CategoryNameRequired()
        {
           
            //Arrange
            Mock <ICategoryService> moqRepo = new Mock <ICategoryService>();//Mock is type of our Interface

            //This Category does not contain CategoryName hence the Category is invalid
            CategoryDTO categoryNameIsMissing = new CategoryDTO();
        
            moqRepo.Setup(Repo=>Repo.CreateCategory(categoryNameIsMissing));
            CategoryController controller = new CategoryController(moqRepo.Object);//pass moq object inside controller
            controller.ModelState.AddModelError("CategoryName","Required");

            //Act
            ActionResult result = await controller.Post(categoryNameIsMissing);

            //Assert
            BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequest.Value);  
            
        }
        

        //Test Post() Method 
        //When valid object is passed 
        [Fact]
        public async Task Post_ValidObject_ReturnsOkResult()
        {
            //Arrange
            Mock <ICategoryService> moqRepo = new Mock <ICategoryService>();//Mock is type of our Interface
           
            CategoryDTO testData = new CategoryDTO()
            {
                //since we have identity column then CategoryId will be auto generated
                CategoryName = "Items" 
            };

            moqRepo.Setup(repo => repo.CreateCategory(testData));//access the function inside the service class and specify what it returns

            CategoryController controller = new CategoryController(moqRepo.Object);//pass moq object inside controller

            //Act
            ActionResult createdResponse = await controller.Post(testData);//Response data

            //Assert
            Assert.IsType<OkObjectResult>(createdResponse);
            var okResult = Assert.IsType<OkObjectResult>(createdResponse);
            Assert.True((okResult.Value as CategoryDTO).CategoryId > 0); // or we can say : Assert.NotEqual(0, (okResult.Value as CategoryDTO).CategoryId);
    
        }
        
        //Test Put() Method
        //When Non Existing Category is Passed 
        [Fact]
        public async Task Put_NotExistingCategoryPassed_ReturnsNotFoundResponse()
        {
            
            //Arrange
            Mock <ICategoryService> moqRepo = new Mock <ICategoryService>();//Mock is type of our Interface
            moqRepo.Setup(repo => repo.UpdateCategory(null));//access the function inside the service class and specify what it returns
            CategoryController controller = new CategoryController(moqRepo.Object);//pass moq object inside controller
        
            //Act
            ActionResult badResponse = await controller.Put(null);//non existing Category is passed
        
            //Assert
            Assert.IsType <NotFoundResult>(badResponse);

        }

        //Test Put() Method
        //When Existing Category is Passed 
        [Fact]
        public async Task Put_ExistingCategoryPassed_ReturnsOkResult()
        {
            //Arrange
            Mock <ICategoryService> moqRepo = new Mock <ICategoryService>();//Mock is type of our Interface
            CategoryDTO category = new CategoryDTO()
            {
                CategoryId = 1,
                CategoryName ="Items"
            };
            moqRepo.Setup(repo => repo.UpdateCategory(category));//access the function inside the service class and specify what it returns
            CategoryController controller = new CategoryController(moqRepo.Object);//pass moq object inside controller
        
            //Act
            ActionResult okResponse = await controller.Put(category);//existing Category is passed
        
            //Assert
            Assert.IsType <OkObjectResult> (okResponse);

        }
        
        //Test Delete() Method
        //When null is Passed
        [Fact]
        public async Task Remove_NullPassed_ReturnsNotFoundResponse()
        {
           
            //Arrange
            Mock <ICategoryService> moqRepo = new Mock <ICategoryService>();//Mock is type of our Interface
            moqRepo.Setup(repo => repo.DeleteCategory(null));//access the function inside the service class and specify what it returns
            CategoryController controller = new CategoryController(moqRepo.Object);//pass moq object inside controller
    
            //Act
            ActionResult badResponse = await controller.Delete(null);

            //Assert
            Assert.IsType<NotFoundResult>(badResponse);
            
        }

        //Test Delete() Method
        //When Existing Id is Passed
        [Fact]
        public async Task Remove_ExistingIntIdPassed_ReturnsOkResult()
        {
            
            //Arrange
            Mock <ICategoryService> moqRepo = new Mock <ICategoryService>();//Mock is type of our Interface
            moqRepo.Setup(repo => repo.DeleteCategory(1));//access the function inside the service class and specify what it returns
            CategoryController controller = new CategoryController(moqRepo.Object);//pass moq object inside controller
            
            //Act
            ActionResult okResponse = await controller.Delete(1);

            //Assert
            Assert.IsType<OkResult>(okResponse);
                    
        } 
    }
}