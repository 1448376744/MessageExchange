using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageExchange.RabbitMQ.Attributes
{
    public class QueueAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
