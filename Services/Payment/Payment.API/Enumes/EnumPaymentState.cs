using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Azure.Core.HttpHeader;

namespace Payment.API.Enums
{
    public enum EnumPaymentState : short
    {
                
        [Display(Name = "پرداخت موفق")]
        PaymentSucceeded= 1,
        
        [Display(Name = "پرداخت ناموفق")]
        PaymentFailed = 2

    }
}
