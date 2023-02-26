using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RankingUp.Core.Domain
{
    public class Notifiable
    {
        public Notifiable()
        {
            _notifications = new List<string>();
        }
        [JsonIgnore]
        private List<string> _notifications;
        [Computed]
        [JsonIgnore]
        public IList<string> Notifications => this._notifications;
        [Computed]
        [JsonIgnore]
        public bool HasNotifications => _notifications?.Any() ?? false;
        [Computed]
        [JsonIgnore]
        public bool Invalid => HasNotifications;
        [Computed]
        [JsonIgnore]
        public bool Valid => !HasNotifications;

        public void AddNotification(string message)
        {
            _notifications.Add(message);
        }
        public void AddNotification(string key, string message)
        {
            if (!message?.StartsWith(key) ?? false)
                _notifications.Add($"{key} {message}");
            else
                _notifications.Add(message);
        }

        public void AddNotifications(IList<string> notifications)
        {
            if (notifications?.Any() ?? false)
                _notifications.AddRange(notifications);
        }

        public void AddNotifications(ICollection<string> notifications)
        {
            if (notifications?.Any() ?? false)
                _notifications.AddRange(notifications);
        }

        public void AddNotifications(INotifiable item)
        {
            if (item?.HasNotifications ?? false)
                AddNotifications(item.Notifications);
        }

        public void AddNotifications(params INotifiable[] items)
        {
            foreach (var item in items)
                AddNotifications(item);
        }

        public bool Includes(string notification)
        {
            return _notifications.Contains(notification);
        }
    }
}
