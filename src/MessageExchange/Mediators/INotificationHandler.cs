using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageExchange
{
    public interface INotificationHandler
    {
        
    }
    public interface INotificationHandler<in TNotification>: INotificationHandler
        where TNotification : INotification
    {
        Task Handle(TNotification notification, CancellationToken cancellationToken);
    }
}
