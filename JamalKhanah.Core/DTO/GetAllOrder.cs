namespace JamalKhanah.Core.DTO;

public class GetAllOrder
{
    public bool InHome { get; set; }
    
    public DateTime? Start { get; set; } = null;

    public DateTime? End { get; set; } = null;

    public int? OrderStatus { get; set; } = null; //  , 1, 2, 3, 4, 5
}