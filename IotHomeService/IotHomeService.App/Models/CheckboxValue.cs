namespace IotHomeService.App.Models
{
    public class CheckboxValue<TItem>
    {
        public CheckboxValue(TItem value, bool isChecked)
        {
            IsChecked = isChecked;
            Value = value;
        }

        public bool IsChecked { get; set; }

        public TItem Value { get; set; }
    }
}
