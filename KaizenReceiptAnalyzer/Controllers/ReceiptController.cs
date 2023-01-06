using KaizenReceiptAnalyzer.Models;
using Microsoft.AspNetCore.Mvc;

namespace KaizenReceiptAnalyzer.Controllers;

[ApiController]
[Route("[controller]")]
public class ReceiptController : ControllerBase
{
    [HttpPost]
    public ActionResult<List<string>> ParseReceipt([FromBody] List<ReceiptItem> items)
    {
        // Create a list to store the sorted descriptions
        List<ReceiptItem> sortedItems = new List<ReceiptItem>();

        for (var i = 1; i < items.Count; i++)
        {
            var item = items[i];
            // Extract the bounding polygon for the description
            var vertices = item.BoundingPoly.Vertices;

            // Get the x and y coordinates of the top-left vertex
            int x = vertices.Min(v => v.X);
            int y = vertices.Min(v => v.Y);

            // Find the insert position for the description based on the x and y coordinates
            int insertPos = sortedItems.FindIndex(d =>
            {
                var v = d.BoundingPoly.Vertices;
                return v.Max(v => v.Y) > y && v.Min(v => v.X) < x;
            });

            // Insert new item if not found
            if (insertPos < 0)
            {
                sortedItems.Insert(sortedItems.Count, item);
            }
            else
            {
                // Add description to previous one
                sortedItems[insertPos].Description += " " + item.Description;

                // Sort current and new items coordinates
                var xSortedCurrentVertices = sortedItems[insertPos].BoundingPoly.Vertices.OrderBy(v => v.X).ToList();
                var xSortedItemVertices = item.BoundingPoly.Vertices.OrderBy(v => v.X).ToList();

                // Update coordinates if min max regions changed
                xSortedCurrentVertices[0].X = Math.Min(xSortedCurrentVertices[0].X, xSortedItemVertices[0].X);

                var ySortedCurrentVertices = sortedItems[insertPos].BoundingPoly.Vertices.OrderBy(v => v.Y).ToList();
                var ySortedItemVertices = item.BoundingPoly.Vertices.OrderBy(v => v.Y).ToList();

                ySortedCurrentVertices[0].X = Math.Min(ySortedCurrentVertices[0].X, ySortedItemVertices[0].X);
            }
        }

        return sortedItems.Select(si => si.Description).ToList();
    }
}