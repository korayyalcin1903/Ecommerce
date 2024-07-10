namespace Ecommerce;

public class Pagination
{
    public int Location { get; set; }
    public double PageSize { get; set; } = 2;

    public double TotalPages(double totalCount){
        return Math.Ceiling(totalCount / PageSize);
    }

}
