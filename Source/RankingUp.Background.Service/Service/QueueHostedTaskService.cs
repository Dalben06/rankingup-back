using Microsoft.Extensions.Hosting;
using RankingUp.Background.Service.Interfaces;
using System.Diagnostics;

namespace RankingUp.Background.Service.Service
{
    public sealed class QueueHostedTaskService : BackgroundService
    {
        private readonly IRunEventTaskService _runEventTaskService;
        private const int MaxQueueMillisegundsSecundsToRead = 100; 

        public QueueHostedTaskService(IRunEventTaskService runEventTaskService)
        {
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
                if (!QueueTaskEvent.Instance.TryDequeue(out var eventExecution))
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(MaxQueueMillisegundsSecundsToRead), stoppingToken);
                    continue;
                }

                try
                {
                    await _runEventTaskService.RunAsync(eventExecution);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex, "Error occurred executing task work item.");
                    QueueTaskEvent.Instance.AddTask(eventExecution);
                    await Task.Delay(TimeSpan.FromMilliseconds(MaxQueueMillisegundsSecundsToRead), stoppingToken);
                }
            }
        }
    }
}
