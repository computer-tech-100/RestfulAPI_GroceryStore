using MyApp.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using MyApp.Core.Contexts;

namespace MyApp.UnitTests{
    public static class TestData
    {
        //For Category unit testing we add some dummy test data to the database (we add it to Categories table)
        public  static Category  CategoryTestData(this ShoppingCartContext x)
        {
            //Check if database is created, if not - create
            x.Database.EnsureCreated();

            var My_Category = new Category()
            {
                CategoryId = 1,
                CategoryName="Clothes"
            };
            x.Categories.Add(My_Category); //add data to Categories table   
            x.SaveChanges();
            return My_Category;
        }

        //For Product unit testing we add some dummy test data to the database (we add it to Products table)
        public static Product ProductTestData(this ShoppingCartContext x)
        {
            //Check if database is created, if not - create
            x.Database.EnsureCreated();
            var The_Category = new Category()
            {
                CategoryId = 3,
                CategoryName="Fruits"
            };
            var My_Product = new Product()
            {
            
                ProductId=1,
                ProductName="Oranges",
                Price=2,
                CategoryId=3,
                Category=The_Category
            };
            x.Products.Add(My_Product); //add data to Products table   
            x.SaveChanges();
            return My_Product;
        }
    
        //For CartItem unit testing we add some dummy test data to the database (we add it to CartItems table)
        public static CartItem CartItemTestData(this ShoppingCartContext x)
        {
            //Check if database is created, if not - create
            x.Database.EnsureCreated();
            var The_Category = new Category()
            {
                CategoryId = 3,
                CategoryName="Fruits"
            };
            var My_Product = new Product()
            {
                ProductId = 1,
                ProductName="Oranges",
                Price=2,
                CategoryId=3,
                Category=The_Category
            };
            var My_CartItem = new CartItem()
            {
                ProductId = 1,
                Product=My_Product,
                Price=2,
                Quantity=5 
            };
            x.CartItems.Add(My_CartItem); //add data to CartItems table   
            x.SaveChanges();
            return My_CartItem;
        }
    
        //For Cart unit testing we add some dummy test data to the database (we add it to Carts table)
        public static Cart CartTestData(this ShoppingCartContext x)
        {
            //Check if database is created, if not - create
            x.Database.EnsureCreated();
            var The_Category = new Category()
            {
                CategoryId = 2,
                CategoryName="Items"
            };
            var The_Category2 = new Category()
            {
                CategoryId = 3,
                CategoryName="Fruits"
            };
            var My_Product1 = new Product()
            {
                ProductId = 3,
                ProductName="Chocolate",
                Price=3,
                CategoryId=2,
                Category=The_Category
            };
            var My_Product2 = new Product()
            {
                ProductId = 1,
                ProductName="Oranges",
                Price=2,
                CategoryId=3,
                Category=The_Category2
            };
            var My_CartItem1 = new CartItem()
            {
                ProductId = 3,
                Product=My_Product1,
                Price=3,
                Quantity=2   
            };
            var My_CartItem2 = new CartItem()
            {
                ProductId = 1,
                Product=My_Product2,
                Price=2,
                Quantity=5  
            };
            List<CartItem> All_Cart_Items = new List<CartItem>();
            All_Cart_Items.Add(My_CartItem1);
            All_Cart_Items.Add(My_CartItem2);
            var My_Cart = new Cart(){
                CartId=1,
                CartName="My Cart",
                AllCartItems=All_Cart_Items,
                GrandTotal=16 // --> (3*2)+(2*5)
            };
            x.Carts.Add(My_Cart);//add data to Carts table   
            x.SaveChanges();
            return My_Cart;
        }
    }
}