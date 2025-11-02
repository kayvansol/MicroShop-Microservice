using MicroShop.Domain.DTOs.Category;

namespace MicroShop.Core.Queries
{
    public record GetAllCategoryQuery(int statrtPage,int pageSize) : IRequest<ResultDto<List<GetAllCategoryDto>>>
    {

    }
}
