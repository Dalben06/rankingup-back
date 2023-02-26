namespace RankingUp.Core.Domain
{
    public class RequestResponse<T> where T : class
    {
        public T Model { get; private set; }
        public Notifiable Notificacoes { get; private set; }

        public RequestResponse(T model, Notifiable notificacoes)
        {
            Model = model;
            Notificacoes = notificacoes;
        }

        public RequestResponse(string Notificacao)
        {
            Notificacoes = new Notifiable();
            this.Notificacoes.AddNotification(Notificacao);
        }

        public RequestResponse(Notifiable Notificacoes)
        {
            this.Notificacoes = Notificacoes;
        }

    }
}
