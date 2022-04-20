using System;
using System.Collections.Generic;

namespace NMiCS_Lab_2
{
    public class ZeidelMethod
    {
        double[,] matrixA;
        double[] matrixB;
        List<double> currentArguments;
        List<double> previousArguments = new List<double>();
        double epsilon;

        public ZeidelMethod(double[,] normalizedMatrixA, double[] normalizedMatrixB, List<double> argumentsX, double epsilon)
        {
            matrixA = normalizedMatrixA;
            matrixB = normalizedMatrixB;
            currentArguments = argumentsX;
            this.epsilon = epsilon;

            previousArguments.AddRange(currentArguments);
        }

        public List<double> DoZeidelMethod()
        {
            for (int i = 0; i < currentArguments.Count; i++)
                FindX(i);

            for (int i = 0; i < currentArguments.Count; i++)
            {
                if (Math.Abs(previousArguments[i] - currentArguments[i]) > epsilon)
                {
                    previousArguments.Clear();
                    previousArguments.AddRange(currentArguments);
                    return DoZeidelMethod();
                }
            }

            return currentArguments;
        }
        public void FindX(int iteration)
        {
            double temp = 0;
            for (int i = 0; i < currentArguments.Count; i++)
                if (i != iteration)
                    temp += matrixA[iteration, i] * currentArguments[i];

            currentArguments[iteration] = (matrixB[iteration] - temp) / matrixA[iteration, iteration];
        }
    }
}