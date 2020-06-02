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
        
        // Variáveis de Barra
        SparseArray<int, int> NBarra = new SparseArray<int, int>();
        SparseArray<int, string> OperacaoBarra = new SparseArray<int, string>();
        SparseArray<int, string> EstadoBarra = new SparseArray<int, string>();
        SparseArray<int, int> Tipo = new SparseArray<int, int>();
        SparseArray<int, int> GrupoDeBaseDeTensao = new SparseArray<int, int>();
        SparseArray<int, string> Nome = new SparseArray<int, string>();
        SparseArray<int, int> GrupoDeLimiteDeTensao = new SparseArray<int, int>();
        SparseArray<int, double> Tensao = new SparseArray<int, double>();
        SparseArray<int, double> Angulo = new SparseArray<int, double>();
        SparseArray<int, double> GeracaoAtiva = new SparseArray<int, double>();
        SparseArray<int, double> GeracaoReativa = new SparseArray<int, double>();
        SparseArray<int, double> GeracaoReativaMinima = new SparseArray<int, double>();
        SparseArray<int, double> GeracaoReativaMaxima = new SparseArray<int, double>();
        SparseArray<int, int> BarraControladaBarra = new SparseArray<int, int>();
        SparseArray<int, double> CargaAtiva = new SparseArray<int, double>();
        SparseArray<int, double> CargaReativa = new SparseArray<int, double>();
        SparseArray<int, double> CapacitorReator = new SparseArray<int, double>();
        SparseArray<int, int> Area = new SparseArray<int, int>();
        SparseArray<int, double> TensaoParaDefinicaoDeCarga = new SparseArray<int, double>();
        SparseArray<int, int> ModoDeVisualizacao = new SparseArray<int, int>();
        SparseArray<int, int> AgregadorBarra1 = new SparseArray<int, int>();
        SparseArray<int, int> AgregadorBarra2 = new SparseArray<int, int>();
        SparseArray<int, int> AgregadorBarra3 = new SparseArray<int, int>();
        SparseArray<int, int> AgregadorBarra4 = new SparseArray<int, int>();
        SparseArray<int, int> AgregadorBarra5 = new SparseArray<int, int>();

        // Variáveis de Linha
        SparseArray<int, int> DaBarra = new SparseArray<int, int>();
        SparseArray<int, string> AberturaDaBarra = new SparseArray<int, string>();
        SparseArray<int, string> OperacaoLinha = new SparseArray<int, string>();
        SparseArray<int, string> AberturaParaBarra = new SparseArray<int, string>();
        SparseArray<int, int> ParaBarra = new SparseArray<int, int>();
        SparseArray<int, int> Circuito = new SparseArray<int, int>();
        SparseArray<int, string> EstadoLinha = new SparseArray<int, string>();
        SparseArray<int, string> Proprietario = new SparseArray<int, string>();
        SparseArray<int, double> Resistencia = new SparseArray<int, double>();
        SparseArray<int, double> Reatancia = new SparseArray<int, double>();
        SparseArray<int, double> Susceptancia = new SparseArray<int, double>();
        SparseArray<int, double> Tap = new SparseArray<int, double>();
        SparseArray<int, double> TapMinimo = new SparseArray<int, double>();
        SparseArray<int, double> TapMaximo = new SparseArray<int, double>();
        SparseArray<int, double> Defasagem = new SparseArray<int, double>();
        SparseArray<int, int> BarraControladaLinha = new SparseArray<int, int>();
        SparseArray<int, double> CapacidadeNormal = new SparseArray<int, double>();
        SparseArray<int, double> CapacidadeEmEmergencia = new SparseArray<int, double>();
        SparseArray<int, int> NumeroDeSteps = new SparseArray<int, int>();
        SparseArray<int, double> CapacidadeEquipamento = new SparseArray<int, double>();
        SparseArray<int, int> AgregadorLinha1 = new SparseArray<int, int>();
        SparseArray<int, int> AgregadorLinha2 = new SparseArray<int, int>();
        SparseArray<int, int> AgregadorLinha3 = new SparseArray<int, int>();
        SparseArray<int, int> AgregadorLinha4 = new SparseArray<int, int>();
        SparseArray<int, int> AgregadorLinha5 = new SparseArray<int, int>();
        SparseArray<int, int> AgregadorLinha6 = new SparseArray<int, int>();
        SparseArray<int, int> AgregadorLinha7 = new SparseArray<int, int>();
        SparseArray<int, int> AgregadorLinha8 = new SparseArray<int, int>();
        SparseArray<int, int> AgregadorLinha9 = new SparseArray<int, int>();
        SparseArray<int, int> AgregadorLinha10 = new SparseArray<int, int>();

        private void abrirToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseFile = new OpenFileDialog();
            browseFile.Filter = "PWF Files (*.PWF)|*.PWF";
            browseFile.Title = "Browse PWF file";

            if (browseFile.ShowDialog() == DialogResult.OK)
            {
                Stream myStream = browseFile.OpenFile();
                arquivo = new System.IO.StreamReader(myStream);//leitura

                while ((textlinha = arquivo.ReadLine()) != null)
                {
                    string indicador = textlinha.Trim();
                    
                    switch (indicador)
                    {
                        case "DBAR":
                            //ler dados de barra
                            lerbarras();
                            PrencherTabelaBarras();
                            break;
                        case "DLIN":
                            //ler dados de Linha  
                            lerlinhas();
                            PrencherTabelaLinhas();
                            break;
                        case "FIM":
                            //ler dados de Linha
                            break;
                    }
                }
            }
        }

        private void lerbarras()
        {
            textlinha = arquivo.ReadLine();
            int i = 1;
            while ((textlinha = arquivo.ReadLine()) != "99999")
            {//Atribue a uma variavel os dados da barra
                textlinha = textlinha.PadRight(textlinha.Length + 100,' ');
                NBarra[i] = Convert.ToInt32(textlinha.Substring(0, 5));
                OperacaoBarra[i] = textlinha.Substring(5, 1).Trim() == ""? "A": textlinha.Substring(5, 1);
                EstadoBarra[i] = textlinha.Substring(6, 1).Trim() == ""? "L": textlinha.Substring(6, 1);
                Tipo[i] = Convert.ToInt32(textlinha.Substring(7, 1).Trim() == "" ? "0" : textlinha.Substring(7, 1));
                GrupoDeBaseDeTensao[i] = Convert.ToInt32(textlinha.Substring(8, 2).Trim() == "" ? "0" : textlinha.Substring(8, 2));
                Nome[i] = textlinha.Substring(10, 12).Trim() == "" ? "" : textlinha.Substring(10, 12);
                GrupoDeLimiteDeTensao[i] = Convert.ToInt32(textlinha.Substring(22, 2).Trim() == "" ? "0" : textlinha.Substring(22, 2));
                Tensao[i] = double.Parse(textlinha.Substring(24, 4).Trim() == ""?"1,0" : textlinha.Substring(24,4), CultureInfo.InvariantCulture);
                Angulo[i] = double.Parse(textlinha.Substring(28, 4).Trim() == "" ? "0,0" : textlinha.Substring(28, 4), CultureInfo.InvariantCulture);
                GeracaoAtiva[i] = double.Parse(textlinha.Substring(32, 5).Trim() == "" ? "0,0" : textlinha.Substring(32, 5), CultureInfo.InvariantCulture);
                GeracaoReativa[i] = double.Parse(textlinha.Substring(37, 5).Trim() == "" ? "0,0" : textlinha.Substring(37, 5), CultureInfo.InvariantCulture);
                GeracaoReativaMinima[i] = double.Parse(textlinha.Substring(42, 5).Trim() == "" ? "0,0" : textlinha.Substring(42, 5), CultureInfo.InvariantCulture);
                GeracaoReativaMaxima[i] = double.Parse(textlinha.Substring(47, 5).Trim() == "" ? "0,0" : textlinha.Substring(47, 5), CultureInfo.InvariantCulture);
                BarraControladaBarra[i] = Convert.ToInt32(textlinha.Substring(52, 6).Trim() == "" ? textlinha.Substring(0, 5) : textlinha.Substring(52, 6));
                CargaAtiva[i] = double.Parse(textlinha.Substring(58, 5).Trim() == "" ? "0,0" : textlinha.Substring(58, 5), CultureInfo.InvariantCulture);
                CargaReativa[i] = double.Parse(textlinha.Substring(63, 5).Trim() == "" ? "0,0" : textlinha.Substring(63, 5), CultureInfo.InvariantCulture);
                CapacitorReator[i] = double.Parse(textlinha.Substring(68, 5).Trim() == "" ? "0,0" : textlinha.Substring(68, 5), CultureInfo.InvariantCulture);
                Area[i] = Convert.ToInt32(textlinha.Substring(68, 3).Trim() == "" ? "1" : textlinha.Substring(68, 3));
                TensaoParaDefinicaoDeCarga[i] = double.Parse(textlinha.Substring(76, 4).Trim() == "" ? "1,0" : textlinha.Substring(76, 4), CultureInfo.InvariantCulture);
                ModoDeVisualizacao[i] = Convert.ToInt32(textlinha.Substring(80, 1).Trim() == "" ? "0" : textlinha.Substring(80, 1));
                AgregadorBarra1[i] = Convert.ToInt32(textlinha.Substring(81, 3).Trim() == "" ? "0" : textlinha.Substring(81, 3));
                AgregadorBarra2[i] = Convert.ToInt32(textlinha.Substring(84, 3).Trim() == "" ? "0" : textlinha.Substring(84, 3));
                AgregadorBarra3[i] = Convert.ToInt32(textlinha.Substring(87, 3).Trim() == "" ? "0" : textlinha.Substring(87, 3));
                AgregadorBarra4[i] = Convert.ToInt32(textlinha.Substring(90, 3).Trim() == "" ? "0" : textlinha.Substring(90, 3));
                AgregadorBarra5[i] = Convert.ToInt32(textlinha.Substring(93, 3).Trim() == "" ? "0" : textlinha.Substring(93, 3));
                i++;
            }
        }

        private void PrencherTabelaBarras()
        {
            DataTable tabelaBarras = new DataTable();
            int i;
            //criando as colunas das tabelas//
            tabelaBarras.Columns.Add("Barra N°", typeof(int));
            tabelaBarras.Columns.Add("Operação", typeof(string));
            tabelaBarras.Columns.Add("Estado", typeof(string));
            tabelaBarras.Columns.Add("Tipo", typeof(int));
            tabelaBarras.Columns.Add("Grupo de Base de Tensão", typeof(int));
            tabelaBarras.Columns.Add("Nome", typeof(string));
            tabelaBarras.Columns.Add("Grupo de Limite de Tensão", typeof(int));
            tabelaBarras.Columns.Add("Tensão (pu)", typeof(double));
            tabelaBarras.Columns.Add("Ângulo (graus)", typeof(double));
            tabelaBarras.Columns.Add("Geração Ativa (MW)", typeof(double));
            tabelaBarras.Columns.Add("Geração Reativa (Mvar)", typeof(double));
            tabelaBarras.Columns.Add("Geração Reativa Mínima (Mvar)", typeof(double));
            tabelaBarras.Columns.Add("Geração Reativa Máxima (Mvar)", typeof(double));
            tabelaBarras.Columns.Add("Barra Controlada", typeof(int));
            tabelaBarras.Columns.Add("Carga Ativa (MW)", typeof(double));
            tabelaBarras.Columns.Add("Carga Reativa (Mvar)", typeof(double));
            tabelaBarras.Columns.Add("Capacitor Reator (Mvar)", typeof(double));
            tabelaBarras.Columns.Add("Área", typeof(int));
            tabelaBarras.Columns.Add("Tensão Para Definição de Carga (pu)", typeof(double));
            tabelaBarras.Columns.Add("Modo de Vizualização", typeof(int));
            tabelaBarras.Columns.Add("Agregador 1", typeof(string));
            tabelaBarras.Columns.Add("Agregador 2", typeof(string));
            tabelaBarras.Columns.Add("Agregador 3", typeof(string));
            tabelaBarras.Columns.Add("Agregador 4", typeof(string));
            tabelaBarras.Columns.Add("Agregador 5", typeof(string));

            for (i = 1; i <= NBarra.Count; i++)
            {
                tabelaBarras.Rows.Add(NBarra[i], OperacaoBarra[i], EstadoBarra[i], Tipo[i], GrupoDeBaseDeTensao[i], Nome[i], GrupoDeLimiteDeTensao[i], Tensao[i], Angulo[i], GeracaoAtiva[i], GeracaoReativa[i], GeracaoReativaMinima[i], GeracaoReativaMaxima[i], BarraControladaBarra[i], CargaAtiva[i], CargaReativa[i], CapacitorReator[i], Area[i], TensaoParaDefinicaoDeCarga[i], ModoDeVisualizacao[i], AgregadorBarra1[i], AgregadorBarra2[i], AgregadorBarra3[i], AgregadorBarra4[i], AgregadorBarra5[i]);
            }
            dataGridView1.DataSource = tabelaBarras;
        }

        private void lerlinhas()
        {
            textlinha = arquivo.ReadLine();
            int i = 1;
            while ((textlinha = arquivo.ReadLine()) != "99999")
            {//Atribue a uma variavel os dados de linha
                textlinha = textlinha.PadRight(textlinha.Length + 100, ' ');
                DaBarra[i] = Convert.ToInt32(textlinha.Substring(0, 5));
                AberturaDaBarra[i] = textlinha.Substring(5, 1).Trim() == "" ? "L" : textlinha.Substring(5, 1);
                OperacaoLinha[i] = textlinha.Substring(7, 1).Trim() == "" ? "A" : textlinha.Substring(7, 1);
                AberturaParaBarra[i] = textlinha.Substring(9, 1).Trim() == "" ? "L" : textlinha.Substring(9, 1);
                ParaBarra[i] = Convert.ToInt32(textlinha.Substring(10, 5).Trim() == "" ? "" : textlinha.Substring(10,5));
                Circuito[i] = Convert.ToInt32(textlinha.Substring(15, 2).Trim() == "" ? "" : textlinha.Substring(15, 2));
                EstadoLinha[i] = textlinha.Substring(17, 1).Trim() == "" ? "L" : textlinha.Substring(17, 1);
                Proprietario[i] = textlinha.Substring(18, 1).Trim() == "" ? "F" : textlinha.Substring(18, 1);
                Resistencia[i] = double.Parse(textlinha.Substring(20, 6).Trim() == "" ? "0,0" : textlinha.Substring(20, 6), CultureInfo.InvariantCulture);
                Reatancia[i] = double.Parse(textlinha.Substring(26, 6).Trim() == "" ? "" : textlinha.Substring(26, 6), CultureInfo.InvariantCulture);
                Susceptancia[i] = double.Parse(textlinha.Substring(32, 6).Trim() == "" ? "0,0" : textlinha.Substring(32, 6), CultureInfo.InvariantCulture);
                Tap[i] = double.Parse(textlinha.Substring(38, 5).Trim() == "" ? "0" : textlinha.Substring(38, 5), CultureInfo.InvariantCulture);
                TapMinimo[i] = double.Parse(textlinha.Substring(43, 5).Trim() == "" ? "0" : textlinha.Substring(43, 5), CultureInfo.InvariantCulture);
                TapMaximo[i] = double.Parse(textlinha.Substring(48, 5).Trim() == "" ? "0" : textlinha.Substring(48, 5), CultureInfo.InvariantCulture);
                Defasagem[i] = double.Parse(textlinha.Substring(53, 5).Trim() == "" ? "0,0" : textlinha.Substring(53, 5), CultureInfo.InvariantCulture);
                BarraControladaLinha[i] = Convert.ToInt32(textlinha.Substring(58, 6).Trim() == "" ? textlinha.Substring(0, 5) : textlinha.Substring(58, 6));
                CapacidadeNormal[i] = double.Parse(textlinha.Substring(64, 4).Trim() == "" ? "9999" : textlinha.Substring(64, 4), CultureInfo.InvariantCulture);
                CapacidadeEmEmergencia[i] = double.Parse(textlinha.Substring(68, 4).Trim() == "" ? textlinha.Substring(64, 4).Trim() : textlinha.Substring(68, 4), CultureInfo.InvariantCulture);
                NumeroDeSteps[i] = Convert.ToInt32(textlinha.Substring(72, 2).Trim() == "" ? "0" : textlinha.Substring(72, 2));
                CapacidadeEquipamento[i] = double.Parse(textlinha.Substring(74, 4).Trim() == "" ? textlinha.Substring(64, 4).Trim() : textlinha.Substring(74, 4), CultureInfo.InvariantCulture);
                AgregadorLinha1[i] = Convert.ToInt32(textlinha.Substring(78, 3).Trim() == "" ? "0" : textlinha.Substring(78, 3));
                AgregadorLinha2[i] = Convert.ToInt32(textlinha.Substring(81, 3).Trim() == "" ? "0" : textlinha.Substring(81, 3));
                AgregadorLinha3[i] = Convert.ToInt32(textlinha.Substring(84, 3).Trim() == "" ? "0" : textlinha.Substring(84, 3));
                AgregadorLinha4[i] = Convert.ToInt32(textlinha.Substring(87, 3).Trim() == "" ? "0" : textlinha.Substring(87, 3));
                AgregadorLinha5[i] = Convert.ToInt32(textlinha.Substring(90, 3).Trim() == "" ? "0" : textlinha.Substring(90, 3));
                AgregadorLinha6[i] = Convert.ToInt32(textlinha.Substring(93, 3).Trim() == "" ? "0" : textlinha.Substring(93, 3));
                AgregadorLinha7[i] = Convert.ToInt32(textlinha.Substring(96, 3).Trim() == "" ? "0" : textlinha.Substring(96, 3));
                AgregadorLinha8[i] = Convert.ToInt32(textlinha.Substring(99, 3).Trim() == "" ? "0" : textlinha.Substring(99, 3));
                AgregadorLinha9[i] = Convert.ToInt32(textlinha.Substring(102, 3).Trim() == "" ? "0" : textlinha.Substring(102, 3));
                AgregadorLinha10[i] = Convert.ToInt32(textlinha.Substring(105, 3).Trim() == "" ? "0" : textlinha.Substring(105, 3));
                i++;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void PrencherTabelaLinhas()
        {
            DataTable tabelaLinhas = new DataTable();
            int i;
            //criando as colunas das tabelas//
            tabelaLinhas.Columns.Add("Da Barra", typeof(int));
            tabelaLinhas.Columns.Add("Abertura da Barra", typeof(string));
            tabelaLinhas.Columns.Add("Operação", typeof(string));
            tabelaLinhas.Columns.Add("Abertura para Barra", typeof(string));
            tabelaLinhas.Columns.Add("Para Barra", typeof(int));
            tabelaLinhas.Columns.Add("Circuito", typeof(int));
            tabelaLinhas.Columns.Add("Estado", typeof(string));
            tabelaLinhas.Columns.Add("Proprietário", typeof(string));
            tabelaLinhas.Columns.Add("Resistência (%)", typeof(double));
            tabelaLinhas.Columns.Add("Reatância (%)", typeof(double));
            tabelaLinhas.Columns.Add("Susceptância (Mvar)", typeof(double));
            tabelaLinhas.Columns.Add("Tap", typeof(double));
            tabelaLinhas.Columns.Add("Tap Mínimo", typeof(double));
            tabelaLinhas.Columns.Add("Tap Máximo", typeof(double));
            tabelaLinhas.Columns.Add("Defasagem (graus)", typeof(double));
            tabelaLinhas.Columns.Add("Barra Controlada", typeof(int));
            tabelaLinhas.Columns.Add("Capacidade Normal (MVA)", typeof(double));
            tabelaLinhas.Columns.Add("Capacidade em Emergência (MVA)", typeof(double));
            tabelaLinhas.Columns.Add("Número de Steps", typeof(int));
            tabelaLinhas.Columns.Add("Capacidade de Equipamento (MVA)", typeof(double));
            tabelaLinhas.Columns.Add("Agregador 1", typeof(int));
            tabelaLinhas.Columns.Add("Agregador 2", typeof(int));
            tabelaLinhas.Columns.Add("Agregador 3", typeof(int));
            tabelaLinhas.Columns.Add("Agregador 4", typeof(int));
            tabelaLinhas.Columns.Add("Agregador 5", typeof(int));
            tabelaLinhas.Columns.Add("Agregador 6", typeof(int));
            tabelaLinhas.Columns.Add("Agregador 7", typeof(int));
            tabelaLinhas.Columns.Add("Agregador 8", typeof(int));
            tabelaLinhas.Columns.Add("Agregador 9", typeof(int));
            tabelaLinhas.Columns.Add("Agregador 10", typeof(int));


            for (i = 1; i <= DaBarra.Count; i++)
            {
                tabelaLinhas.Rows.Add(DaBarra[i], AberturaDaBarra[i], OperacaoLinha[i], AberturaParaBarra[i], ParaBarra[i], Circuito[i], EstadoLinha[i], Proprietario[i], Resistencia[i], Reatancia[i], Susceptancia[i], Tap[i], TapMinimo[i], TapMaximo[i], Defasagem[i], BarraControladaLinha[i], CapacidadeNormal[i], CapacidadeEmEmergencia[i], NumeroDeSteps[i], CapacidadeEquipamento[i], AgregadorLinha1[i], AgregadorLinha2[i], AgregadorLinha3[i], AgregadorLinha4[i], AgregadorLinha5[i], AgregadorLinha6[i], AgregadorLinha7[i], AgregadorLinha8[i], AgregadorLinha9[i], AgregadorLinha10[i]);
            }
            dataGridView2.DataSource = tabelaLinhas;
        }
    }
}
