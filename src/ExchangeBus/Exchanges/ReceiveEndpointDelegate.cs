using System.Threading.Tasks;

namespace ExchangeBus
{
    public delegate Task ReceiveEndpointDelegate(EventContext message);
}
