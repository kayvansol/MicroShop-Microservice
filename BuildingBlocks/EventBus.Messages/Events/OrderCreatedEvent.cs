using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Messages.Events
{
    public class OrderCreatedEvent : IntegrationBaseEvent
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime Created { get; set; }
    }
}
