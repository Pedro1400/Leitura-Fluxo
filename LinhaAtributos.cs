using SparseCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitura
{
    partial class Linha
    {
        // Variáveis de Linha
        public SparseArray<int, int> DaBarra = new SparseArray<int, int>();
        SparseArray<int, string> AberturaDaBarra = new SparseArray<int, string>();
        SparseArray<int, string> OperacaoLinha = new SparseArray<int, string>();
        SparseArray<int, string> AberturaParaBarra = new SparseArray<int, string>();
        public SparseArray<int, int> ParaBarra = new SparseArray<int, int>();
        SparseArray<int, int> Circuito = new SparseArray<int, int>();
        SparseArray<int, string> EstadoLinha = new SparseArray<int, string>();
        SparseArray<int, string> Proprietario = new SparseArray<int, string>();
        public SparseArray<int, double> Resistencia = new SparseArray<int, double>();
        public SparseArray<int, double> Reatancia = new SparseArray<int, double>();
        public SparseArray<int, double> Susceptancia = new SparseArray<int, double>();
        public SparseArray<int, double> Tap = new SparseArray<int, double>();
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
    }
}
