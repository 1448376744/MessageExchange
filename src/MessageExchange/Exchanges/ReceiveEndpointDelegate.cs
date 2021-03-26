using System.Threading.Tasks;

namespace MessageExchange
{
    public delegate Task ReceiveEndpointDelegate(ReceiveEndpointContext message);
}
