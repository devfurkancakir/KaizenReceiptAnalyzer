namespace KaizenReceiptAnalyzer.Models;

public class ReceiptItem
{
    public string? Locale { get; set; }
    public string Description { get; set; } = string.Empty;
    public BoundingPoly BoundingPoly { get; set; } = new BoundingPoly();
}
