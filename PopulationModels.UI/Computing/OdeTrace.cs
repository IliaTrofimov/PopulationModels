namespace PopulationModels.UI.Computing
{
    internal readonly struct OdeTrace(int n)
    {
        public readonly double[] XValues = new double[n];
        public readonly double[] YValues = new double[n];

        public bool HasValue => XValues?.Length > 0;
        public double X0 => XValues[0];
        public double Y0 => YValues[0];
        public double Xn => XValues[XValues.Length - 1];
        public double Yn => YValues[XValues.Length - 1];

        public void SetValue(int i, double x, double y)
        {
            XValues[i] = x;
            YValues[i] = y;
        }
    }
}
