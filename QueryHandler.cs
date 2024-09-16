using MassTransit;
using MassTransit.Saga;
using MessageTypes;

namespace KSR_Backend
{
    internal class QueryHandlerClass : IConsumer<MessageTypes.IRoomsQuery>
    {
        private ISendEndpoint SendEndpoint { get; set; }
        public RoomRepository RoomRepository { get; set; }
        public RoomTypeRepository RoomTypeRepository { get; set; }
        public QueryHandlerClass() 
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(new Uri("rabbitmq://cow.rmq2.cloudamqp.com/uklfscur"), h =>
                {
                    h.Username("uklfscur");
                    h.Password("l-2PHIXrRbG2lreh61yvypU6sx4dPGoi");
                });
            });

            var task = bus.GetSendEndpoint(new Uri("rabbitmq://cow.rmq2.cloudamqp.com/uklfscur/responsequeue"));
            task.Wait();
            SendEndpoint = task.Result;
        }

        public Task Consume(ConsumeContext<IRoomsQuery> context)
        {
            //query do bazy
            var result = RoomRepository.GetAll();
            return SendEndpoint.Send(new RoomsResponse(result));
        }
    }
}
