using MicroShop.Domain.DTOs.Product;

namespace MicroShop.Core.Commands
{
    public record AddProductCommand(AddProductCommandDto AddDto) : IRequest<ResultDto<Unit>>
    {

    }
}
