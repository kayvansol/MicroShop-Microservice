using System;
using System.Collections.Generic;

namespace Payment.API.Entities;

public partial class Payment : BaseEntity<int>
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string? CardNumber { get; set; }

    public short? PaymentMethod { get; set; }

    public short? Status { get; set; }
}
