namespace Amazon.Models;

public class UpdateQtyRequest
{
    public int ProductId { get; set; }
    public int Delta     { get; set; } // +1 або -1
}
