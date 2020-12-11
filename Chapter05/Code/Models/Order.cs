using System;
using System.Collections.Generic;

namespace Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public List<Product> Products { get; set; }
    }
}
