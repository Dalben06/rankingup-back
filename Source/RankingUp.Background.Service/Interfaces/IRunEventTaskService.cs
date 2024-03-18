using RankingUp.Core.Messages;

namespace RankingUp.Background.Service.Interfaces
{
    public interface IRunEventTaskService
    {
        Task<bool> RunAsync<T>(T @event) where T : Event;
    }
}
