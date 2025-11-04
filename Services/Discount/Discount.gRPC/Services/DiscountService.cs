
using Discount.gRPC.Protos;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Discount.gRPC.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Discount.gRPC.Services
{
    public class DiscountService :  DiscountProtoService.DiscountProtoServiceBase
    {
        
        public DiscountService()
        {
            
        }

        public override async Task<Discount.gRPC.Protos.CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {

            DiscountRepository _repository = new DiscountRepository();

            return await _repository.GetDiscount(int.Parse(request.ProductId));

        }

    }
}