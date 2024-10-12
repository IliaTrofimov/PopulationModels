using CommunityToolkit.Mvvm.ComponentModel;

using MathNet.Numerics.LinearAlgebra;

using PopulationModels.UI.ViewModels;

namespace PopulationModels.UI.Models
{
    public sealed class BazykinModelA : ObservableObject, IOdeModel
    {
        private readonly ModelParameter A = new("A", 1, -5, 5);
        private readonly ModelParameter B = new("B", 1, -5, 5);
        private readonly ModelParameter E = new("E", 1, -5, 5);
        private readonly ModelParameter C = new("C", 1, -5, 5);
        private readonly ModelParameter D = new("D", 1, -5, 5);
        private readonly ModelParameter M = new("M", 1, -5, 5);
        private readonly ModelParameter p = new("p", 1, -5, 5);

        private readonly ModelParameter[] parameters;

        public IEnumerable<ModelParameter> Parameters => parameters;
        public string Name => "Базыкина (1)";
        public string Formula => "x' = Ax - Bxy / (1 + px) - Ex^2\ny' = -Cy + Dxy / (1 + px) - My^2";


        public BazykinModelA()
        {
            parameters = [A, B, E, C, D, M, p];

            A.PropertyChanged += InternalPropertyChanged;
            B.PropertyChanged += InternalPropertyChanged;
            E.PropertyChanged += InternalPropertyChanged;
            C.PropertyChanged += InternalPropertyChanged;
            D.PropertyChanged += InternalPropertyChanged;
            M.PropertyChanged += InternalPropertyChanged;
            p.PropertyChanged += InternalPropertyChanged;
        }

        public Vector<double> Derivatives(double t, Vector<double> y)
        {
            return CreateVector.DenseOfArray([
                A*y[0] - B*y[0]*y[1]/ (1 + p*y[0]) * E*Math.Pow(y[0], 2),
                -C*y[1] - D*y[0]*y[1]/ (1 + p*y[0]) * M*Math.Pow(y[1], 2)
            ]);
        }

        private void InternalPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }
    }
}
