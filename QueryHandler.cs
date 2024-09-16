using MassTransit;
using MassTransit.Saga;
using MessageTypes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KSR_Backend
{
    internal class QueryHandlerClass : IConsumer<MessageTypes.IRoomsQuery>
    {
        public RoomRepository RoomRepository { get; set; }
        public RoomTypeRepository RoomTypeRepository { get; set; }

        public async Task Consume(ConsumeContext<IRoomsQuery> context)
        {
            Console.WriteLine(Utils.printTimestamp() + "RoomsQuery");
            //query do bazy
            var result = JsonSerializer.Serialize(RoomRepository.GetAll());
            //return SendEndpoint.Send(new RoomsResponse(result));
            await context.RespondAsync(new RoomsResponse(result));
        }
    }
}
