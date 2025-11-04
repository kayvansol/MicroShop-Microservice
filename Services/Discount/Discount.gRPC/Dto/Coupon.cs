using System;
using System.Collections.Generic;

namespace Discount.gRPC;

public partial class CouponModel
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int? Amount { get; set; }

    public string? Description { get; set; }
}
