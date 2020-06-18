using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using SparseCollections;
using MathNet;
using MathNet.Numerics;

namespace Leitura
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

            
        }

        StreamReader arquivo;
        NumberFormatInfo nomeDoFormato = new CultureInfo("pt-BR", false).NumberFormat;
        string textlinha;

        int TipoTrafo = 2;
        int Metodo = 1;

        Barra Barra = new Barra();
        Linha Linha = new Linha();

        private void abrirToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFile = new OpenFileDialog();
            browseFile.Filter = "PWF Files (*.PWF)|*.PWF";
            browseFile.Title = "Browse PWF file";

            if (browseFile.ShowDialog() == DialogResult.OK)
            {
                Stream myStream = browseFile.OpenFile();
                arquivo = new StreamReader(myStream);

                while ((textlinha = arquivo.ReadLine()) != null)
                {
                    string indicador = textlinha.Trim();
                    
                    switch (indicador)
                    {
                        case "DBAR":
                            // Ler e mostrar dados de barra
                            Barra.Clear();
                            Barra.Ler(textlinha, arquivo);
                            Barra.PreencherTabela(dataGridView1);
                            break;

                        case "DLIN":
                            // Ler e mostrar dados de Linha  
                            Linha.Clear();
                            Linha.Ler(textlinha, arquivo);
                            Linha.PreencherTabela(dataGridView2);
                            break;

                        case "FIM":
                            break;
                    }
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void executarToolStripMenuItem_Click(object sender, EventArgs e)
        {

            MatrizAdmitancia Matriz = new MatrizAdmitancia(Barra, Linha, TipoTrafo);
            Matriz.DefinirTipoTransformador();
            Matriz.CalculaMatrizAdmitancia();
            Matriz.PreencherTabela(dataGridView3, Matriz.G_matriz);
            Matriz.PreencherTabela(dataGridView4, Matriz.B_matriz);
            Matriz.PreencherTabelaComplexa(dataGridView5, Matriz.Y_matriz);

            switch(Metodo)
            {
                case 1:
                    SolucaoFluxoPotenciaGaussSiedel SolucaoGS = new SolucaoFluxoPotenciaGaussSiedel(Matriz.Y_matriz, Barra, Linha);
                    SolucaoGS.CalculaFluxo();
                    SolucaoGS.PreencherTabelaComplexa(dataGridView6, SolucaoGS.V_solucao);
                    break;

                case 2:
                    SolucaoFluxoPotenciaMatrizZ SolucaoZ = new SolucaoFluxoPotenciaMatrizZ(Matriz.Y_matriz, Barra, Linha);
                    SolucaoZ.CalculaFluxo();
                    SolucaoZ.PreencherTabelaComplexa(dataGridView6, SolucaoZ.V_solucao);
                    SolucaoZ.PreencherTabelaMatriz_Z(dataGridView7, SolucaoZ.Z_matriz);
                    break;

                case 3:
                    break;
            }

        }

        // Menu para a escolha do método do cálculo de fluxo de potência ------------------------------
        private void gaussSiedelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gaussSiedelToolStripMenuItem.Checked = true;
            matrizZToolStripMenuItem.Checked = false;
            newtonRaphsonToolStripMenuItem.Checked = false;
            Metodo = 1;
        }

        private void matrizZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gaussSiedelToolStripMenuItem.Checked = false;
            matrizZToolStripMenuItem.Checked = true;
            newtonRaphsonToolStripMenuItem.Checked = false;
            Metodo = 2;
        }

        //---------------------------------------------------------------------------------------------

        // Menu para a escolha do modelo de trafo -------------------------------------------------------
        private void Modelo1_Click(object sender, EventArgs e)
        {
            Modelo1.Checked = true;
            Modelo2.Checked = false;
            Modelo3.Checked = false;
            Modelo4.Checked = false;
            TipoTrafo = 1;
        }

        private void Modelo2_Click(object sender, EventArgs e)
        {
            Modelo1.Checked = false;
            Modelo2.Checked = true;
            Modelo3.Checked = false;
            Modelo4.Checked = false;
            TipoTrafo = 2;
        }

        private void Modelo3_Click(object sender, EventArgs e)
        {
            Modelo1.Checked = false;
            Modelo2.Checked = false;
            Modelo3.Checked = true;
            Modelo4.Checked = false;
            TipoTrafo = 3;
        }

        private void Modelo4_Click(object sender, EventArgs e)
        {

            Modelo1.Checked = false;
            Modelo2.Checked = false;
            Modelo3.Checked = false;
            Modelo4.Checked = true;
            TipoTrafo = 4;
        }

        // ----------------------------------------------------------------------------------------------
    }
}
