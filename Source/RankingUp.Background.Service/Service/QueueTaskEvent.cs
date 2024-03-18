using RankingUp.Core.Messages;

namespace RankingUp.Background.Service.Service
{
    public class QueueTaskEvent
    {
        private static QueueTaskEvent instance = null;

        private QueueTaskEvent()
        {
            _queueTask = new List<Event> { };
        }

        public static QueueTaskEvent Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new QueueTaskEvent();
                }
                return instance;
            }
        }

        public IReadOnlyList<Event> QueueTask { get => _queueTask; }

        private readonly List<Event> _queueTask; 

        public void AddTask(Event task)
        {
            _queueTask.Add(task);
        }

        public void RemoveTask(Event task) 
        {
            _queueTask.Remove(task);
        }


    }
}
