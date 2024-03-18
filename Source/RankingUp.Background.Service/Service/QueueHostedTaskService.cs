using Microsoft.Extensions.Hosting;
using RankingUp.Background.Service.Interfaces;
using RankingUp.Core.Messages;
using System.Diagnostics;

namespace RankingUp.Background.Service.Service
{
    public sealed class QueueHostedTaskService : BackgroundService
    {
        private readonly IRunEventTaskService _runEventTaskService;
        private Event _eventExecution = null;
        private const int MaxQueueMillisegundsSecundsToRead = 100; 

        public QueueHostedTaskService(IRunEventTaskService runEventTaskService)
        {
            _eventExecution = null;
            _runEventTaskService = runEventTaskService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ProcessTaskQueueAsync(stoppingToken);
        }

        private async Task ProcessTaskQueueAsync(CancellationToken stoppingToken)
        {
            Debug.WriteLine($"Start to read the queue to execute");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if(QueueTaskEvent.Instance.QueueTask.Any() && _eventExecution is null)
                    {
                        _eventExecution = QueueTaskEvent.Instance.QueueTask.First();
                        await _runEventTaskService.RunAsync(_eventExecution);
                        QueueTaskEvent.Instance.RemoveTask(_eventExecution);
                        _eventExecution = null;
                    }
                    else
                        await Task.Delay(TimeSpan.FromMilliseconds(MaxQueueMillisegundsSecundsToRead));
                }
                catch (OperationCanceledException)
                {
                    // Prevent throwing if stoppingToken was signaled
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex, "Error occurred executing task work item.");
                }
            }
        }
    }
}
