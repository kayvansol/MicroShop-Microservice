using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroShop.Domain.Enums
{
    public enum EnumOrderState : byte
    {
        
        [Display(Name = "در انتظار پرداخت")]
        Pending = 0,

        [Display(Name = "پرداخت‌ شده")]
        Paid = 1,

        [Display(Name = "در حال پردازش")]
        Processing = 2,

        [Display(Name = "آماده ارسال")]
        Packed = 3

    }
}
