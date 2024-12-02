namespace PopulationModels.Computing.Matrix;

public partial struct ContinuousMatrix
{
    private sealed class MatrixRow : MatrixSlice
    {
        private readonly double[][] data;
    
        internal MatrixRow(double[][] data, int row)
        {
            this.data = data;
            Row = row % data.Length;
        }

        public override double this[int index]
        {
            get => data[Row][index];
            set => data[Row][index] = value;
        }
        
        public int Row { get; private set; }
        
        public override int Index => Row;

        public override int Length => data.GetLength(1);

        public override void Next()
        {
            if (Row == data.Length - 1)
                throw new InvalidOperationException($"Cannot move row to next position. Current columns index {Row}, max row index {data.Length - 1}.");
            Row++;
        }

        public override void NextCycle()
        {
            if (Row >= data.Length - 1) Row = 0;
            else Row++;
        }
        
        public override double[] AsArray() => data[Row];

        public override string ToString()
        {
            return Length <= 10 
                ? $"Row_{Row}[{Length}] <{string.Join("; ", Enumerable.Range(0, Length).Select(i => $"{this[i]:F3}"))}>" 
                : $"Row_{Row}[{Length}] <{string.Join("; ", Enumerable.Range(0, 10).Select(i => $"{this[i]:F3}"))}; ... {this[Length - 1]:F3}>";
        }
    }

}