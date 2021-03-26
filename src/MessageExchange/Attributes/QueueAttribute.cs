using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageExchange.Attributes
{
    public class QueueAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
