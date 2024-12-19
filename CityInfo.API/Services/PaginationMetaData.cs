namespace CityInfo.API.Services;

public class PaginationMetaData
{
    public int TotalItemsCount { get; set; }
    public int PageSize { get; set; }
    public int TotalPagesCount { get; set; }
    public int CurrentPage { get; set; }

    public PaginationMetaData(int totalItemsCount , int pageSize ,int currentPage) 
    {
        TotalItemsCount = totalItemsCount;
        PageSize = pageSize;
        CurrentPage = currentPage;
        TotalPagesCount = (int)Math.Ceiling(totalItemsCount / (double)pageSize);
    }
}
