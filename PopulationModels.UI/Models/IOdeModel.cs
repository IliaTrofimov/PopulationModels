using System.ComponentModel;

using MathNet.Numerics.LinearAlgebra;

using PopulationModels.UI.ViewModels;

namespace PopulationModels.UI.Models
{
    public interface IOdeModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public IEnumerable<ModelParameter> Parameters { get; }
        public string Name { get; }
        public string Formula { get; }
        public Vector<double> Derivatives(double t, Vector<double> y);
    }
}
