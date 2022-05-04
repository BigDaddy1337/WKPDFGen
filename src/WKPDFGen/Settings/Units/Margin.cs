using System.Globalization;

namespace WKPDFGen.Settings;

public enum MarginUnit
{
    Inches,
    Millimeters,
    Centimeters
}
    
public class Margin
{
    public Margin(double top, double right, double bottom, double left, MarginUnit unit = MarginUnit.Millimeters)
    {
        Top = top;
        Bottom = bottom;
        Left = left;
        Right = right;
        Unit = unit;
    }
        
    public Margin()
    {
        Unit = MarginUnit.Millimeters;
    }
        
    public MarginUnit Unit { get; set; }

    public double? Top { get; set; }

    public double? Bottom { get; set; }

    public double? Left { get; set; }

    public double? Right { get; set; }
        
    public string? GetMarginValue(double? value)
    {
        if (!value.HasValue) return null;

        var strUnit = Unit switch
        {
            MarginUnit.Inches => "in",
            MarginUnit.Millimeters => "mm",
            MarginUnit.Centimeters => "cm",
            _ => "in"
        };

        return value.Value.ToString("0.##", CultureInfo.InvariantCulture) + strUnit;
    }
}