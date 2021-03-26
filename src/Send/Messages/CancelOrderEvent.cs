using MessageExchange;
using MessageExchange.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Send
{
    [Queue(Name ="order")]
    public class CancelOrderMessage : IMessage
    {
        public string OrderId { get; set; }
    }
}
