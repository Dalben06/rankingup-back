namespace RankingUp.Core.ViewModels
{
    public class PaginationViewModel<T>
    {
        public IEnumerable<T> Datasource { get; set; }
        public int TotalItens { get; set; }
        public int CurrentPage { get; set; }
        public int ItensPerPage { get; set; }
        public int TotalPages
        {
            get
            {
                if (this.TotalItens > 0 && this.ItensPerPage > 0)
                {
                    if (this.TotalItens == this.ItensPerPage)
                        return 1;
                    else
                        return (this.TotalItens / this.ItensPerPage) + 1;
                }

                else
                    return 1;

            }
        }

        public PaginationViewModel()
        {
            this.CurrentPage = 1;
            this.ItensPerPage = 15;
            this.Datasource = new List<T>();
        }

    }
}
