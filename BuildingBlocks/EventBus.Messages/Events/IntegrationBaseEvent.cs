using System;

namespace EventBus.Messages.Events
{
    public class IntegrationBaseEvent
    {
		public IntegrationBaseEvent()
		{
            //CorrelationId = Guid.NewGuid();
			CreationDate = DateTime.Now;
		}

		public IntegrationBaseEvent(Guid id, DateTime createDate)
		{
            CorrelationId = id;
			CreationDate = createDate;
		}

		public Guid CorrelationId { get; set; }

		public DateTime CreationDate { get; set; }
	}
}
