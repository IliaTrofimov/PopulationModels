namespace PopulationModels.Computing.Matrix;

public partial struct ContinuousMatrix 
{
    private sealed class MatrixColumn : MatrixSlice
    {
        private readonly double[][] data;
    
        internal MatrixColumn(double[][] data, int column)
        {
            this.data = data;
            Column = column % data[0].Length;
        }

        public override double this[int index]
        {
            get => data[index][Column];
            set => data[index][Column] = value;
        }
     
        public int Column { get; private set; }
        
        public override int Index => Column;
        
        public override int Length => data.GetLength(0);

        public override void Next()
        {
            if (Column == data[0].Length - 1)
                throw new InvalidOperationException($"Cannot move column to next position. Current columns index {Column}, max column index {data[0].Length - 1}.");
            Column++;
        }
        
        public override void NextCycle()
        {
            if (Column >= data[0].Length - 1) Column = 0;
            else Column++;
        }

        
        public override string ToString()
        {
            return Length <= 10
                ? $"Column_{Column}[{Length}] <{string.Join("; ", Enumerable.Range(0, Length).Select(i => $"{this[i]:F3}"))}>"
                : $"Column_{Column}[{Length}] <{string.Join("; ", Enumerable.Range(0, 10).Select(i => $"{this[i]:F3}"))}; ... {this[Length - 1]:F3}>";
        }
    }
}