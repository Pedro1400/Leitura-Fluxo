using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SparseCollections;

namespace Leitura
{
    class MatrizAdmitancia
    {
        private Barra Barra;
        private Linha Linha;
        private int TipoTrafo;

        public MatrizAdmitancia(Barra Bar, Linha Lin, int TipoTraf)
        {
            Barra = Bar;
            Linha = Lin;
            TipoTrafo = TipoTraf;
        }

        // Varáveis para o cálculo da matriz admitância
        public SparseArray<int, double> akk = new SparseArray<int, double>();
        public SparseArray<int, double> amm = new SparseArray<int, double>();
        public SparseArray<int, double> akm = new SparseArray<int, double>();
        public SparseArray<int, double> Sh_barra = new SparseArray<int, double>();
        public SparseArray<int, double> b_km = new SparseArray<int, double>();
        public SparseArray<int, double> g_km = new SparseArray<int, double>();
        public SparseArray<int, double> b_km_sh = new SparseArray<int, double>();
        public SparseArray<int, double> x_km = new SparseArray<int, double>();

        // Matrizes Condutância G e Susceptância B
        public Sparse2DMatrix<int, int, double> G_matriz = new Sparse2DMatrix<int, int, double>();
        public Sparse2DMatrix<int, int, double> B_matriz = new Sparse2DMatrix<int, int, double>();
        public Sparse2DMatrix<int, int, double> Bl_matriz = new Sparse2DMatrix<int, int, double>();
        public Sparse2DMatrix<int, int, Complex> Y_matriz = new Sparse2DMatrix<int, int, Complex>();

        public void DefinirTipoTransformador()
        {
            double tap;

            for (int i = 1; i <= Linha.DaBarra.Count; i++)
            {
                tap = Linha.Tap[i];

                switch (TipoTrafo)
                {
                    case 1:
                        akk[i] = Math.Pow(tap, 2);
                        amm[i] = 1;
                        akm[i] = tap;
                        break;
                    case 2:
                        akk[i] = 1 / Math.Pow(tap, 2);
                        amm[i] = 1;
                        akm[i] = 1 / tap;
                        break;
                    case 3:
                        akk[i] = 1;
                        amm[i] = Math.Pow(tap, 2);
                        akm[i] = tap;
                        break;
                    case 4:
                        akk[i] = 1;
                        amm[i] = 1 / Math.Pow(tap, 2);
                        akm[i] = 1 / tap;
                        break;
                }
            }


        }

        public void CalculaMatrizAdmitancia()
        {
            for (int i = 1; i <= Barra.NBarra.Count; i++)
            {
                int k = i;
                Sh_barra[i] = Barra.CapacitorReator[i]/100;
                B_matriz[k, k] = B_matriz[k, k] + Sh_barra[k];
            }

            for (int i = 1; i <= Linha.DaBarra.Count; i++)
            {
                int k = Linha.DaBarra[i];
                int m = Linha.ParaBarra[i];

                g_km[i] = Linha.Resistencia[i] / (Math.Pow(Linha.Resistencia[i], 2) + Math.Pow(Linha.Reatancia[i], 2))*100;
                b_km[i] = -Linha.Reatancia[i] / (Math.Pow(Linha.Resistencia[i], 2) + Math.Pow(Linha.Reatancia[i], 2))*100;
                b_km_sh[i] = Linha.Susceptancia[i] / 2000;
                x_km[i] = Linha.Reatancia[i];

                G_matriz[k, k] = G_matriz[k, k] + g_km[i] * akk[i];//Montagem da Matriz G
                G_matriz[m, m] = G_matriz[m, m] + g_km[i] * amm[i];
                G_matriz[k, m] = G_matriz[k, m] - g_km[i] * akm[i];
                G_matriz[m, k] = G_matriz[m, k] - g_km[i] * akm[i];

                B_matriz[k, k] = B_matriz[k, k] + b_km[i] * akk[i] + b_km_sh[i];//Montagem da Matriz B
                B_matriz[m, m] = B_matriz[m, m] + b_km[i] * amm[i] + b_km_sh[i];
                B_matriz[k, m] = B_matriz[k, m] - b_km[i] * akm[i];
                B_matriz[m, k] = B_matriz[m, k] - b_km[i] * akm[i];

                Bl_matriz[k, k] = Bl_matriz[k, k] + (1 / x_km[i] * akk[i]);//Matriz B' para os métodos desacoplados e XB
                Bl_matriz[m, m] = Bl_matriz[m, m] + (1 / x_km[i] * amm[i]);
                Bl_matriz[k, m] = Bl_matriz[k, m] - (1 / x_km[i] * akm[i]);
                Bl_matriz[m, k] = Bl_matriz[m, k] - (1 / x_km[i] * akm[i]);

            }

            for (int i = 1; i <= Barra.NBarra.Count; i++)
            {
                for (int j = 1; j <= Barra.NBarra.Count; j++)
                {
                    Y_matriz[i, j] = new Complex(G_matriz[i, j], B_matriz[i, j]);
                }
            }

        }

        public void PreencherTabela(DataGridView grade, Sparse2DMatrix<int, int, double> Matriz)
        {
            DataTable tabela = new DataTable();

            for (int i = 1; i <= Barra.NBarra.Count; i++)
            {
                tabela.Columns.Add(Convert.ToString(i), typeof(double));
            }


            for (int i = 1; i <= Barra.NBarra.Count; i++)
            {
                DataRow row = tabela.NewRow();

                for (int k = 1; k <= Barra.NBarra.Count; k++)
                {
                    row[k-1] = Matriz[i, k];
                }

                tabela.Rows.Add(row);
            }

            grade.DataSource = tabela;
        }

        public void PreencherTabelaComplexa(DataGridView grade, Sparse2DMatrix<int, int, Complex> Matriz)
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
    }
}
