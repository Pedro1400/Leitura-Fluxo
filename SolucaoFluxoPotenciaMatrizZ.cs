using SparseCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using System.Windows.Forms;
using System.Data;

namespace Leitura
{
    class SolucaoFluxoPotenciaMatrizZ
    {
        private Barra Barra;
        private Linha Linha;
        int BarraSlack;
        private int iteracao = 0;

        SparseArray<int, Complex> C = new SparseArray<int, Complex>();

        public Sparse2DMatrix<int, int, Complex> Y_matriz = new Sparse2DMatrix<int, int, Complex>();
        public Sparse2DMatrix<int, int, Complex> Z_matriz = new Sparse2DMatrix<int, int, Complex>();

        public Sparse2DMatrix<int, int, Complex> V_solucao = new Sparse2DMatrix<int, int, Complex>();
        public Sparse2DMatrix<int, int, Complex> S_solucao = new Sparse2DMatrix<int, int, Complex>();
        public Sparse2DMatrix<int, int, Complex> I_solucao = new Sparse2DMatrix<int, int, Complex>();

        public SolucaoFluxoPotenciaMatrizZ(Sparse2DMatrix<int, int, Complex> Y_mat, Barra Bar, Linha Lin)
        {
            Barra = Bar;
            Linha = Lin;
            Y_matriz = Y_mat;
            Calcula_Z_matriz();
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

                if (iteracao == 0)
                {
                    for (int k = 1; k <= Barra.NBarra.Count; k++)
                    {
                        for (int i = 1; i <= Barra.NBarra.Count; i++)
                        {
                            if (i != BarraSlack)
                                C[k] += Z_matriz[k, i] * Y_matriz[i, BarraSlack] * V_solucao[iteracao, BarraSlack];
                        }
                    }
                }

                iteracao++;
            }
        }

        private void Calcula_V(int iteracao, int k)
        {
            if (iteracao == 0)
            {
                V_solucao[iteracao, k] = 1;
                I_solucao[iteracao, k] = Complex.Conjugate(Complex.Divide(S_solucao[iteracao, k] / 100, V_solucao[iteracao, k]));
            }

            else
            {
                Complex p1 = new Complex(0, 0);
                Complex p2 = new Complex(0, 0);

                for (int i = 2; i < k; i++)
                {
                    if (i != BarraSlack)
                    {
                        p1 += Z_matriz[k, i] * I_solucao[iteracao, i];
                        Complex x = I_solucao[iteracao, i];
                    }
                }

                for (int i = k; i <= Barra.NBarra.Count; i++)
                {
                   if (i != BarraSlack)
                   {
                        p2 += Z_matriz[k, i] * I_solucao[iteracao - 1, i];
                        Complex x = I_solucao[iteracao - 1, i];
                   }
                }

                V_solucao[iteracao, k] = p1 + p2 - C[k];
                I_solucao[iteracao, k] = Complex.Conjugate(Complex.Divide(S_solucao[iteracao, k] / 100, V_solucao[iteracao, k]));
            }
        }

        private void Calcula_Q(int iteracao, int k)
        {

        }

        private void Calcula_Z_matriz()
        {
            Matrix<Complex32> Yl_matriz = Matrix<Complex32>.Build.Dense(Barra.NBarra.Count, Barra.NBarra.Count);

            // Identificando a Barra de Referência
            for (int k = 1; k <= Barra.NBarra.Count; k++)
            {
                if (Barra.Tipo[k] == 2)
                {
                    BarraSlack = k;
                    break;
                }
            }

            // Nota: As mastrizes e vetores têm sempre tamanho Barra.NBarra.Count + 1, pois o elemento 0 nunca é incrementado.
            //       Isto possibilita associar o número da barra(k) ao elemento array[k] sem a necessidade de modificação nos
            //       índices. Para o cálculo da matriz inversa, esse artifício não é possível, pois as lacunas nos índices 0
            //       afetarão no cálculo.

            for (int i = 0; i < Barra.NBarra.Count; i++)
            {
                for (int j = 0; j < Barra.NBarra.Count; j++)
                {
                    if (i == BarraSlack - 1 && j == BarraSlack - 1)
                    {
                        Yl_matriz[i, j] = new Complex32(float.Parse(Y_matriz[i + 1, j + 1].Real.ToString()) * 200000000000000000, float.Parse(Y_matriz[i + 1, j + 1].Imaginary.ToString()) * 200000000000000000);
                    }
                    else
                    {
                        Yl_matriz[i, j] = new Complex32(float.Parse(Y_matriz[i + 1, j + 1].Real.ToString()), float.Parse(Y_matriz[i + 1, j + 1].Imaginary.ToString()));
                    }
                }
            }

            Matrix<Complex32> Z1_matriz = Yl_matriz.Inverse();

            for (int i = 1; i <= Barra.NBarra.Count; i++)
            {
                for (int j = 1; j <= Barra.NBarra.Count; j++)
                {
                    Z_matriz[i, j] = new Complex(double.Parse(Z1_matriz[i - 1, j - 1].Real.ToString()), double.Parse(Z1_matriz[i - 1, j - 1].Imaginary.ToString()));
                }
            }
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

        public void PreencherTabelaMatriz_Z(DataGridView grade, Sparse2DMatrix<int, int, Complex> Matriz)
        {
            DataTable tabela = new DataTable();

            for (int i = 1; i <= Barra.NBarra.Count; i++)
            {
                tabela.Columns.Add(Convert.ToString(i), typeof(Complex));
            }


            for (int i = 1; i <= Barra.NBarra.Count; i++)
            {
                DataRow row = tabela.NewRow();

                for (int k = 1; k <= Barra.NBarra.Count; k++)
                {
                    row[k - 1] = Matriz[i, k];
                }

                tabela.Rows.Add(row);
            }

            grade.DataSource = tabela;
        }

        private Boolean Calcula_Condicao()
        {
            double[] erro = new double[Barra.NBarra.Count];

            for (int k = 1; k <= Barra.NBarra.Count; k++)
            {
                erro[k - 1] = Math.Abs((V_solucao[iteracao - 1, k] - V_solucao[iteracao - 2, k]).Magnitude);
            }

            if (erro.Max() <= 0.0000000001 && iteracao > 5)
                return true;
            else
                return false;
        }
    }
}
