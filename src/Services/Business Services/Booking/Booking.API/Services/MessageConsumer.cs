using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.API.Services
{
    public class MessageConsumer : IMessageConsumer
    {
        public void ReceivingMessage()
        {
            try
            {

                var factory = new ConnectionFactory()
                {
                    HostName = "localhost",
                    UserName = "test",
                    Password = "test",
                    VirtualHost = "/"
                };
                var con = factory.CreateConnection();
                var channel = con.CreateModel();
                channel.QueueDeclare("book3", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, EventArgs) =>
                { 
                    var body = EventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(message);
                    Console.ReadKey();
                };
                channel.BasicConsume(queue: "book3", autoAck: true, consumer: consumer);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
