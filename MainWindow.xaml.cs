using Projekt_PR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using static Projekt_PR.Matrix;
namespace Projekt_PR
{
    public partial class MainWindow : Window
    {
        double[,] A;
        double[,] B;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GenerateMatrices_Click(object sender, RoutedEventArgs e)
        {
            int size = (int)MatrixSizeSlider.Value;

            MatrixAInputGrid.Columns = size;
            MatrixAInputGrid.Rows = size;
            MatrixAInputGrid.Children.Clear();

            MatrixBInputGrid.Columns = size;
            MatrixBInputGrid.Rows = size;
            MatrixBInputGrid.Children.Clear();

            for (int i = 0; i < size * size; i++)
            {
                MatrixAInputGrid.Children.Add(new TextBox
                {
                    Width = 40,
                    Height = 30,
                    Margin = new Thickness(2),
                    HorizontalContentAlignment = HorizontalAlignment.Center
                });

                MatrixBInputGrid.Children.Add(new TextBox
                {
                    Width = 40,
                    Height = 30,
                    Margin = new Thickness(2),
                    HorizontalContentAlignment = HorizontalAlignment.Center
                });
            }

            ResultText.Text = $"Wprowadź dane dla macierzy {size}×{size}";        }

        private async void Async_Click(object sender, RoutedEventArgs e)
        {
            int size = (int)MatrixSizeSlider.Value;

            try
            {
                A = ReadMatrixFromGrid(MatrixAInputGrid, size);
                B = ReadMatrixFromGrid(MatrixBInputGrid, size);
            }
            catch (Exception ex)
            {
                ResultText.Text = ex.Message;
                return;
            }

            Progress.Visibility = Visibility.Visible;
            ResultText.Text = "Trwa mnożenie (async)...";

            var sw = Stopwatch.StartNew();
            var result = await Matrix.MultiplyAsync(A, B);
            sw.Stop();

            Progress.Visibility = Visibility.Collapsed;
            ResultText.Text = $"Async wynik w {sw.ElapsedMilliseconds} ms";

            ShowResultPreview(result);
        }

        private void Sync_Click(object sender, RoutedEventArgs e)
        {
            int size = (int)MatrixSizeSlider.Value;

            try
            {
                A = ReadMatrixFromGrid(MatrixAInputGrid, size);
                B = ReadMatrixFromGrid(MatrixBInputGrid, size);
            }
            catch (Exception ex)
            {
                ResultText.Text = ex.Message;
                return;
            }

            Progress.Visibility = Visibility.Visible;
            ResultText.Text = "Trwa mnożenie synchroniczne...";

            var sw = Stopwatch.StartNew();
            var result = Matrix.MultiplySync(A, B);
            sw.Stop();

            Progress.Visibility = Visibility.Collapsed;
            ResultText.Text = $"Synchroniczny wynik w {sw.ElapsedMilliseconds} ms";

            ShowResultPreview(result);
        }

        private double[,] ReadMatrixFromGrid(UniformGrid grid, int size)
        {
            double[,] result = new double[size, size];
            int index = 0;

            foreach (var child in grid.Children)
            {
                if (child is TextBox tb)
                {
                    if (double.TryParse(tb.Text.Replace(",", "."), out double value))
                    {
                        int row = index / size;
                        int col = index % size;
                        result[row, col] = value;
                        index++;
                    }
                    else
                    {
                        throw new Exception($"Nieprawidłowa wartość w polu ({index / size + 1},{index % size + 1})");
                    }
                }
            }

            return result;
        }

        private void ShowResultPreview(double[,] result)
        {
            ResultOutputGrid.Children.Clear();

            int rows = result.GetLength(0);
            int cols = result.GetLength(1);

            ResultOutputGrid.Rows = rows;
            ResultOutputGrid.Columns = cols;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    ResultOutputGrid.Children.Add(new TextBlock
                    {
                        Text = Math.Round(result[i, j], 2).ToString(),
                        FontSize = 14,
                        Width = 40,
                        Height = 30,
                        Margin = new Thickness(2),
                        TextAlignment = TextAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Background = System.Windows.Media.Brushes.White,
                        Foreground = System.Windows.Media.Brushes.Black
                    });
                }
            }

