using System.Collections.Generic;

namespace Identity.App.ViewModel.Paged
{
    public class PagedQueryResult<T>
    {
        public int Total { get; set; }
        public int TotalNotFiltered { get; set; }
        public List<T> Rows { get; set; }
    }
}
