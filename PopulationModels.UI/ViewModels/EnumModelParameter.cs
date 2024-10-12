namespace PopulationModels.UI.ViewModels
{
    public class EnumModelParameter : BaseModelParameter
    {
        private readonly double[] values;
        private int selectedIndex = 0;

        public IEnumerable<double> Values => values;

        public override double Value
        {
            get => _value;
            set => throw new NotImplementedException($"Cannot set value of {nameof(EnumModelParameter)} directly. Use {nameof(SelectedIndex)} property.");
        }

        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                if (value < 0 || value >= values.Length)
                    throw new IndexOutOfRangeException(nameof(value));
                
                SetProperty(ref selectedIndex, value);
                
                OnPropertyChanging(nameof(Value));
                _value = values[value];
                OnPropertyChanged(nameof(Value));
            }
        }


        public EnumModelParameter(string name, params double[] avaliableValues)
            : base(name, avaliableValues[0])
        {
            if (avaliableValues == null || avaliableValues.Length == 0)
                throw new ArgumentNullException(nameof(avaliableValues));
    
            values = avaliableValues;
            _value = avaliableValues[0];
        }
    }

}
