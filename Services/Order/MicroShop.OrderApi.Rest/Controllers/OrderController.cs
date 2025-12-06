
using MicroShop.Domain.DTOs.Order;

namespace MicroShop.OrderApi.Rest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : BaseController
    {

        [AllowAnonymous]
        [HttpPost("GetAllOrders")]
        public async Task<ResultDto<List<GetAllOrders>>> GetAllOrders(GetAllOrdersQuery query, CancellationToken cancellationToken) => await Mediator.Send(query, cancellationToken);


        [AllowAnonymous]
        [HttpPost("GetOrderItems")]
        public async Task<ResultDto<List<GetOrderItems>>> GetOrderItems(GetOrderItemsQuery query, CancellationToken cancellationToken) => await Mediator.Send(query, cancellationToken);


        [AllowAnonymous]
        [HttpPost("InsertOrder")]
        public async Task<ResultDto<OrderDto>> InsertOrder(AddOrderCommand command, CancellationToken cancellationToken) => await Mediator.Send(command, cancellationToken);


        [AllowAnonymous]
        [HttpPost("WaitingPayments")]
        public async Task<ResultDto<List<WaitingPaymentDto>>> WaitingPayments(GetAllWaitingPaymentsQuery query, CancellationToken cancellationToken) => await Mediator.Send(query, cancellationToken);


    }
}
