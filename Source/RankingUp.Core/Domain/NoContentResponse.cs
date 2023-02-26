namespace RankingUp.Core.Domain
{
    public class NoContentResponse
    {
        public Notifiable Notificacoes { get; private set; }
        public NoContentResponse(string Notificacao)
        {
            Notificacoes = new Notifiable();
            this.Notificacoes.AddNotification(Notificacao);
        }

        public NoContentResponse(Notifiable Notificacoes)
        {
            this.Notificacoes = Notificacoes;
        }

    }
}
