using MassTransit;
using MassTransit.Saga;
using MessageTypes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KSR_Backend
{
    internal class CommandHandlerClass : IConsumer<MessageTypes.IRoomUpdate>, IConsumer<MessageTypes.IRoomDelete>
    {
        public RoomRepository RoomRepository { get; set; }
        public RoomTypeRepository RoomTypeRepository { get; set; }

        public async Task Consume(ConsumeContext<IRoomUpdate> context)
        {
            Console.WriteLine(Utils.printTimestamp() + "RoomUpdateCommand");
            //komenda
            var result = JsonSerializer.Serialize(
                RoomRepository.Save(
                    new ReadModel.RoomDisplay() {
                        Id = context.Message.RoomId,
                        Name = context.Message.Name,
                        Status = context.Message.Status,
                        TypeId = context.Message.RoomType
                    }));
            await context.RespondAsync(new RoomsResponse(result));
        }

        public async Task Consume(ConsumeContext<IRoomDelete> context)
        {
            Console.WriteLine(Utils.printTimestamp() + "RoomDeleteCommand");
            //komenda
            var result = JsonSerializer.Serialize(RoomRepository.Delete(RoomRepository.FindById(context.Message.RoomId)));
            await context.RespondAsync(new RoomsResponse(result));
        }
    }
}
