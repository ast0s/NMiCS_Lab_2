using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;

namespace NMiCS_Lab_2
{
    public partial class MainWindow : Window
    {
        public List<string> slar = new List<string>();

        public double epsilon;
        public double[,] matrixA;
        public double[] matrixB;
        public double[,] mainMatrix;

        public double[,] transposedMatrixA;
        public double[,] normalizedMatrixA;
        public double[] normalizedMatrixB;
        public double[,] normalizedMainMatrix;

        public List<double> argumentsX = new List<double>();

        public MainWindow()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            InitializeComponent();
        }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            EquationField.Text = EquationField.Text.Replace("- ", "-").Replace("+", "").Replace("=", "");

            if (EquationField.Text != "")
                slar.Add(EquationField.Text);

            EquationField.Text = "";
        }
        private void EpsilonOKButton_Click(object sender, RoutedEventArgs e)
        {
            epsilon = double.Parse(EpsilonField.Text);
            EpsilonField.Text = "";
        }
        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            slar.Clear();
            EquationField.Text = "";
        }
        private void ClearAll_Button_Click(object sender, RoutedEventArgs e)
        {
            slar.Clear();
            argumentsX.Clear();

            matrixA = null;
            matrixB = null;
            mainMatrix = null;

            transposedMatrixA = null;
            normalizedMatrixA = null;
            normalizedMatrixB = null;
            normalizedMainMatrix = null;

            ResultField.Text = "";
            EpsilonField.Text = "";
            EquationField.Text = "";
        }
        private void Solve_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GetArgumentsX();
                GetMainMatrix();
                GetMatrixA();
                GetMatrixB();

                ZeidelMethod zm = new ZeidelMethod(matrixA, matrixB, argumentsX, epsilon);
                argumentsX = zm.DoZeidelMethod();

                if (argumentsX == null)
                {
                    argumentsX = new List<double>();
                    GetArgumentsX();
                    GetTransposedMatrixA();
                    GetNormalizedMatrixA();
                    GetNormalizedMatrixB();

                    zm = new ZeidelMethod(normalizedMatrixA, normalizedMatrixB, argumentsX, epsilon);
                    argumentsX = zm.DoZeidelMethod();

                    if (argumentsX == null)
                        ResultField.Text = "Something went wrong!";
                    else
                        for (int i = 0; i < argumentsX.Count; i++)
                            ResultField.Text += $"x{i + 1} = " + argumentsX[i] + "\n";
                }
                else
                    for (int i = 0; i < argumentsX.Count; i++)
                        ResultField.Text += $"x{i + 1} = " + argumentsX[i] + "\n";

            }
            catch (Exception exception)
            {
                _ = MessageBox.Show($"An error has occured! Message: {exception.Message}");
            }
        }

        public void GetArgumentsX()
        {
            int Xquantity = slar[0].Split(' ').Select(x => double.Parse(x)).ToArray().Length - 1;

            for (int i = 0; i < Xquantity; i++)
                argumentsX.Add(0);
        }
        public void GetMainMatrix()
        {
            double[][] temp = new double[argumentsX.Count][];

            for (int i = 0; i < argumentsX.Count; i++)
            {
                temp[i] = new double[argumentsX.Count + 1];
                temp[i] = slar[i].Split(' ').Select(x => double.Parse(x)).ToArray();
            }

            mainMatrix = new double[temp.GetLength(0), argumentsX.Count + 1];

            for (int i = 0; i < mainMatrix.GetLength(0); i++)
                for (int j = 0; j < mainMatrix.GetLength(1); j++)
                {
                    if(j != mainMatrix.GetLength(1) - 1)
                        mainMatrix[i, j] = temp[i][j];
                    else
                        mainMatrix[i, j] = temp[i][j] * -1;
                }
        }
        public void GetMatrixA()
        {
            matrixA = new double[mainMatrix.GetLength(0), mainMatrix.GetLength(1) - 1];

            for (int i = 0; i < matrixA.GetLength(0); i++)
                for (int j = 0; j < matrixA.GetLength(1); j++)
                    matrixA[i, j] = mainMatrix[i, j];
        }
        public void GetMatrixB()
        {
            matrixB = new double[slar.Count];

            for (int i = 0; i < matrixB.Length; i++)
                matrixB[i] = mainMatrix[i, mainMatrix.GetLength(1) - 1] * -1;
        }

        public void GetTransposedMatrixA()
        {
            transposedMatrixA = new double[matrixA.GetLength(1), matrixA.GetLength(0)];

            for (int i = 0; i < transposedMatrixA.GetLength(0); i++)
                for (int j = 0; j < transposedMatrixA.GetLength(1); j++)
                    transposedMatrixA[i, j] = matrixA[j, i];
        }
        public void GetNormalizedMatrixA()
        {
            normalizedMatrixA = new double[matrixA.GetLength(0), matrixA.GetLength(1)];

            for (int i = 0; i < normalizedMatrixA.GetLength(0); i++)
                for (int j = 0; j < normalizedMatrixA.GetLength(1); j++)
                    for (int k = 0; k < normalizedMatrixA.GetLength(1); k++)
                        normalizedMatrixA[i, j] += transposedMatrixA[i, k] * matrixA[k, j];
        }
        public void GetNormalizedMatrixB()
        {
            normalizedMatrixB = new double[matrixB.Length];

            for (int i = 0; i < normalizedMatrixB.Length; i++)
                for (int j = 0; j < argumentsX.Count; j++)
                    normalizedMatrixB[i] += transposedMatrixA[i, j] * matrixB[j];
        }
    }
}