            ResultOutputGrid.Visibility = Visibility.Visible;
        }

        private void FillRandom_Click(object sender, RoutedEventArgs e)
        {
            int size = (int)MatrixSizeSlider.Value;
            var rand = new Random();

            // Wypełnij Macierz A
            int index = 0;
            foreach (var child in MatrixAInputGrid.Children)
            {
                if (child is TextBox tb)
                {
                    tb.Text = rand.Next(1, 10).ToString(); // liczby 1-9
                    index++;
                    if (index >= size * size) break;
                }
            }

            // Wypełnij Macierz B
            index = 0;
            foreach (var child in MatrixBInputGrid.Children)
            {
                if (child is TextBox tb)
                {
                    tb.Text = rand.Next(1, 10).ToString();
                    index++;
                    if (index >= size * size) break;
                }
            }
        }


        private async void ParallelMeta_Click(object sender, RoutedEventArgs e)
        {
            int size = (int)MatrixSizeSlider.Value;
            try
            {
                A = ReadMatrixFromGrid(MatrixAInputGrid, size);
                B = ReadMatrixFromGrid(MatrixBInputGrid, size);
            }
            catch (Exception ex)
            {
                ResultText.Text = ex.Message;
                return;
            }

            Progress.Visibility = Visibility.Visible;
            ResultText.Text = "Trwa mnożenie (Parallel + meta)...";

            var sw = Stopwatch.StartNew();
            var result = await Task.Run(() => Matrix.MultiplyParallelWithMeta(A, B));
            sw.Stop();

            Progress.Visibility = Visibility.Collapsed;
            ResultText.Text = $"Parallel meta wynik w {sw.ElapsedMilliseconds} ms";

            ShowResultPreviewMeta(result);


        }

        private List<System.Windows.Media.Brush> GenerateColors(int count)
        {
            var brushes = new List<System.Windows.Media.Brush>();
            var rnd = new Random(42);
            for (int i = 0; i < count; i++)
            {
                byte r = (byte)rnd.Next(60, 220);
                byte g = (byte)rnd.Next(60, 220);
                byte b = (byte)rnd.Next(60, 220);
                brushes.Add(new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(r, g, b)));
            }
            return brushes;
        }

        private void ShowResultPreviewMeta(CellMeta[,] result)
        {

        ResultOutputGrid.Children.Clear();
            int rows = result.GetLength(0);
            int cols = result.GetLength(1);

            // KROK 1: Znajduje wszystkie ThreadId
            var allThreads = new HashSet<int>();
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    allThreads.Add(result[i, j].ThreadId);

            // KROK 2: Przypisuje kolory
            var threadList = new List<int>(allThreads);
            var colors = GenerateColors(threadList.Count);
            var threadColors = new Dictionary<int, System.Windows.Media.Brush>();
            for (int i = 0; i < threadList.Count; i++)
                threadColors[threadList[i]] = colors[i];

            // KROK 3: Generuje heatmapę
            ResultOutputGrid.Rows = rows;
            ResultOutputGrid.Columns = cols;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    var meta = result[i, j];
                    var stack = new StackPanel { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Center };
                    stack.Children.Add(new TextBlock
                    {
                        Text = Math.Round(meta.Value, 2).ToString(),
                        FontSize = 14,
                        FontWeight = FontWeights.Bold,
                        TextAlignment = TextAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    });
                    stack.Children.Add(new TextBlock
                    {
                        Text = $"(Wątek: {meta.ThreadId}, {meta.ElapsedMs}ms)",
                        FontSize = 10,
                        Foreground = System.Windows.Media.Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        TextAlignment = TextAlignment.Center
                    });

                    var border = new Border
                    {
                        Child = stack,
                        Background = threadColors[meta.ThreadId],
                        BorderBrush = System.Windows.Media.Brushes.Black,
                        BorderThickness = new Thickness(1),
                        Margin = new Thickness(1)
                    };
                    ResultOutputGrid.Children.Add(border);
                }
            }

            ResultOutputGrid.Visibility = Visibility.Visible;
        }
    }
}
