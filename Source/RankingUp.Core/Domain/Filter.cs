namespace RankingUp.Core.Domain
{
    public class Filter
    {
        public int ItensPerPage { get; set; }
        public int CurrentPage { get; set; }
        public bool Ignore { get; set; }
        public string Order { get; set; }
        public string OrderType { get; set; }
        public Filter() 
        { 
            this.CurrentPage = 1;
            this.ItensPerPage = 15;
            this.Ignore = false;
        }

    }
}
