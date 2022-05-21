using System;

namespace NorthwindMvcClient.ViewModels
{
    public class PageViewModel
    {
        public const int BasePageSize = 5;

        public PageViewModel(int totalCount, int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }

        public bool HasPreviousPage => this.PageNumber > 1;

        public bool HasNextPage => this.PageNumber < this.TotalPages;
    }
}
