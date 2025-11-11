using AutoMapper;
using Basket.API.Entities;
using Basket.API.gRPCServices;
using Basket.API.Repositories.Interfaces;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly DiscountgRPCService _discountgRPCService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;


        public BasketController(IBasketRepository repository, DiscountgRPCService discountgRPCService,
            IPublishEndpoint publishEndpoint, IMapper mapper)
        {

            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _discountgRPCService = discountgRPCService ?? throw new ArgumentNullException(nameof(discountgRPCService));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }


        [HttpGet("{customerId}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(int customerId)
        {
            var basket = await _repository.GetBasket(customerId);
            return Ok(basket ?? new ShoppingCart(customerId));
        }


        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            // TODO : Communicate with Discount.gRPC
            // and Calculate latest prices of product into shopping cart
            // consume Discount gRPC

            //TODO: Get Discount for all the items in Basket in gRPC Call.
            
            foreach (var item in basket.Items)
            {
                var coupon = await _discountgRPCService.GetDiscount(item.ProductId);
                item.Price -= coupon.Amount;
                item.Discount = coupon.Amount;
            }

            return Ok(await _repository.UpdateBasket(basket));
        }


        [HttpDelete("{customerId}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> DeleteBasket(int customerId)
        {
            bool status = await _repository.DeleteBasket(customerId);
            return Ok(status);
        }


        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            // get existing basket with total price 
            // Create basketCheckoutEvent -- Set TotalPrice on basketCheckout eventMessage
            // send checkout event to rabbitmq
            // remove the basket

            // get existing basket with total price

            var basket = await _repository.GetBasket(basketCheckout.CustomerId);
            if (basket == null)
            {
                return BadRequest();
            }


            // send checkout event to rabbitmq

            /*
            var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.TotalPrice = basket.TotalPrice;
            eventMessage.BasketItems = ""; // JsonConvert.SerializeObject(basket.Items);// Products List
            
            await _publishEndpoint.Publish(eventMessage);
            */


            await _publishEndpoint.Publish(new BasketCheckoutEvent
            {                
                BasketItems = JsonConvert.SerializeObject(basket.Items), // Products List
                TotalPrice = basket.TotalPrice,
                CustomerId = basket.CustomerId,
                CorrelationId = Guid.NewGuid(),
                OrderId = 0
            });


            // remove the basket

            bool status = await _repository.DeleteBasket(basket.CustomerId);

            //Note : this is for ensuring that message delivered to the queue ...

            //Thread.Sleep(5000); 

            return Accepted(status);
        }


    }

}
