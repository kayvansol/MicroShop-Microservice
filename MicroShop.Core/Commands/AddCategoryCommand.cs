using MicroShop.Domain.DTOs.Category;

namespace MicroShop.Core.Commands
{
    public record AddCategoryCommand(AddCategoryCommandDto AddDto) : IRequest<ResultDto<Unit>>
    {
        
    }
}
