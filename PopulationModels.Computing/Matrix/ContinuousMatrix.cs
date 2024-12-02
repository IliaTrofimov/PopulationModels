using System.Buffers;
using System.Diagnostics;
using System.Text;


namespace PopulationModels.Computing.Matrix;

public partial struct ContinuousMatrix : IDisposable, IEquatable<ContinuousMatrix>
{
    public const int MIN_ELEMENTS_FOR_ARRAY_POOL = 2000;
    
    private readonly double[][] data;

    
    public bool UsesArrayPool { get; private init; } = false;

    public bool IsEmpty { get; } = true;

    public int Rows { get; private init; } = 0;
   
    public int Columns { get; private init; } = 0;
    
    public int ItemsCount => Rows * Columns;
    
    public double this[int row, int column]
    {
        get
        {
            ValidateIndex(row, column);
            return data[row][column];
        }
        set
        {
            ValidateIndex(row, column);
            data[row][column] = value;
        }
    }

    public double[] this[int row]
    {
        get
        {
            ValidateIndex(row);
            return data[row];
        }
    }
    
    public ContinuousMatrix(int rows, int columns, bool? useArrayPool = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rows, nameof(rows));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(columns, nameof(columns));
        
        Rows = rows;
        Columns = columns;
        IsEmpty = false;
        data = new double[Rows][];
        UsesArrayPool = useArrayPool ?? columns >= MIN_ELEMENTS_FOR_ARRAY_POOL;
        
        if (UsesArrayPool)
        {
            UsesArrayPool = true;
            for (var i = 0; i < Rows; i++)
                data[i] = ArrayPool<double>.Shared.Rent(Columns);
        }
        else
        {
            UsesArrayPool = false;
            for (var i = 0; i < Rows; i++)
                data[i] = new double[Columns];
        }
    }
    
    public ContinuousMatrix(double[][] data)
    {
        ArgumentNullException.ThrowIfNull(data, nameof(data));
        if (data.Length == 0)
            throw new ArgumentException("Matrix rows must be greater than 0.");
        if (data[0].Length == 0)
            throw new ArgumentException("Matrix columns must be greater than 0.");
        
        this.data = data ?? throw new ArgumentNullException(nameof(data));
        Rows = this.data.Length;
        Columns = this.data[0].Length;
        IsEmpty = false;
    }


    public static implicit operator double[][](ContinuousMatrix matrix) => matrix.data;
    
    public static explicit operator ContinuousMatrix(double[][] data) => new(data);

    
    public void SetColumn(int column, IList<double> columnData)
    {
        var length = Math.Min(Rows, columnData.Count);
        for (var i = 0; i < length; i++)
            this[i, column] = columnData[i];
    }
    
    public void SetRow(int row, IList<double> rowData)
    {
        var length = Math.Min(Columns, rowData.Count);
        for (var i = 0; i < length; i++)
            this[row, i] = rowData[i];
    }    
    
    public MatrixSlice GetColumn(int column) => new MatrixColumn(data, column);
    
    public MatrixSlice GetRow(int row) => new MatrixRow(data, row);

    public ContinuousMatrix Transpose()
    {
        if (Rows == 0 || Columns == 0)
            return new ContinuousMatrix();
        
        var transposed = new double[Columns][];
        if (UsesArrayPool)
        {
            for (var i = 0; i < Columns; i++)
                data[i] = ArrayPool<double>.Shared.Rent(Rows);
        }
        else
        {
            for (var i = 0; i < Columns; i++)
                data[i] = new double[Rows];
        }
        
        return new ContinuousMatrix(transposed) { UsesArrayPool = UsesArrayPool, Rows = Columns, Columns = Rows };
    }
    
    
    public override string ToString() => ToString(10);
    
    public string ToString(int maxDisplayElements)
    {
        #if DEBUG
        
        var sb = new StringBuilder(ItemsCount * 5 + 15);
        sb.Append($"Matrix_{GetHashCode():x}{(UsesArrayPool ? "_pooled" : "")}[{Rows}, {Columns}]:\n");   
        for (var i = 0; i < Rows && i < maxDisplayElements; i++)
        {
            sb.Append("| ");
            for (var j = 0; j < Columns - 1 && j < maxDisplayElements; j++)
                sb.AppendFormat("{0,8:F3} ", this[i][j]);

            if (Columns <= maxDisplayElements)
                sb.AppendFormat("{0,8:F3} |\n", this[i][Columns - 1]);
            else
                sb.AppendFormat("... {0,8:F3} |\n", this[i][Columns - 1]);
        }
        if (Rows >= maxDisplayElements)
            sb.Append(" ... \n");

        return sb.ToString();

        #else
        
        return $"Matrix_{GetHashCode():x}{(UsesArrayPool ? "_pooled" : "")}[{Rows}, {Columns}]:\n";
        
        #endif
    }
    
    
    public void Dispose()
    {
        if (!UsesArrayPool || Rows == 0 || Columns == 0) return;
        for (var i = 0; i < Rows; i++)
            ArrayPool<double>.Shared.Return(data[i], clearArray: true);
    }

    public void Clear()
    {
        if (Columns == 0) return;
        for (var i = 0; i < Rows; i++)
            Array.Clear(data[i]);
    }


    public override bool Equals(object? obj)
    {
        if (obj is double[][] dataArray && dataArray == data) return true;
        if (obj is ContinuousMatrix matrix) return Equals(matrix);
        return false;
    }

    public bool Equals(ContinuousMatrix other)
    {
        if (other.Columns != Columns || other.Rows != Rows) return false;
        for (var i = 0; i < Rows; i++)
        for (var j = 0; j < Columns; j++)
            if (Math.Abs(data[i][j] - other[i][j]) <= double.Epsilon * 2)
                return false;
        return true;
    }

    public override int GetHashCode() => HashCode.Combine(data, UsesArrayPool, Rows, Columns);


    private void ValidateIndex(int row)
    {
        if (row < 0 || row >= Rows)
            throw new ArgumentOutOfRangeException(nameof(row), $"Row index must be in range [0, Rows({Rows - 1})], actual index {row}.");
    }
    
    private void ValidateIndex(int row, int column)
    {
        ValidateIndex(row);
        if (column < 0 || column >= Columns)
            throw new ArgumentOutOfRangeException(nameof(row), $"Column index must be in range [0, Columns({Columns - 1})], actual index {column}.");
    }

    public static bool operator ==(ContinuousMatrix left, ContinuousMatrix right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ContinuousMatrix left, ContinuousMatrix right)
    {
        return !(left == right);
    }
}