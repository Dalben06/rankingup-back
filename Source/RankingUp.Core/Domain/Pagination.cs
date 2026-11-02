namespace RankingUp.Core.Domain
{
    public class Pagination<T>
    {
        public ICollection<T> Datasource { get; set; }
        public int TotalItens { get; set; }
        public int CurrentPage { get; set; }
        public int ItensPerPage { get; set; }
        public int TotalPages
        {
            get => this.TotalItens > 0 && this.ItensPerPage > 0
                ? (int)Math.Ceiling((double)this.TotalItens / this.ItensPerPage)
                : 0;
        }


        public Pagination()
        {
            this.CurrentPage = 1;
            this.ItensPerPage = 15;
            this.Datasource = new List<T>();
        }
    }
}
