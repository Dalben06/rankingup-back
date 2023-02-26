using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingUp.Core.Domain
{
    public interface INotifiable
    {
        IList<string> Notifications { get; }
        bool HasNotifications { get; }

        bool Includes(string notification);
        void AddNotification(string key, string message);
        void AddNotification(string message);
        void AddNotifications(IList<string> notifications);
        void AddNotifications(ICollection<string> notifications);
        void AddNotifications(INotifiable item);
        void AddNotifications(params INotifiable[] items);
        bool Invalid { get; }
        bool Valid { get; }
    }
}
