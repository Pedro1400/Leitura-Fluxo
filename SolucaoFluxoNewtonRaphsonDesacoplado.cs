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
    class SolucaoFluxoNewtonRaphsonDesacoplado
    {
        public Sparse2DMatrix<int, int, double> G_matriz = new Sparse2DMatrix<int, int, double>();
        public Sparse2DMatrix<int, int, double> B_matriz = new Sparse2DMatrix<int, int, double>();

        public SparseArray<int, Complex> V_solucao = new SparseArray<int, Complex>();

        public SparseArray<int, double> Pcal = new SparseArray<int, double>();
        public SparseArray<int, double> Qcal = new SparseArray<int, double>();
        public SparseArray<int, double> D_P = new SparseArray<int, double>();
        public SparseArray<int, double> D_Q = new SparseArray<int, double>();
        public SparseArray<int, double> D_P_Solver = new SparseArray<int, double>();
        public SparseArray<int, double> D_Q_Solver = new SparseArray<int, double>();
        public SparseArray<int, double> D_theta = new SparseArray<int, double>();
        public SparseArray<int, double> D_V = new SparseArray<int, double>();

        private readonly Barra Barra;
        private readonly Linha Linha;

        public int iteracaoP;
        public int iteracaoQ;

        private int KP;
        private int KQ;

        public Sparse2DMatrix<int, int, double> Jacobiano_H = new Sparse2DMatrix<int, int, double>();
        public Sparse2DMatrix<int, int, double> Jacobiano_L = new Sparse2DMatrix<int, int, double>();

        public SolucaoFluxoNewtonRaphsonDesacoplado(Sparse2DMatrix<int, int, double> G_mat,
                                         Sparse2DMatrix<int, int, double> B_mat,
                                         Barra Bar,
                                         Linha Lin)
        {
            G_matriz = G_mat;
            B_matriz = B_mat;
            Barra = Bar;
            Linha = Lin;
        }

        public void CalculaFluxo()
        {

            for (int k = 1; k <= Barra.NBarra.Count; k++)
            {
                switch (Barra.Tipo[k])
                {
                    case 0:
                        V_solucao[k] = 1;
                        break;

                    case 1:
                        V_solucao[k] = Barra.Tensao[k];
                        break;

                    case 2:
                        V_solucao[k] = Barra.Tensao[k];
                        break;
                }
            }

            iteracaoP = 0;
            iteracaoQ = 0;

            KP = 1;
            KQ = 1;

            while (true)
            {

                // Meia iteração P
                for (int k = 1; k <= Barra.NBarra.Count; k++)
                {
                    Pcal[k] = G_matriz[k, k] * Math.Pow(V_solucao[k].Magnitude, 2);
                }

                for (int i = 1; i <= Linha.DaBarra.Count; i++)
                {
                    int k = Linha.DaBarra[i];
                    int m = Linha.ParaBarra[i];
                    double Vk = V_solucao[k].Magnitude;
                    double Vm = V_solucao[m].Magnitude;
                    double TetaKM = (V_solucao[k].Phase - V_solucao[m].Phase + Linha.Defasagem[i]);
                    double gkm = G_matriz[k, m];
                    double bkm = B_matriz[k, m];

                    Pcal[k] += Vk * Vm * (gkm * Math.Cos(TetaKM) + bkm * Math.Sin(TetaKM));
                    Pcal[m] += Vk * Vm * (gkm * Math.Cos(TetaKM) - bkm * Math.Sin(TetaKM));
                }

                double obsPcal = Pcal[1];
                double obsPca2 = Pcal[2];
                double obsPca3 = Pcal[3];
                double obsPca4 = Pcal[4];
                double obsPca5 = Pcal[5];

                for (int i = 1; i <= Barra.NBarra.Count; i++)
                {

                    switch (Barra.Tipo[i])
                    {
                        case 2:
                            D_P[i] = 0;
                            break;

                        case 0:
                            D_P[i] = (Barra.PotenciaAtivaEsperada[i] / 100) - Pcal[i];
                            break;

                        case 1:
                            D_P[i] = (Barra.PotenciaAtivaEsperada[i] / 100) - Pcal[i];
                            break;
                    }
                }

                for (int k = 0; k < Barra.NBarra.Count; k++)
                {
                    D_P_Solver[k] = D_P[k + 1];
                    double obs = D_P_Solver[k];
                }

                double[] erroP = new double[Barra.NBarra.Count];

                for (int k = 0; k < Barra.NBarra.Count; k++)
                {
                    erroP[k] = D_P_Solver[k];
                }

                if (erroP.Max() <= 0.00000001 && iteracaoP > 2)
                {
                    KP = 0;
                }

                if (KP == 1)
                {

                    // Calculando matriz H
                    for (int k = 1; k <= Barra.NBarra.Count; k++)
                    {
                        double Vk = V_solucao[k].Magnitude;

                        switch (Barra.Tipo[k])
                        {
                            case 2:
                                Jacobiano_H[k, k] = 1e40;
                                break;

                            case 0:
                                Jacobiano_H[k, k] = -Qcal[k] - B_matriz[k, k] * Math.Pow(Vk, 2);
                                break;

                            case 1:
                                Jacobiano_H[k, k] = -Qcal[k] - B_matriz[k, k] * Math.Pow(Vk, 2);
                                break;
                        }
                    }

                    for (int i = 1; i <= Linha.DaBarra.Count; i++)
                    {
                        int k = Linha.DaBarra[i];
                        int m = Linha.ParaBarra[i];
                        double Vk = V_solucao[k].Magnitude;
                        double Vm = V_solucao[m].Magnitude;
                        double TetaKM = V_solucao[k].Phase - V_solucao[m].Phase;

                        Jacobiano_H[k, m] = Vk * Vm * (G_matriz[k, m] * Math.Sin(TetaKM) - B_matriz[k, m] * Math.Cos(TetaKM));
                        Jacobiano_H[m, k] = -Vk * Vm * (G_matriz[k, m] * Math.Sin(TetaKM) + B_matriz[k, m] * Math.Cos(TetaKM));
                    }

                    for (int j = 0; j < Barra.NBarra.Count; j++)
                    {
                        for (int i = 0; i < Barra.NBarra.Count; i++)
                        {
                            Jacobiano_H[j, i] = Jacobiano_H[j + 1, i + 1];
                        }
                    }

                    double obs1 = Jacobiano_H[0, 0];
                    double obs2 = Jacobiano_H[0, 1];
                    double obs3 = Jacobiano_H[1, 0];
                    double obs4 = Jacobiano_H[1, 1];

                    LinearEquationSolver.Solve(D_P_Solver.Count, Jacobiano_H, D_P_Solver, D_theta);

                    for (int k = 1; k <= Barra.NBarra.Count; k++)
                    {
                        V_solucao[k] = Complex.FromPolarCoordinates(
                            V_solucao[k].Magnitude,
                            V_solucao[k].Phase + D_theta[k - 1]);
                    }

                    iteracaoP++;
                    KQ = 1;
                }

                else if (KQ == 0)
                {
                    break;
                }

                // Meia iteração Q
                for (int k = 1; k <= Barra.NBarra.Count; k++)
                {
                    Qcal[k] = -B_matriz[k, k] * Math.Pow(V_solucao[k].Magnitude, 2);
                }

                for (int i = 1; i <= Linha.DaBarra.Count; i++)
                {
                    int k = Linha.DaBarra[i];
                    int m = Linha.ParaBarra[i];
                    double Vk = V_solucao[k].Magnitude;
                    double Vm = V_solucao[m].Magnitude;
                    double TetaKM = (V_solucao[k].Phase - V_solucao[m].Phase + Linha.Defasagem[i]);
                    double gkm = G_matriz[k, m];
                    double bkm = B_matriz[k, m];

                    Qcal[k] += Vk * Vm * (gkm * Math.Sin(TetaKM) - bkm * Math.Cos(TetaKM));
                    Qcal[m] += Vk * Vm * (-gkm * Math.Sin(TetaKM) - bkm * Math.Cos(TetaKM));
                }

                for (int i = 1; i <= Barra.NBarra.Count; i++)
                {

                    switch (Barra.Tipo[i])
                    {
                        case 2:
                            D_Q[i] = 0;
                            break;

                        case 0:
                            D_Q[i] = (Barra.PotenciaReativaEsperada[i] / 100) - Qcal[i];
                            break;

                        case 1:
                            D_Q[i] = 0;
                            break;
                    }
                }

                for (int k = 0; k < Barra.NBarra.Count; k++)
                {
                    D_Q_Solver[k] = D_Q[k + 1];
                }

                double[] erroQ = new double[Barra.NBarra.Count];

                for (int k = 0; k < Barra.NBarra.Count; k++)
                {
                    erroQ[k] = D_Q_Solver[k];
                    double obs = erroQ[k];
                }

                if (erroP.Max() <= 0.00000001 && iteracaoP > 2)
                {
                    KQ = 0;
                }

                if (KQ == 1)
                {

                    // Calculando matriz L
                    for (int k = 1; k <= Barra.NBarra.Count; k++)
                    {
                        double Vk = V_solucao[k].Magnitude;

                        switch (Barra.Tipo[k])
                        {
                             case 2:
                                Jacobiano_L[k, k] = 1e40;
                                break;

                            case 0:
                                Jacobiano_L[k, k] = (Qcal[k] - B_matriz[k, k] * Math.Pow(Vk, 2)) / Vk;
                                break;

                            case 1:
                                Jacobiano_L[k, k] = 1e40;
                                break;
                        }
                    }

                    for (int i = 1; i <= Linha.DaBarra.Count; i++)
                    {
                        int k = Linha.DaBarra[i];
                        int m = Linha.ParaBarra[i];
                        double Vk = V_solucao[k].Magnitude;
                        double Vm = V_solucao[m].Magnitude;
                        double TetaKM = V_solucao[k].Phase - V_solucao[m].Phase;

                        Jacobiano_L[k, m] = Vk * (G_matriz[k, m] * Math.Sin(TetaKM) - B_matriz[k, m] * Math.Cos(TetaKM));
                        Jacobiano_L[m, k] = -Vm * (G_matriz[k, m] * Math.Sin(TetaKM) + B_matriz[k, m] * Math.Cos(TetaKM));
                    }

                    for (int j = 0; j < Barra.NBarra.Count; j++)
                    {
                        for (int i = 0; i < Barra.NBarra.Count; i++)
                        {
                            Jacobiano_L[j, i] = Jacobiano_L[j + 1, i + 1];
                        }
                    }

                    for (int j = 0; j < Barra.NBarra.Count; j++)
                    {
                        D_Q_Solver[j] = D_Q[j + 1];
                    }

                    LinearEquationSolver.Solve(D_Q_Solver.Count, Jacobiano_L, D_Q_Solver, D_V);

                    for (int k = 1; k <= Barra.NBarra.Count; k++)
                    {
                        V_solucao[k] = Complex.FromPolarCoordinates(
                            V_solucao[k].Magnitude + D_V[k - 1],
                            V_solucao[k].Phase);
                    }

                    iteracaoQ++;
                    KP = 1;
                }

                else if (KP == 0)
                {
                    break;
                }
            }
        }

        public void PreencherTabelaComplexa(DataGridView grade, SparseArray<int, Complex> V_solucao)
        {
            DataTable tabela = new DataTable();

            for (int i = 1; i <= Barra.NBarra.Count; i++)
            {
                tabela.Columns.Add("Barra " + Convert.ToString(i), typeof(Complex));
            }

            DataRow row = tabela.NewRow();

            for (int k = 1; k <= Barra.NBarra.Count; k++)
            {
                row[k - 1] = V_solucao[k];
            }

            tabela.Rows.Add(row);

            grade.DataSource = tabela;
        }
    }
}
