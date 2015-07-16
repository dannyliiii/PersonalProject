using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DTW
{
    struct Vector2 {
        public float x;
        public float y;
        public Vector2(float a, float b) {
            x = a;
            y = b;
        }
    };

    class Program
    {

        static public float DistanceBetween2Vectors(Vector2 a, Vector2 b)
        {
            return (float)(Math.Pow((a.x - b.x), 2) + Math.Pow((a.y - b.y), 2));
        }

        static float[,] GetDistanceMatrix(Vector2[] time_series_A, Vector2[] time_series_B, int lengthA, int lengthB) {

            float[,] dMatrix = new float[lengthB, lengthA];

            for (int i = 0; i < lengthB; i++)
            {
                for (int j = 0; j < lengthA; j++)
                {
                    dMatrix[i, j] = DistanceBetween2Vectors(time_series_A[j], time_series_B[i]);
                }
            }

            for (int i = 0; i < lengthB; i++)
            {
                for (int j = 0; j < lengthA; j++)
                {
                    Debug.Write(dMatrix[i, j]);
                    Debug.Write(" ");
                }
                Debug.Write("\n");
            }
            return dMatrix;
        }
        static float[,] GetDTWMatrix(float[,] dMatrix, int lengthA, int lengthB) {

            float[,] rMatrix = new float[lengthB, lengthA];

            rMatrix[0, 0] = dMatrix[0, 0];
            for (int i = 1; i < lengthB; i++)
            {
                rMatrix[i, 0] = rMatrix[i - 1, 0] + dMatrix[i, 0];
            }
            for (int i = 1; i < lengthA; i++)
            {
                rMatrix[0, i] = rMatrix[0, i - 1] + dMatrix[0, i];
            }

            for (int i = 1; i < lengthB; i++)
            {
                for (int j = 1; j < lengthA; j++)
                {

                    rMatrix[i, j] = dMatrix[i, j] + Math.Min(Math.Min(rMatrix[i - 1, j - 1], rMatrix[i, j - 1]), rMatrix[i - 1, j]);

                }
            }

            Debug.WriteLine("===========");
            for (int i = 0; i < lengthB; i++)
            {
                for (int j = 0; j < lengthA; j++)
                {
                    Debug.Write(rMatrix[i, j]);
                    Debug.Write(" ");
                }
                Debug.Write("\n");
            }

            return rMatrix;
        }

        static List<Vector2> GetOptimalPath(float[,] dtwMatrix, int lengthA, int lengthB) {
 
            List<Vector2> res = new List<Vector2>();

            int row = lengthB - 1;
            int col = lengthA - 1;
            while (row > 0 && col > 0)
            {
                if (dtwMatrix[row -1 , col] == Math.Min(Math.Min(dtwMatrix[row,col-1], dtwMatrix[row -1, col]), dtwMatrix[row -1, col -1])) {
                    row -= 1;
                }
                else if (dtwMatrix[row, col - 1] == Math.Min(Math.Min(dtwMatrix[row, col - 1], dtwMatrix[row - 1, col]), dtwMatrix[row - 1, col - 1]))
                {
                    col -= 1;
                }
                else
                {
                    row -= 1;
                    col -= 1;
                }
                res.Add(new Vector2(row, col));
            }

            return res;
        }


        static void Main(string[] args)
        {
            Vector2[] time_series_A = { new Vector2(-0.87f,-0.88f), new Vector2(-0.84f,-0.91f), new Vector2(-0.85f,-0.84f), new Vector2(-0.82f,-0.82f), new Vector2(-0.23f,-0.24f), 
                                        new Vector2(1.95f,1.92f), new Vector2(1.36f,1.41f), new Vector2(0.60f,0.51f), new Vector2(0.0f,0.03f), new Vector2(-0.29f,-0.18f)};
            Vector2[] time_series_B = { new Vector2(-0.60f,-0.46f), new Vector2(-0.65f,-0.62f), new Vector2(-0.71f,-0.68f), new Vector2(-0.58f,-0.63f), new Vector2(-0.17f,-0.32f), 
                                        new Vector2(0.77f,0.74f), new Vector2(1.94f,1.97f)};
            int lengthA = time_series_A.Length;
            int lengthB = time_series_B.Length;

            float[,] dMatrix = GetDistanceMatrix(time_series_A, time_series_B, lengthA, lengthB);

            float[,] rMatrix = GetDTWMatrix(dMatrix, lengthA, lengthB);

            List<Vector2> path = GetOptimalPath(rMatrix, lengthA, lengthB);
            
            foreach (var p in path) {
                Debug.WriteLine(p.x.ToString() + " " + p.y.ToString());
            }
        }
    }
}
