using SparseCollections;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Windows.Forms;
using System.Data;
using MathNet.Numerics.Statistics;

namespace Leitura
{
    class SolucaoFluxoPotenciaGaussSiedel
    {

        private Barra Barra;
        private Linha Linha;
        private int iteracao = 0;

        public Sparse2DMatrix<int, int, Complex> Y_matriz = new Sparse2DMatrix<int, int, Complex>();

        public Sparse2DMatrix<int, int, Complex> V_solucao = new Sparse2DMatrix<int, int, Complex>();
        public Sparse2DMatrix<int, int, Complex> S_solucao = new Sparse2DMatrix<int, int, Complex>();

        public SolucaoFluxoPotenciaGaussSiedel(Sparse2DMatrix<int, int, Complex> Y_mat, Barra Bar, Linha Lin)
        {
            Barra = Bar;
            Linha = Lin;
            Y_matriz = Y_mat;
        }

        public void CalculaFluxo()
        {
            while (Calcula_Condicao() == false)
            {
                for (int k = 1; k <= Barra.NBarra.Count; k++)
                {
                    switch (Barra.Tipo[k])
                    {
                        case 0:
                            S_solucao[iteracao, k] = new Complex(Barra.PotenciaAtivaEsperada[k], Barra.PotenciaReativaEsperada[k]);
                            Calcula_V(iteracao, k);
                            break;
                        case 1:
                            Calcula_Q(iteracao, k);
                            Calcula_V(iteracao, k);
                            break;
                        case 2:
                            S_solucao[iteracao, k] = new Complex(Barra.PotenciaAtivaEsperada[k], Barra.PotenciaReativaEsperada[k]);
                            V_solucao[iteracao, k] = new Complex(Barra.Tensao[k], 0);
                            break;
                    }
                }

                iteracao++;
            }
        }

        private void Calcula_V(int iteracao, int k)
        {
            if (iteracao == 0)
                V_solucao[iteracao, k] = 1;

            else
            {
                Complex iYkk = Complex.Divide(1, Y_matriz[k, k]);
                Complex Sk = Complex.Conjugate(Complex.Divide(S_solucao[iteracao, k] / 100, V_solucao[iteracao - 1, k]));

                Complex p1 = new Complex(0, 0);
                Complex p2 = new Complex(0, 0);

                for (int i = 1; i < k; i++)
                {
                    p1 += Y_matriz[k, i] * V_solucao[iteracao, i];
                }

                for (int i = k + 1; i <= Barra.NBarra.Count; i++)
                {
                    p2 += Y_matriz[k, i] * V_solucao[iteracao - 1, i];
                }

                V_solucao[iteracao, k] = iYkk * (Sk - p1 - p2);
            }
        }

        private void Calcula_Q(int iteracao, int k)
        {
            Complex Ek = V_solucao[k, iteracao - 1];
            //double p1 = 0;

            //for (int i = 1; Barra.NBarra.Count < k; i++)
            //{
            //    p1 += Ek.Magnitude * Y_matriz[k, i].Magnitude * V_solucao[i, iteracao - 1].Magnitude * Math.Sin(Barra.Angulo[k] - Barra.Angulo[i] - Y_matriz[k, i].Phase);
            //}

            //S_solucao[iteracao, k] = new Complex(Barra.PotenciaAtivaEsperada[k], p1*100);
            S_solucao[iteracao, k] = new Complex(100,0);
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

        private Boolean Calcula_Condicao()
        {
            double[] erro = new double[Barra.NBarra.Count];

            for (int k = 1; k <= Barra.NBarra.Count; k++)
            {
                erro[k-1] = Math.Abs((V_solucao[iteracao-1, k] - V_solucao[iteracao - 2, k]).Magnitude);
            }

            if (erro.Max() <= 0.0000000001 && iteracao > 5)
                return true;
            else
                return false;
        }
    }
}
