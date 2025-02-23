namespace BLAExercise.Data.Models;

public class GetQueryParameters
{
    public GetQueryParameters(int page, int pageSize, string sortBy, bool descending)
    {
        Page = page;
        PageSize = pageSize;
        SortBy = sortBy;
        Descending = descending;
    }

    public int Page { get; set; }
    public int PageSize { get; set; }
    public string SortBy { get; set; }
    public bool Descending { get; set; }
}
