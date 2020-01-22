using Blazorise.Charts;

namespace IotHomeService.App.Models
{
    public class MyChartColor
    {
        public MyChartColor(string name, byte red, byte green, byte blue)
        {
            Name = name;
            Fill = ChartColor.FromRgba(red, green, blue, 0.2f);
            Border = ChartColor.FromRgba(red, green, blue, 1f);
        }

        public string Name { get; }

        public ChartColor Border { get; }

        public ChartColor Fill { get; }
    }
}
