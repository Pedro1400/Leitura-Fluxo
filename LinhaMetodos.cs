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
    partial class Linha
    {
        public void Ler(string textlinha, StreamReader arquivo)
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
                ParaBarra[i] = Convert.ToInt32(textlinha.Substring(10, 5).Trim() == "" ? "" : textlinha.Substring(10, 5));
                Circuito[i] = Convert.ToInt32(textlinha.Substring(15, 2).Trim() == "" ? "" : textlinha.Substring(15, 2));
                EstadoLinha[i] = textlinha.Substring(17, 1).Trim() == "" ? "L" : textlinha.Substring(17, 1);
                Proprietario[i] = textlinha.Substring(18, 1).Trim() == "" ? "F" : textlinha.Substring(18, 1);
                Resistencia[i] = double.Parse(textlinha.Substring(20, 6).Trim() == "" ? "0,0" : textlinha.Substring(20, 6), CultureInfo.InvariantCulture);
                Reatancia[i] = double.Parse(textlinha.Substring(26, 6).Trim() == "" ? "" : textlinha.Substring(26, 6), CultureInfo.InvariantCulture);
                Susceptancia[i] = double.Parse(textlinha.Substring(32, 6).Trim() == "" ? "0,0" : textlinha.Substring(32, 6), CultureInfo.InvariantCulture);
                Tap[i] = double.Parse(textlinha.Substring(38, 5).Trim() == "" ? "1" : textlinha.Substring(38, 5), CultureInfo.InvariantCulture);
                TapMinimo[i] = double.Parse(textlinha.Substring(43, 5).Trim() == "" ? "0" : textlinha.Substring(43, 5), CultureInfo.InvariantCulture);
                TapMaximo[i] = double.Parse(textlinha.Substring(48, 5).Trim() == "" ? "0" : textlinha.Substring(48, 5), CultureInfo.InvariantCulture);
                Defasagem[i] = double.Parse(textlinha.Substring(53, 5).Trim() == "" ? "0,0" : textlinha.Substring(53, 5), CultureInfo.InvariantCulture);
                BarraControladaLinha[i] = Convert.ToInt32(textlinha.Substring(58, 6).Trim() == "" ? textlinha.Substring(0, 5) : textlinha.Substring(58, 6));
                CapacidadeNormal[i] = double.Parse(textlinha.Substring(64, 4).Trim() == "" ? "9999" : textlinha.Substring(64, 4), CultureInfo.InvariantCulture);
                CapacidadeEmEmergencia[i] = double.Parse(textlinha.Substring(68, 4).Trim() == "" ? "9999" : textlinha.Substring(68, 4), CultureInfo.InvariantCulture);
                NumeroDeSteps[i] = Convert.ToInt32(textlinha.Substring(72, 2).Trim() == "" ? "0" : textlinha.Substring(72, 2));
                CapacidadeEquipamento[i] = textlinha.Substring(74, 4).Trim() == "" ? CapacidadeNormal[i] : double.Parse(textlinha.Substring(74, 4), CultureInfo.InvariantCulture);
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

        public void PreencherTabela(DataGridView grade)
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
            grade.DataSource = tabelaLinhas;
        }

        public void Clear()
        {
            DaBarra.Clear(); AberturaDaBarra.Clear(); OperacaoLinha.Clear(); AberturaParaBarra.Clear(); ParaBarra.Clear();
            Circuito.Clear(); EstadoLinha.Clear(); Proprietario.Clear(); Resistencia.Clear(); Reatancia.Clear();
            Susceptancia.Clear(); Tap.Clear(); TapMinimo.Clear(); TapMaximo.Clear(); Defasagem.Clear(); BarraControladaLinha.Clear();
            CapacidadeNormal.Clear(); CapacidadeEmEmergencia.Clear(); NumeroDeSteps.Clear(); CapacidadeEquipamento.Clear();
            AgregadorLinha1.Clear(); AgregadorLinha2.Clear(); AgregadorLinha3.Clear(); AgregadorLinha4.Clear(); AgregadorLinha5.Clear();
            AgregadorLinha6.Clear(); AgregadorLinha7.Clear(); AgregadorLinha8.Clear(); AgregadorLinha9.Clear(); AgregadorLinha10.Clear();
        }
    }
}
