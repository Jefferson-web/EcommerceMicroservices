using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Entities
{
    public class ShoppingCart
    {
        public string UserName { get; set; }
        public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();
        public ShoppingCart(){}
        public ShoppingCart(string UserName)
        {
            this.UserName = UserName;
        }

        public decimal TotalPrice {
            get {
                decimal totalPrice = 0;
                foreach (var item in this.Items)
                {
                    totalPrice += item.Quantity * item.Price;
                }
                return totalPrice;
            }
        }

    }
}
