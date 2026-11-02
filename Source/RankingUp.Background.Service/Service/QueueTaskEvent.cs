using RankingUp.Core.Messages;

namespace RankingUp.Background.Service.Service
{
    public class QueueTaskEvent
    {
        private static readonly Lazy<QueueTaskEvent> instance = new(() => new QueueTaskEvent());
        private readonly object _queueLock = new();

        private QueueTaskEvent()
        {
            _queueTask = new List<Event> { };
        }

        public static QueueTaskEvent Instance => instance.Value;

        public IReadOnlyList<Event> QueueTask
        {
            get
            {
                lock (_queueLock)
                    return _queueTask.ToArray();
            }
        }

        private readonly List<Event> _queueTask; 

        public void AddTask(Event task)
        {
            lock (_queueLock)
                _queueTask.Add(task);
        }

        public void RemoveTask(Event task) 
        {
            lock (_queueLock)
                _queueTask.Remove(task);
        }

        public bool TryDequeue(out Event task)
        {
            lock (_queueLock)
            {
                if (_queueTask.Count == 0)
                {
                    task = null;
                    return false;
                }

                task = _queueTask[0];
                _queueTask.RemoveAt(0);
                return true;
            }
        }


    }
}
