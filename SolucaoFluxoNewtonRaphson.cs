using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using SparseCollections;

namespace Leitura
{
    class SolucaoFluxoNewtonRaphson
    {
        public Sparse2DMatrix<int, int, double> G_matriz = new Sparse2DMatrix<int, int, double>();
        public Sparse2DMatrix<int, int, double> B_matriz = new Sparse2DMatrix<int, int, double>();
        public Sparse2DMatrix<int, int, Complex> Y_matriz = new Sparse2DMatrix<int, int, Complex>();

        public Sparse2DMatrix<int, int, Complex> V_solucao = new Sparse2DMatrix<int, int, Complex>();
        public Sparse2DMatrix<int, int, Complex> S_solucao = new Sparse2DMatrix<int, int, Complex>();

        private readonly Barra Barra;
        private int BarraSlack;
        public int iteracao = 0;

        Matrix<Complex> Jacobiano_H;
        Matrix<Complex> Jacobiano_N;
        Matrix<Complex> Jacobiano_M;
        Matrix<Complex> Jacobiano_L;

        public SolucaoFluxoNewtonRaphson(Sparse2DMatrix<int, int, double> G_mat,
                                         Sparse2DMatrix<int, int, double> B_mat,
                                         Sparse2DMatrix<int, int, Complex> Y_mat,
                                         Barra Bar)
        {
            G_matriz = G_mat;
            B_matriz = B_mat;
            Y_matriz = Y_mat;
            Barra = Bar;
        }

        public void CalculaFluxo()
        {
            // Identificando a barra de referência
            for (int k = 1; k <= Barra.NBarra.Count; k++)
            {
                if (Barra.Tipo[k] == 2)
                {
                    BarraSlack = k;
                    break;
                }
            }

            while (CalculaCondicao() || iteracao < 5)
            {
                // Calculando as matrizes da matriz Jacobiano (H, N, M e L)
                for (int k = 1; k <= Barra.NBarra.Count; k++)
                {
                    switch (Barra.Tipo[k])
                    {
                        case 0:
                            Calcula_H(iteracao, k);
                            Calcula_N(iteracao, k);
                            Calcula_M(iteracao, k);
                            Calcula_L(iteracao, k);
                            break;

                        case 1:
                            Calcula_H(iteracao, k);
                            Calcula_L(iteracao, k);
                            break;

                        case 2:
                            break;
                    }
                }
            }
        }

        private void Calcula_H(int iteracao, int k)
        {
            for(int i = 1; i <= Barra.NBarra.Count; i++)
            {
                if (i != BarraSlack)
                {
                    if (i == k)
                    {
                        Jacobiano_H[k, i] = -B_matriz[k, i] * Math.Pow(V_solucao[iteracao - 1, k].Magnitude, 2) - S_solucao[iteracao - 1, k].Imaginary;
                    }

                    else
                    {
                        Jacobiano_H[k, i] = V_solucao[iteracao - 1, k].Magnitude * V_solucao[iteracao - 1, i].Magnitude *
                            (G_matriz[i, k] * Math.Sin(Y_matriz[i, k].Phase) - B_matriz[i, k] * Math.Cos(Y_matriz[i, k].Phase));
                    }
                }
            }
        }
        private void Calcula_N(int iteracao, int k)
        {
            for (int i = 1; i <= Barra.NBarra.Count; i++)
            {
                if (i != BarraSlack)
                {
                    if (i == k)
                    {
                        Jacobiano_N[k, i] = (1 / V_solucao[iteracao - 1, k].Magnitude) * (S_solucao[iteracao - 1, k].Real
                            + Math.Pow(V_solucao[iteracao - 1, k].Magnitude, 2) * G_matriz[k ,i]);
                    }

                    else
                    {
                        Jacobiano_N[k, i] = V_solucao[iteracao - 1, k].Magnitude *
                            (G_matriz[i, k] * Math.Cos(Y_matriz[i, k].Phase) + B_matriz[i, k] * Math.Sin(Y_matriz[i, k].Phase));
                    }
                }
            }
        }
        private void Calcula_M(int iteracao, int k)
        {
            for (int i = 1; i <= Barra.NBarra.Count; i++)
            {
                if (i != BarraSlack)
                {
                    if (i == k)
                    {
                        Jacobiano_M[k, i] = S_solucao[iteracao - 1, k].Real - Math.Pow(V_solucao[iteracao - 1, k].Magnitude, 2) * G_matriz[k, i];
                    }

                    else
                    {
                        Jacobiano_M[k, i] = -V_solucao[iteracao - 1, k].Magnitude * V_solucao[iteracao - 1, i].Magnitude *
                            (G_matriz[i, k] * Math.Cos(Y_matriz[i, k].Phase) + B_matriz[i, k] * Math.Sin(Y_matriz[i, k].Phase));
                    }
                }
            }
        }
        private void Calcula_L(int iteracao, int k)
        {
            for (int i = 1; i <= Barra.NBarra.Count; i++)
            {
                if (i != BarraSlack)
                {
                    if (i == k)
                    {

                    }

                    else
                    {

                    }
                }
            }
        }
    }
}
