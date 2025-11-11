using MicroShop.Domain;
using MicroShop.Domain.DTOs.Order;

namespace MicroShop.Core.Commands
{
    public record AddOrderCommand(AddOrderCommandDto AddDto) : IRequest<ResultDto<OrderDto>>
    {

    }
}
