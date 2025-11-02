using MicroShop.Domain.DTOs.Product;

namespace MicroShop.Core.Queries
{
    public record GetAllProductQuery : IRequest<ResultDto<List<GetAllProductDto>>>
    {

    }
}
