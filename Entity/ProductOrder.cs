﻿using Ecommerce.Entity;

namespace Ecommerce;

public class ProductOrder
{
    public int ProductId { get; set; }  
    public Product Product{ get; set; }

    public int OrderId { get; set; }
    public Order Order{ get; set; }
}
