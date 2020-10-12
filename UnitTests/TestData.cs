using MyApp.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using MyApp.Core.Contexts;
using MyApp.Core.Models.DbEntities;

namespace MyApp.UnitTests{
    public static class TestData
    {
        //For Category unit testing we add some dummy test data to the database (we add it to Categories table)
        public  static Category CategoryTestData(this ShoppingCartContext context)
        {
            //Check if database is created, if not - create
            context.Database.EnsureCreated();

            Category my_Category = new Category()
            {
                CategoryId = 1,
                CategoryName = "Clothes"
            };

            context.Categories.Add(my_Category); //add data to Categories table   
            context.SaveChanges();
            return my_Category;
        }

        //For Product unit testing we add some dummy test data to the database (we add it to Products table)
        public static Product ProductTestData(this ShoppingCartContext context)
        {
            //Check if database is created, if not - create
            context.Database.EnsureCreated();
            Category the_Category = new Category()
            {
                CategoryId = 3,
                CategoryName ="Fruits"
            };

            Product my_Product = new Product()
            {
            
                ProductId = 1,
                ProductName = "Oranges",
                Price=2,
                CategoryId = 3,
                Category = the_Category
            };

            context.Products.Add(my_Product); //add data to Products table   
            context.SaveChanges();
            return my_Product;
        }
    
        //For CartItem unit testing we add some dummy test data to the database (we add it to CartItems table)
        public static CartItem CartItemTestData(this ShoppingCartContext context)
        {
            //Check if database is created, if not - create
            context.Database.EnsureCreated();
            Category the_Category = new Category()
            {
                CategoryId = 3,
                CategoryName = "Fruits"
            };

            Product my_Product = new Product()
            {
                ProductId = 1,
                ProductName = "Oranges",
                Price = 2,
                CategoryId = 3,
                Category = the_Category
            };

            CartItem my_CartItem = new CartItem()
            {
                ProductId = 1,
                Product = my_Product,
                Price = 2,
                Quantity = 5 
            };

            context.CartItems.Add(my_CartItem); //add data to CartItems table   
            context.SaveChanges();
            return my_CartItem;
        }
    
        //For Cart unit testing we add some dummy test data to the database (we add it to Carts table)
        public static Cart CartTestData(this ShoppingCartContext context)
        {
            //Check if database is created, if not - create
            context.Database.EnsureCreated();
            Category the_Category = new Category()
            {
                CategoryId = 2,
                CategoryName = "Items"
            };

            Category the_Category2 = new Category()
            {
                CategoryId = 3,
                CategoryName = "Fruits"
            };

            Product my_Product1 = new Product()
            {
                ProductId = 3,
                ProductName = "Chocolate",
                Price = 3,
                CategoryId = 2,
                Category = the_Category
            };

            Product my_Product2 = new Product()
            {
                ProductId = 1,
                ProductName = "Oranges",
                Price = 2,
                CategoryId = 3,
                Category = the_Category2
            };

            CartItem my_CartItem1 = new CartItem()
            {
                ProductId = 3,
                Product = my_Product1,
                Price = 3,
                Quantity = 2   
            };

            CartItem my_CartItem2 = new CartItem()
            {
                ProductId = 1,
                Product = my_Product2,
                Price = 2,
                Quantity = 5  
            };

            List<CartItem> All_Cart_Items = new List<CartItem>();
            All_Cart_Items.Add(my_CartItem1);
            All_Cart_Items.Add(my_CartItem2);

            Cart my_Cart = new Cart(){
                CartId = 1,
                CartName = "My Cart",
                AllCartItems = All_Cart_Items,
                GrandTotal = 16 // --> (3*2)+(2*5)
            };
            
            context.Carts.Add(my_Cart);//add data to Carts table   
            context.SaveChanges();
            return my_Cart;
        }
    }
}