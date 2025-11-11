using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MicroShop.Domain.Enums
{
    public enum EnumOrderState : byte
    {
        
        [Display(Name = "در انتظار پرداخت")]
        Pending = 0,

        [Display(Name = "عدم موجودی")]
        OutOfStock = 1,

        [Display(Name = "پرداخت‌ شده")]
        Paid = 2,

        [Display(Name = "پرداخت ناموفق")]
        PaymentFailed = 3,

        [Display(Name = "در حال پردازش")]
        Processing = 4,

        [Display(Name = "آماده ارسال")]
        Packed = 5

    }
}
