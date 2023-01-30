using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booking.API.Services
{
    public interface IMessageProducer
    {
        public void SendingMessage<T>(T message);
    }
}
