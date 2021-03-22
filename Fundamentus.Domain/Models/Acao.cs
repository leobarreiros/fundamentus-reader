using System;
using System.Collections.Generic;
using System.Text;

namespace Fundamentus.Domain.Models
{
    public class Acao
    {
        public string Nome { get; set; }
        public string Codigo { get; set; }
        public string Classe { get; set; }
        public decimal Cotacao { get; set; }
        public string Setor { get; set; }
        public decimal ValorMercado { get; set; }
        public decimal ValorFirma { get; set; }
        public long QuantidadeAcoes { get; set; }
        public double PatrimonioLiquido { get; set; }
        public double EBIT { get; set; }

        #region All Rounders

        public decimal PL { get; set; }
        public decimal P_VP { get; set; }
        public decimal VPA { get; set; }
        public decimal LPA { get; set; }
        public decimal DividendYield { get; set; }
        public decimal LiquidezCorrente { get; set; }

        #endregion

        #region Lucratividade

        public decimal MargemLiquida { get; set; }
        public decimal MargemBruta { get; set; }
        public decimal MargemEBIT { get; set; }
        public decimal ROE { get; set; }
        public decimal ROIC { get; set; }

        #endregion

        #region Crescimento

        public decimal CAGR { get; set; }

        public decimal PegRatio => CAGR != 0 && PL != 0 ? 
            Math.Round(PL / CAGR, 2) : 0;

        #endregion

        #region Endividamento

        public double DL_EBIT => DividaLiquida != 0 && EBIT != 0 ?
            Math.Round(DividaLiquida / EBIT, 2) : 0;

        public double DL_PatrimonioLiquido => DividaLiquida != 0 && PatrimonioLiquido != 0 ?
            Math.Round(DividaLiquida / PatrimonioLiquido, 2) : 0;

        public decimal EV_EBITDA { get; set; }

        public double DividaBruta { get; set; }
        public double DividaLiquida { get; set; }

        #endregion
    }
}
