using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mathematics;
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

        public SparseArray<int, double> Pcal = new SparseArray<int, double>();
        public SparseArray<int, double> Qcal = new SparseArray<int, double>();
        public SparseArray<int, double> D_PQ = new SparseArray<int, double>();
        public SparseArray<int, double> D_PQ_Solver = new SparseArray<int, double>();
        public SparseArray<int, double> D_theta_V = new SparseArray<int, double>();
        public SparseArray<int, double> D_theta = new SparseArray<int, double>();
        public SparseArray<int, double> D_V = new SparseArray<int, double>();

        private readonly Barra Barra;
        private readonly Linha Linha;
        public int iteracao = 0;

        public Sparse2DMatrix<int, int, double> Jacobiano_H = new Sparse2DMatrix<int, int, double>();
        public Sparse2DMatrix<int, int, double> Jacobiano_N = new Sparse2DMatrix<int, int, double>();
        public Sparse2DMatrix<int, int, double> Jacobiano_M = new Sparse2DMatrix<int, int, double>();
        public Sparse2DMatrix<int, int, double> Jacobiano_L = new Sparse2DMatrix<int, int, double>();
        public Sparse2DMatrix<int, int, double> Jacobiano = new Sparse2DMatrix<int, int, double>();

        public SolucaoFluxoNewtonRaphson(Sparse2DMatrix<int, int, double> G_mat,
                                         Sparse2DMatrix<int, int, double> B_mat,
                                         Sparse2DMatrix<int, int, Complex> Y_mat,
                                         Barra Bar,
                                         Linha Lin)
        {
            G_matriz = G_mat;
            B_matriz = B_mat;
            Y_matriz = Y_mat;
            Barra = Bar;
            Linha = Lin;
        }

        public void CalculaFluxo()
        {
            while (CalculaCondicao() || iteracao < 5)
            {
                // Definindo valores iniciais
                if (iteracao == 0)
                {
                    for (int k = 1; k <= Barra.NBarra.Count; k++)
                    {
                        switch(Barra.Tipo[k])
                        {
                            case 0:
                                V_solucao[iteracao, k] = 1;
                                break;

                            case 1:
                                V_solucao[iteracao, k] = Barra.Tensao[k];
                                break;

                            case 2:
                                V_solucao[iteracao, k] = Barra.Tensao[k];
                                break;
                        }
                    }

                    iteracao++;
                    continue;
                }

                for (int k = 1; k <= Barra.NBarra.Count; k++)
                {
                    Pcal[k] = G_matriz[k, k] * Math.Pow(V_solucao[iteracao - 1, k].Magnitude, 2);
                    Qcal[k] = -B_matriz[k, k] * Math.Pow(V_solucao[iteracao - 1, k].Magnitude, 2);
                }

                for (int i = 1; i <= Linha.DaBarra.Count; i++)
                {
                    int k = Linha.DaBarra[i];
                    int m = Linha.ParaBarra[i];
                    double Vk = V_solucao[iteracao - 1, k].Magnitude;
                    double Vm = V_solucao[iteracao - 1, m].Magnitude;
                    double TetaKM = (V_solucao[iteracao - 1, k].Phase - V_solucao[iteracao - 1, m].Phase + Linha.Defasagem[i]);
                    double gkm = G_matriz[k, m];
                    double bkm = B_matriz[k, m];

                    Pcal[k] += Vk * Vm * (gkm * Math.Cos(TetaKM) + bkm * Math.Sin(TetaKM));
                    Pcal[m] += Vk * Vm * (gkm * Math.Cos(TetaKM) - bkm * Math.Sin(TetaKM));
                    Qcal[k] += Vk * Vm * (gkm * Math.Sin(TetaKM) - bkm * Math.Cos(TetaKM));
                    Qcal[m] += Vk * Vm * (-gkm * Math.Sin(TetaKM) - bkm * Math.Cos(TetaKM));
                }

                // Calculando as submatrizes da matriz Jacobiano (H, N, M e L)
                for (int k = 1; k <= Barra.NBarra.Count; k++)
                {
                    double Vk = V_solucao[iteracao - 1, k].Magnitude;

                    switch (Barra.Tipo[k])
                    {
                        case 2:
                            Jacobiano_H[k, k] = 1e40;
                            Jacobiano_N[k, k] = (Pcal[k] + G_matriz[k, k] * Math.Pow(Vk, 2)) / Vk;
                            Jacobiano_M[k, k] = Pcal[k] - G_matriz[k, k] * Math.Pow(Vk, 2);
                            Jacobiano_L[k, k] = 1e40;
                            break;

                        case 0:
                            Jacobiano_H[k, k] = -Qcal[k] - B_matriz[k, k] * Math.Pow(Vk, 2);
                            Jacobiano_N[k, k] = (Pcal[k] + G_matriz[k, k] * Math.Pow(Vk, 2)) / Vk;
                            Jacobiano_M[k, k] = Pcal[k] - G_matriz[k, k] * Math.Pow(Vk, 2);
                            Jacobiano_L[k, k] = (Qcal[k] - B_matriz[k, k] * Math.Pow(Vk, 2)) / Vk;
                            break;

                        case 1:
                            Jacobiano_H[k, k] = -Qcal[k] - B_matriz[k, k] * Math.Pow(Vk, 2);
                            Jacobiano_N[k, k] = (Pcal[k] + G_matriz[k, k] * Math.Pow(Vk, 2)) / Vk;
                            Jacobiano_M[k, k] = Pcal[k] - G_matriz[k, k] * Math.Pow(Vk, 2);
                            Jacobiano_L[k, k] = 1e40;
                            break;
                    }
                }

                for (int i = 1; i <= Linha.DaBarra.Count; i++)
                {
                    int k = Linha.DaBarra[i];
                    int m = Linha.ParaBarra[i];
                    double Vk = V_solucao[iteracao - 1, k].Magnitude;
                    double Vm = V_solucao[iteracao - 1, m].Magnitude;
                    double TetaKM = V_solucao[iteracao - 1, k].Phase - V_solucao[iteracao - 1, m].Phase;

                    Jacobiano_H[k, m] = Vk * Vm * (G_matriz[k, m] * Math.Sin(TetaKM) - B_matriz[k, m] * Math.Cos(TetaKM));
                    Jacobiano_H[m, k] = -Vk * Vm * (G_matriz[k, m] * Math.Sin(TetaKM) + B_matriz[k, m] * Math.Cos(TetaKM));

                    Jacobiano_N[k, m] = Vk * (G_matriz[k, m] * Math.Cos(TetaKM) + B_matriz[k, m] * Math.Sin(TetaKM));
                    Jacobiano_N[m, k] = Vm * (G_matriz[k, m] * Math.Cos(TetaKM) - B_matriz[k, m] * Math.Sin(TetaKM));

                    Jacobiano_M[k, m] = -Vk * Vm * (G_matriz[k, m] * Math.Cos(TetaKM) + B_matriz[k, m] * Math.Sin(TetaKM));
                    Jacobiano_M[m, k] = -Vk * Vm * (G_matriz[k, m] * Math.Cos(TetaKM) - B_matriz[k, m] * Math.Sin(TetaKM));

                    Jacobiano_L[k, m] = Vk * (G_matriz[k, m] * Math.Sin(TetaKM) - B_matriz[k, m] * Math.Cos(TetaKM));
                    Jacobiano_L[m, k] = -Vm * (G_matriz[k, m] * Math.Sin(TetaKM) + B_matriz[k, m] * Math.Cos(TetaKM));
                }

                // Montando a matriz Jacobiano
                for (int j = 0; j < Barra.NBarra.Count; j++)
                {
                    for (int i = 0; i < Barra.NBarra.Count; i++)
                    {
                        int n = Barra.NBarra.Count;

                        Jacobiano[j, i] = Jacobiano_H[j + 1, i + 1];
                        Jacobiano[j + n, i] = Jacobiano_M[j + 1, i + 1];
                        Jacobiano[j, i + n] = Jacobiano_N[j + 1, i + 1];
                        Jacobiano[j + n, i + n] = Jacobiano_L[j + 1, i + 1];
                        double obst = Jacobiano.Count;
                        double obs11 = Jacobiano[1, 1];
                        double obs12 = Jacobiano[1, 3];
                        double obs21 = Jacobiano[3, 1];
                        double obs22 = Jacobiano[3, 3];
                    }
                }

                for (int i = 1; i <= Barra.NBarra.Count; i++)
                {
                    int n = Barra.NBarra.Count;

                    switch (Barra.Tipo[i])
                    {
                        case 2:
                            D_PQ[i] = 0;
                            D_PQ[i + n] = 0;
                            break;

                        case 0:
                            D_PQ[i] = (Barra.PotenciaAtivaEsperada[i] / 100) - Pcal[i];
                            D_PQ[i + n] = (Barra.PotenciaReativaEsperada[i] / 100) - Qcal[i];
                            double obsx = Barra.PotenciaReativaEsperada[i] / 100;
                            double obsy = Qcal[i];
                            double obsz = D_PQ[i + n];
                            break;

                        case 1:
                            D_PQ[i] = (Barra.PotenciaAtivaEsperada[i] / 100) - Pcal[i];
                            D_PQ[i + n] = 0;
                            break;
                    }
                }

                for (int j = 0; j < 2 * Barra.NBarra.Count; j++)
                {
                    D_PQ_Solver[j] = D_PQ[j + 1];
                }

                int obst1 = D_PQ.Count;
                double obs11111 = D_PQ_Solver[0];
                double obs22222 = D_PQ_Solver[1];
                double obs3333 = D_PQ_Solver[2];
                double obs4444 = D_PQ_Solver[3];

                LinearEquationSolver.Solve(D_PQ_Solver.Count, Jacobiano, D_PQ_Solver, D_theta_V);

                for (int i = 1; i <= Barra.NBarra.Count; i++)
                {
                    D_theta[i] = D_theta_V[i - 1];
                    D_V[i] = D_theta_V[i + Barra.NBarra.Count - 1];
                }

                for (int k = 1; k <= Barra.NBarra.Count; k++)
                {
                    V_solucao[iteracao, k] = Complex.FromPolarCoordinates(V_solucao[iteracao - 1, k].Magnitude + D_V[k], V_solucao[iteracao - 1, k].Phase + D_theta[k]);
                    double obs = V_solucao[iteracao, k].Magnitude;
                }

                iteracao++;
            }
        }

        private Boolean CalculaCondicao()
        {
            double[] erro = new double[Barra.NBarra.Count];

            for (int k = 1; k <= Barra.NBarra.Count; k++)
            {
                erro[k - 1] = Math.Abs((V_solucao[iteracao - 1, k] - V_solucao[iteracao - 2, k]).Magnitude);
            }

            if (erro.Max() >= 0.000001 && iteracao <= 3000)
                return true;
            else
                return false;
        }

        public void PreencherTabelaComplexa(DataGridView grade, Sparse2DMatrix<int, int, Complex> Matriz)
        {
            DataTable tabela = new DataTable();

            for (int i = 1; i <= Barra.NBarra.Count; i++)
            {
                tabela.Columns.Add("Barra " + Convert.ToString(i), typeof(Complex));
            }

            DataRow row = tabela.NewRow();

            for (int k = 1; k <= Barra.NBarra.Count; k++)
            {
                row[k - 1] = Matriz[iteracao - 1, k];
            }

            tabela.Rows.Add(row);


            grade.DataSource = tabela;
        }
    }
}
