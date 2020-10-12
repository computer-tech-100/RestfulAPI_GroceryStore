using MyApp.Core.Models.DbEntities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using MyApp.Core.Models.DataTransferObjects;

namespace MyApp.Core.Contexts
{
    //DbContext is a class in Entity Framework. It acts like a bridge between entity classes and the database.
    //Our shoppingCartContext inherits from DBContext that relates to the database
    public class ShoppingCartContext : DbContext 
    {
        //Constructor
        public ShoppingCartContext(DbContextOptions<ShoppingCartContext> options):base(options)
        {  
        } 
        
        //Create and access the tables in database
        public virtual DbSet<Category> Categories {get;set;} //DbSet of type Category (we called it Categories) 
        public DbSet<Product> Products {get;set;} //DbSet of type Product (we called it Products) 
        public DbSet<CartItem> CartItems {get; set;} //DbSet of type CartItem (we called it CartItems) 
        public DbSet<Cart> Carts {get; set;} //DbSet of type Cart (we called it Carts)        
    }   
}