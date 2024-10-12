using System.Runtime.CompilerServices;

using CommunityToolkit.Mvvm.ComponentModel;

using MathNet.Numerics.LinearAlgebra;

using PopulationModels.UI.ViewModels;

namespace PopulationModels.UI.Models
{
    public sealed class LotkaVolterraModel : ObservableObject, IOdeModel
    {
        private readonly ModelParameter alpha = new("α", 1, -5, 5);
        private readonly ModelParameter epsilon = new("beta", 1, -5, 5);
        private readonly ModelParameter gamma = new("γ", 1, -5, 5);
        private readonly ModelParameter mu = new("delta", 1, -5, 5);

        private readonly ModelParameter[] parameters;

        public IEnumerable<ModelParameter> Parameters => parameters;
        public string Name => "Лотки-Вольтерра";
        public string Formula => "x' = (α - beta*y)x\ny' = (-γ + delta*x)y";


        public LotkaVolterraModel()
        {
            parameters = [alpha, epsilon, gamma, mu];

            alpha.PropertyChanged += InternalPropertyChanged;
            epsilon.PropertyChanged += InternalPropertyChanged;
            gamma.PropertyChanged += InternalPropertyChanged;
            mu.PropertyChanged += InternalPropertyChanged;
        }

        public Vector<double> Derivatives(double t, Vector<double> y)
        {
            return CreateVector.DenseOfArray([
                (alpha - epsilon * y[1]) * y[0],
                (-gamma + mu * y[0]) * y[1]
            ]);
        }

        private void InternalPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }
    }
}
