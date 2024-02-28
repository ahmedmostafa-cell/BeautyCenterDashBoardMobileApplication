namespace JamalKhanah.Core.DTO.Pagination;

public class PaginationResponse
{
    public int CurrentPage { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public dynamic Items { get; set; }
}