using PopulationModels.Computing.Matrix;


namespace PopulationModels.UnitTests;

public static class Helpers
{
    public static void AssertEqual<T>(T expected, ContinuousMatrix actual, string message, double eps = 1e-6)
    {
        if (expected is IList<double>[] array)
        {
            AssertEqual(array.Length, actual.Rows, 
                $"[{message}] expected.Rows != actual.Rows ({array.Length} != {actual.Rows})");

            for (var i = 0; i < array.Length; i++)  // Columns
            {
                AssertEqual(array[i].Count, actual.Columns, 
                    $"[{message}] expected[{i}].Columns != actual.Columns ({array[i].Count} != {actual.Columns})");
                
                for (var j = 0; j < array[i].Count; j++)   // Rows
                {
                    AssertEqual(array[i][j], actual[i, j], eps,
                        $"[{message}] expected[{i},{j}] != actual[{i},{j}] ({array[i][j]:F7} != {actual[i, j]:F7}, EPS={eps:e1})");
                }
            }
        }
        else if (expected is ContinuousMatrix matrix)
        {
            AssertEqual(matrix.Rows, actual.Rows, 
                $"[{message}] expected.Rows != actual.Rows ({matrix.Rows} != {actual.Rows})");
            AssertEqual(matrix.Columns, actual.Columns, 
                $"[{message}] expected.Columns != actual.Columns ({matrix.Columns} != {actual.Columns})");

            for (var i = 0; i < actual.Rows; i++) 
            {
                for (var j = 0; j < actual.Columns; j++)
                {
                    AssertEqual(matrix[i, j], actual[i, j], eps,
                        $"[{message}] expected[{i},{j}] != actual[{i},{j}] ({matrix[i, j]:F7} != {actual[i, j]:F7}, EPS={eps:e1})");
                }
            }
        }
        else Assert.Fail($"Cannot compare expected object of type {typeof(T)} to {nameof(ContinuousMatrix)}.");
    }
    
    public static void AssertEqualTransposed<T>(T expected, ContinuousMatrix actual, string message, double eps = 1e-6)
    {
        if (expected is IList<double>[] array)
        {
            AssertEqual(array.Length, actual.Columns, 
                $"[TRANSPOSED {message}] expected.Rows != actual.Columns ({array.Length} != {actual.Columns})");

            for (var i = 0; i < array.Length; i++)  // Columns
            {
                AssertEqual(array[i].Count, actual.Rows, 
                    $"[TRANSPOSED {message}] expected[{i}].Columns != actual.Rows ({array[i].Count} != {actual.Rows})");
                
                for (var j = 0; j < array[i].Count; j++)   // Rows
                {
                    AssertEqual(array[i][j], actual[j, i], eps,
                        $"[TRANSPOSED {message}] expected[{i},{j}] != actual[{j},{i}] ({array[i][j]:F7} != {actual[j, i]:F7}, EPS={eps:e1})");
                }
            }
        }
        else if (expected is ContinuousMatrix matrix)
        {
            AssertEqual(matrix.Rows, actual.Columns, 
                $"[TRANSPOSED {message}] expected.Rows != actual.Columns ({matrix.Rows} != {actual.Columns})");
            AssertEqual(matrix.Columns, actual.Rows, 
                $"[TRANSPOSED {message}] expected.Columns != actual.Rows ({matrix.Columns} != {actual.Rows})");

            for (var i = 0; i < actual.Rows; i++) 
            {
                for (var j = 0; j < actual.Columns; j++)
                {
                    AssertEqual(matrix[i, j], actual[j, i], eps,
                        $"[TRANSPOSED {message}] expected[{i},{j}] != actual[{j},{i}] ({matrix[i, j]:F7} != {actual[j, i]:F7}, EPS={eps:e1})");
                }
            }
        }
        else Assert.Fail($"Cannot compare expected object of type {typeof(T)} to {nameof(ContinuousMatrix)}.");
    }
    
    public static void AssertEqual<T>(T expected, T actual, string message)
    {
        Assert.True(expected?.Equals(actual) == true, message);
    }
    
    public static void AssertEqual(double expected, double actual, double eps, string message)
    {
        Assert.True(Math.Abs(expected - actual) <= eps, message);
    }
}