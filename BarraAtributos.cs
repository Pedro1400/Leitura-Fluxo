using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SparseCollections;

namespace Leitura
{
    partial class Barra
    {
        // Atributos lidos do arquivo PWF
        public SparseArray<int, int> NBarra = new SparseArray<int, int>();
        SparseArray<int, string> OperacaoBarra = new SparseArray<int, string>();
        SparseArray<int, string> EstadoBarra = new SparseArray<int, string>();
        public SparseArray<int, int> Tipo = new SparseArray<int, int>();
        SparseArray<int, int> GrupoDeBaseDeTensao = new SparseArray<int, int>();
        public SparseArray<int, string> Nome = new SparseArray<int, string>();
        SparseArray<int, int> GrupoDeLimiteDeTensao = new SparseArray<int, int>();
        public SparseArray<int, double> Tensao = new SparseArray<int, double>();
        public SparseArray<int, double> Angulo = new SparseArray<int, double>();
        public SparseArray<int, double> GeracaoAtiva = new SparseArray<int, double>();
        public SparseArray<int, double> GeracaoReativa = new SparseArray<int, double>();
        SparseArray<int, double> GeracaoReativaMinima = new SparseArray<int, double>();
        SparseArray<int, double> GeracaoReativaMaxima = new SparseArray<int, double>();
        SparseArray<int, int> BarraControladaBarra = new SparseArray<int, int>();
        public SparseArray<int, double> CargaAtiva = new SparseArray<int, double>();
        public SparseArray<int, double> CargaReativa = new SparseArray<int, double>();
        public SparseArray<int, double> CapacitorReator = new SparseArray<int, double>();
        SparseArray<int, int> Area = new SparseArray<int, int>();
        SparseArray<int, double> TensaoParaDefinicaoDeCarga = new SparseArray<int, double>();
        SparseArray<int, int> ModoDeVisualizacao = new SparseArray<int, int>();
        SparseArray<int, int> AgregadorBarra1 = new SparseArray<int, int>();
        SparseArray<int, int> AgregadorBarra2 = new SparseArray<int, int>();
        SparseArray<int, int> AgregadorBarra3 = new SparseArray<int, int>();
        SparseArray<int, int> AgregadorBarra4 = new SparseArray<int, int>();
        SparseArray<int, int> AgregadorBarra5 = new SparseArray<int, int>();

        // Demais Atributos
        public SparseArray<int, double> PotenciaAtivaEsperada = new SparseArray<int, double>();
        public SparseArray<int, double> PotenciaReativaEsperada = new SparseArray<int, double>();
        public int Teste;
    }
}
