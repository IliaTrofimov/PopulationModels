using CommunityToolkit.Mvvm.ComponentModel;

namespace PopulationModels.UI.ViewModels
{
    public abstract class BaseModelParameter : ObservableObject
    {
        protected double _value;
        protected readonly double defaultValue;
        public string Name { get; private init; } = null!;
        public virtual double Value { get => _value; set => SetProperty(ref _value, value); }

        protected BaseModelParameter(string name, double defaultValue)
        {
            Name = name;
            this.defaultValue = defaultValue;
        }

        public override string ToString() => $"{Name}: {_value}";

        public static implicit operator double(BaseModelParameter p) => p._value;
    }

}
