using System;
using System.Collections.Generic;

namespace clinic_project.Models;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int? CartId { get; set; }

    public string? ProductType { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public virtual Cart? Cart { get; set; }
}
