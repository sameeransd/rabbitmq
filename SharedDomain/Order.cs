using System;
using System.Collections.Generic;
using System.Text;

namespace SharedDomain
{
    public class Order
    {
        public Order(int id, decimal total)
        {
            Id = id;
            TotalValue = total;
        }

        public int Id { get; set; }
        public decimal TotalValue { get; set; }
    }
}
