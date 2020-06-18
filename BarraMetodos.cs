using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Leitura
{
    partial class Barra
    {
        public void Ler(string textlinha, StreamReader arquivo)
        {
            textlinha = arquivo.ReadLine();
            int i = 1;

            try
            {
                while ((textlinha = arquivo.ReadLine()) != "99999")
                {//Atribue a uma variavel os dados da barra
                    textlinha = textlinha.PadRight(textlinha.Length + 100, ' ');
                    NBarra[i] = Convert.ToInt32(textlinha.Substring(0, 5));
                    OperacaoBarra[i] = textlinha.Substring(5, 1).Trim() == "" ? "A" : textlinha.Substring(5, 1);
                    EstadoBarra[i] = textlinha.Substring(6, 1).Trim() == "" ? "L" : textlinha.Substring(6, 1);
                    Tipo[i] = Convert.ToInt32(textlinha.Substring(7, 1).Trim() == "" ? "0" : textlinha.Substring(7, 1));
                    GrupoDeBaseDeTensao[i] = Convert.ToInt32(textlinha.Substring(8, 2).Trim() == "" ? "0" : textlinha.Substring(8, 2));
                    Nome[i] = textlinha.Substring(10, 12).Trim() == "" ? "" : textlinha.Substring(10, 12);
                    GrupoDeLimiteDeTensao[i] = Convert.ToInt32(textlinha.Substring(22, 2).Trim() == "" ? "0" : textlinha.Substring(22, 2));
                    Tensao[i] = double.Parse(textlinha.Substring(24, 4).Trim() == "" ? "1,0" : textlinha.Substring(24, 4), CultureInfo.InvariantCulture) / 1000;
                    Angulo[i] = double.Parse(textlinha.Substring(28, 4).Trim() == "" ? "0,0" : textlinha.Substring(28, 4), CultureInfo.InvariantCulture);
                    GeracaoAtiva[i] = double.Parse(textlinha.Substring(32, 5).Trim() == "" ? "0,0" : textlinha.Substring(32, 5), CultureInfo.InvariantCulture);
                    GeracaoReativa[i] = double.Parse(textlinha.Substring(37, 5).Trim() == "" ? "0,0" : textlinha.Substring(37, 5), CultureInfo.InvariantCulture);
                    GeracaoReativaMinima[i] = double.Parse(textlinha.Substring(42, 5).Trim() == "" ? "0,0" : textlinha.Substring(42, 5), CultureInfo.InvariantCulture);
                    GeracaoReativaMaxima[i] = double.Parse(textlinha.Substring(47, 5).Trim() == "" ? "0,0" : textlinha.Substring(47, 5), CultureInfo.InvariantCulture);
                    BarraControladaBarra[i] = Convert.ToInt32(textlinha.Substring(52, 6).Trim() == "" ? textlinha.Substring(0, 5) : textlinha.Substring(52, 6));
                    CargaAtiva[i] = double.Parse(textlinha.Substring(58, 5).Trim() == "" ? "0,0" : textlinha.Substring(58, 5), CultureInfo.InvariantCulture);
                    CargaReativa[i] = double.Parse(textlinha.Substring(63, 5).Trim() == "" ? "0,0" : textlinha.Substring(63, 5), CultureInfo.InvariantCulture);
                    CapacitorReator[i] = double.Parse(textlinha.Substring(68, 5).Trim() == "" ? "0,0" : textlinha.Substring(68, 5), CultureInfo.InvariantCulture);
                    Area[i] = Convert.ToInt32(textlinha.Substring(73, 3).Trim() == "" ? "1" : textlinha.Substring(73, 3));
                    TensaoParaDefinicaoDeCarga[i] = double.Parse(textlinha.Substring(76, 4).Trim() == "" ? "1,0" : textlinha.Substring(76, 4), CultureInfo.InvariantCulture)/1000;
                    ModoDeVisualizacao[i] = Convert.ToInt32(textlinha.Substring(80, 1).Trim() == "" ? "0" : textlinha.Substring(80, 1));
                    AgregadorBarra1[i] = Convert.ToInt32(textlinha.Substring(81, 3).Trim() == "" ? "0" : textlinha.Substring(81, 3));
                    AgregadorBarra2[i] = Convert.ToInt32(textlinha.Substring(84, 3).Trim() == "" ? "0" : textlinha.Substring(84, 3));
                    AgregadorBarra3[i] = Convert.ToInt32(textlinha.Substring(87, 3).Trim() == "" ? "0" : textlinha.Substring(87, 3));
                    AgregadorBarra4[i] = Convert.ToInt32(textlinha.Substring(90, 3).Trim() == "" ? "0" : textlinha.Substring(90, 3));
                    AgregadorBarra5[i] = Convert.ToInt32(textlinha.Substring(93, 3).Trim() == "" ? "0" : textlinha.Substring(93, 3));

                    PotenciaAtivaEsperada[i] = GeracaoAtiva[i] - CargaAtiva[i];
                    PotenciaReativaEsperada[i] = GeracaoReativa[i] - CargaReativa[i];

                    i++;
                }
            }
            catch(FormatException)
            {
                MessageBox.Show(string.Concat("Algum parâmetro da linha ", i.ToString(), " do DBAR não está no formato adequado." +
                    "\n Por favor, verifique o arquivo .PWF"), "Erro na leitura dos Dados de Barra", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Clear();
            }
        }

        public void PreencherTabela(DataGridView grade)
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
            grade.DataSource = tabelaBarras;
        }

        public void Clear()
        {
            NBarra.Clear(); OperacaoBarra.Clear(); EstadoBarra.Clear(); Tipo.Clear(); GrupoDeBaseDeTensao.Clear();
            Nome.Clear(); GrupoDeLimiteDeTensao.Clear(); Tensao.Clear(); Angulo.Clear(); GeracaoAtiva.Clear();
            GeracaoReativa.Clear(); CargaAtiva.Clear(); CargaReativa.Clear(); GeracaoReativaMinima.Clear();
            GeracaoReativaMaxima.Clear(); BarraControladaBarra.Clear(); CapacitorReator.Clear(); Area.Clear();
            TensaoParaDefinicaoDeCarga.Clear(); ModoDeVisualizacao.Clear(); AgregadorBarra1.Clear(); AgregadorBarra2.Clear();
            AgregadorBarra3.Clear(); AgregadorBarra4.Clear(); AgregadorBarra5.Clear();
        }

    }
}
