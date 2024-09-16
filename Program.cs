using MassTransit;
using MassTransit.Serialization;
using MassTransit.Saga;
using MassTransit.SagaStateMachine;
using MessageTypes;
using MassTransit.Internals;

using Microsoft.EntityFrameworkCore;

namespace KSR_Backend
{
    public class CommandData : SagaStateMachineInstance
    {
        private Uri _responseAddress = new("http://localhost");
        public Uri ResponseAddress
        {
            get => _responseAddress;
            set { _responseAddress = value; }
        }
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public string SessionID { get; set; }
        public int RoomID { get; set; }
        public string RoomStatus { get; set; }
        public Guid? TimeoutId { get; set; }
    }

    public class CommandSaga : MassTransitStateMachine<CommandData> 
    { 
        public State Requested { get; private set; }
        public Event<MessageTypes.IRoomStatusChangeRequest> StartEvent { get; private set; }
        public Schedule<CommandData, MessageTypes.ITimeout> TO { get; private set; }

        public CommandSaga() 
        {
            InstanceState(x => x.CurrentState);

            Event(() => StartEvent,
                x => x.CorrelateBy(
                    s => s.SessionID,
                    ctx => ctx.Message.SessionID)
                .SelectId(ctx => Guid.NewGuid()));

            Schedule(() => TO,
                x => x.TimeoutId,
                x => { x.Delay = TimeSpan.FromSeconds(10); });

            Initially(
                When(StartEvent)
                    .Then(ctx =>
                    {
                        ctx.Saga.SessionID = ctx.Message.SessionID;
                        ctx.Saga.RoomID = ctx.Message.RoomID;
                        ctx.Saga.RoomStatus = ctx.Message.RoomStatus;
                        ctx.Saga.ResponseAddress = ctx.ResponseAddress;
                    })
                    .Schedule(TO, ctx => new Timeout() { CorrelationId = ctx.Saga.CorrelationId, ResponseAddress = ctx.Saga.ResponseAddress }, ctx =>
                    {
                        ctx.ResponseAddress = ctx.Message.ResponseAddress;
                    })
                    .Respond(ctx =>
                    {
                        

                        return new RoomStatusChangeResponse()
                        {
                            CorrelationId = ctx.Saga.CorrelationId,
                            RoomID = ctx.Saga.RoomID,
                            RoomStatus = ctx.Saga.RoomStatus
                        };
                    })
                    .Finalize()
            );
        }
    }

    internal class Program
    {
        private static readonly object ConsoleWriterLock = new();
        private static DateTime StartTime;

        static void PrintStatus()
        {
            Console.Clear();
            Console.WriteLine(Utils.printTimestamp() + "Process initialized. Press ESC to quit");
        }

        static void PrintStatistics()
        {
            TimeSpan uptime = DateTime.Now - StartTime;
            Console.WriteLine(Utils.printTimestamp() + "Server uptime: {0}", uptime);
        }

        static void Main(string[] args)
        {
            bool exitFlag = false;
            StartTime = DateTime.Now;

            var roomRepository = new RoomRepository();
            var roomTypeRepository = new RoomTypeRepository();
            var sagaRepository = new InMemorySagaRepository<CommandData>();
            var commandMachine = new CommandSaga();

            QueryHandlerClass queryHandler = new QueryHandlerClass() { 
                RoomRepository = roomRepository, 
                RoomTypeRepository = roomTypeRepository 
            };

            var commandBus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(new Uri("rabbitmq://cow.rmq2.cloudamqp.com/uklfscur"), h =>
                {
                    h.Username("uklfscur");
                    h.Password("l-2PHIXrRbG2lreh61yvypU6sx4dPGoi");
                });
                sbc.ReceiveEndpoint("commandqueue", ep =>
                {
                    //TODO: saga config
                    //ep.Instance(commandHandler);
                    ep.StateMachineSaga(commandMachine, sagaRepository);
                    ep.UseMessageRetry(r =>
                    {
                        r.Immediate(5);
                    });
                });
                //sbc.UseInMemoryScheduler();
            });

            var queryBus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(new Uri("rabbitmq://cow.rmq2.cloudamqp.com/uklfscur"), h =>
                {
                    h.Username("uklfscur");
                    h.Password("l-2PHIXrRbG2lreh61yvypU6sx4dPGoi");
                });
                sbc.ReceiveEndpoint("queryqueue", ep =>
                {
                    ep.Instance(queryHandler);
                    ep.UseMessageRetry(r =>
                    {
                        r.Immediate(5);
                    });
                });
            });

            commandBus.Start();
            queryBus.Start();

            //Keyboard input resoultion task
            Task.Factory.StartNew(() =>
            {
                ConsoleKey consoleKey = Console.ReadKey().Key;
                while (consoleKey != ConsoleKey.Escape)
                {
                    switch (consoleKey)
                    {
                        case ConsoleKey.S:
                            lock (ConsoleWriterLock)
                            {
                                PrintStatistics();
                            }
                            break;
                        default:
                            break;
                    }
                    consoleKey = Console.ReadKey().Key;
                }
                exitFlag = true;
            });

            PrintStatus();

            //Main loop
            while (!exitFlag)
            {
                continue;
            }

            commandBus.Stop();
            queryBus.Stop();
        }
    }
}
