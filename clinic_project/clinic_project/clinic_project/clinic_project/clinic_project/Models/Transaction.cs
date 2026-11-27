using System;
using System.Collections.Generic;

namespace clinic_project.Models;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public string? Type { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? OrderId { get; set; }

    public virtual Order? Order { get; set; }
}
