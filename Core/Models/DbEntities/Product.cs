using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;//ForeignKey

namespace MyApp.Core.Models.DbEntities
{
    public class Product
    {
        //Product properties which are columns in the table
        public int ProductId { get; set; } //ProductId is primary key
        [Required]
        public string ProductName { get; set; }
        public decimal Price{ get; set; } = 0;
        //Use foreign key to link data between categories table and products table
        //CategoryId is foreign key that references the primary key in Categories table
        public int CategoryId { get; set; } //Foreign key
        
        [ForeignKey("CategoryId")]//CategoryId is Foreign Key 
        public Category Category { get; set; }
    }
}