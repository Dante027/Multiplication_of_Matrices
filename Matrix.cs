using System;

namespace Projekt_PR
{
    public static class Matrix
    {
        public static double[,] GenerateRandom(int rows, int cols)
        {
            var rand = new Random();
            var data = new double[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    data[i, j] = rand.NextDouble() * 10;
            return data;
        }

        public static double[,] MultiplySync(double[,] A, double[,] B)
        {
            int rows = A.GetLength(0); // liczba wierszy macierzy A
            int cols = B.GetLength(1); // liczba kolumn macierzy B
            int common = A.GetLength(1); // wspólny wymiar (kolumny A = wiersze B)
            var result = new double[rows, cols]; // inicjalizacja macierzy wynikowej

            for (int i = 0; i < rows; i++) // dla każdego wiersza A
                for (int j = 0; j < cols; j++) // dla każdej kolumny B
                {
                    double sum = 0;            // przemnażanie elementów wiersza A i kolumny B
                    for (int k = 0; k < common; k++)
                        sum += A[i, k] * B[k, j];

                    System.Threading.Thread.Sleep(100);


                    result[i, j] = sum; // zapisanie wyniku do komórki
                }

            return result;
        }

        public static async Task<double[,]> MultiplyAsync(double[,] A, double[,] B)
        {
            return await Task.Run(() => MultiplySync(A, B));
        }

        public static double[,] MultiplyParallel(double[,] A, double[,] B)
        {
            int rows = A.GetLength(0); // pierwszy wymiar tabeli
            int cols = B.GetLength(1); // drugi wymiar tabeli 
            int common = A.GetLength(1);
            var result = new double[rows, cols];

            Parallel.For(0, rows, i =>             // Parallel.For umożliwia równoległe przetwarzanie wierszy(na wielu wątkach.
            {                                      // każdy wiersz i osobno i równolegle
                for (int j = 0; j < cols; j++)     // każda kolumna j 
                {                                  // przemnażanie i dodawanie
                    double sum = 0;
                    for (int k = 0; k < common; k++)
                        sum += A[i, k] * B[k, j];
                    result[i, j] = sum;
                }
            });

            return result;
        }

        public static async Task<double[,]> MultiplyAsyncParallel(double[,] A, double[,] B)
        {
            return await Task.Run(() => MultiplyParallel(A, B)); // uruchamia mnożenie async w tle
        }

        public class CellMeta
        {
            public double Value { get; set; }
            public int ThreadId { get; set; }
            public long ElapsedMs { get; set; }
        }

        public static CellMeta[,] MultiplyParallelWithMeta(double[,] A, double[,] B)
        {
            int rows = A.GetLength(0);
            int cols = B.GetLength(1);
            int common = A.GetLength(1);
            var result = new CellMeta[rows, cols];

            Parallel.For(0, rows, i =>
            {
                for (int j = 0; j < cols; j++)
                {
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    double sum = 0;
                    for (int k = 0; k < common; k++)
                        sum += A[i, k] * B[k, j];

                    // Wymusza "opóźnienie" na każdym elemencie
                    System.Threading.Thread.Sleep(100);

                    sw.Stop();
                    result[i, j] = new CellMeta
                    {
                        Value = sum,
                        ThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId,
                        ElapsedMs = sw.ElapsedMilliseconds
                    };
                }
            });

            return result;
        }
    }
}

