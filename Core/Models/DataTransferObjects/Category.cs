using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace MyApp.Core.Models.DataTransferObjects
{
    //This is model for Category
    public class CategoryDTO
    {
        //Category properties which are columns in the table
        public int CategoryId { get; set; }//Field CategoryId is primary key
        //Name field is required and it's length should not be less than 2
        [Required, MinLength(2, ErrorMessage = "Minimum length is 2")]
        public string CategoryName { get; set; }
    }
}