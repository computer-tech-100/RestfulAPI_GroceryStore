using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Core.Models.DbEntities
{
    public class Cart
    {
        public int CartId{ get; set; } = 1;//Primary Key
        public string CartName{ get; set; } = "My Cart";
        public List<CartItem> AllCartItems { get; set; } //Cart is list of CartItems
        public decimal GrandTotal{ get; set; }//sum of SubTotals
    }
}