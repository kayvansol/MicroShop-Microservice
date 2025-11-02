using MicroShop.Domain.DTOs.Customer;

namespace MicroShop.Core.Commands
{
    public record AddCustomerCommand(AddCustomerCommandDto AddDto) : IRequest<ResultDto<Unit>>
    {

    }
}
