using MessageTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSR_Backend
{
    internal class Timeout : ITimeout
    {
        private Uri _responseAddress = new("http://localhost");
        public Uri ResponseAddress
        {
            get => _responseAddress;
            set { _responseAddress = value; }
        }
        public Guid CorrelationId { get; set; }
    }

    internal class RoomsResponse : IRoomsResponse
    {
        public object Message { get; set; }

        public RoomsResponse(object message)
        {
            Message = message;
        }
    }

    internal class RoomStatusChangeResponse : IRoomStatusChangeResponse
    {
        public int RoomID { get; set; }
        public string RoomStatus { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
