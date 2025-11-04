using System;
using System.Collections.Generic;

namespace Discount.API.Context;

public partial class Coupon : BaseEntity<int>
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int? Amount { get; set; }

    public string? Description { get; set; }
}
