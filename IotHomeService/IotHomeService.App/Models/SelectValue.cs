namespace IotHomeService.App.Models
{
    public class SelectValue<TItem>
    {
        public SelectValue(TItem value, string text)
        {
            Text = text;
            Value = value;
        }

        public string Text { get; set; }

        public TItem Value { get; set; }
    }
}
