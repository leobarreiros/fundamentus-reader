using Fundamentus.Domain.Models;
using Fundamentus.Domain.Repositories;
using HtmlAgilityPack;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fundamentus.Commands.ExtracaoIndicadores.Fundamentus
{
    public class ExtrairIndicadoresFundamentusHandler :
        IRequestHandler<ExtrairIndicadoresFundamentusRequestDto, ExtrairIndicadoresFundamentusResponseDto>
    {
        private readonly IAcaoRepository _acaoRepository;
        public ExtrairIndicadoresFundamentusHandler(IAcaoRepository acaoRepository)
        {
            _acaoRepository = acaoRepository;
        }

        public async Task<ExtrairIndicadoresFundamentusResponseDto> Handle(
            ExtrairIndicadoresFundamentusRequestDto request, 
            CancellationToken cancellationToken)
        {
            var acoesLidas = await ExtrairIndicadoresFundamentus(request.CodigosAcao);

            if (acoesLidas.Any())
            {
                await _acaoRepository.SalvarAcoes(acoesLidas);

                Console.WriteLine("");
                Console.WriteLine("Indicadores salvos no app!");
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("Nenhuma indicador foi salvo");
            }

            return new ExtrairIndicadoresFundamentusResponseDto();
        }

        private async ValueTask<List<Acao>> ExtrairIndicadoresFundamentus(IEnumerable<string> codigos)
        {
            var acoes = new List<Acao>();

            foreach (var codigo in codigos)
            {
                var htmlString = await LerHtmlSiteFundamentus(codigo);
                var pageDocument = new HtmlDocument();

                pageDocument.LoadHtml(htmlString);

                var acao = LerIndicadoresFundamentus(pageDocument);

                if (acao != null)
                {
                    acoes.Add(acao);
                    Console.WriteLine($"Indicadores {codigo} extraidos.");
                }
                else
                    Console.WriteLine($"{codigo} não localizada.");
            }

            return acoes;
        }

        private Acao LerIndicadoresFundamentus(HtmlDocument documento)
        {
            var acao = new Acao();
            var nodes = documento.DocumentNode.SelectNodes("//span[@class='txt']");

            if (nodes == null || nodes.Count < 106)
                return null;

            decimal.TryParse(nodes[3].InnerText, NumberStyles.Any, new CultureInfo("pt-BR"), out decimal cotacao);
            decimal.TryParse(nodes[21].InnerText, NumberStyles.Any, new CultureInfo("pt-BR"), out decimal valorMercado);
            decimal.TryParse(nodes[25].InnerText, NumberStyles.Any, new CultureInfo("pt-BR"), out decimal valorFirma);
            long.TryParse(nodes[27].InnerText, NumberStyles.Any, new CultureInfo("pt-BR"), out long qtd);
            decimal.TryParse(nodes[32].InnerText, NumberStyles.Any, new CultureInfo("pt-BR"), out decimal PL);
            decimal.TryParse(nodes[34].InnerText, NumberStyles.Any, new CultureInfo("pt-BR"), out decimal LPA);
            decimal.TryParse(nodes[37].InnerText, NumberStyles.Any, new CultureInfo("pt-BR"), out decimal PVP);
            decimal.TryParse(nodes[39].InnerText, NumberStyles.Any, new CultureInfo("pt-BR"), out decimal VPA);
            decimal.TryParse(RemoverCaracteresEspeciais(nodes[44].InnerText), NumberStyles.Any, new CultureInfo("pt-BR"), out decimal margemBruta);
            decimal.TryParse(RemoverCaracteresEspeciais(nodes[49].InnerText), NumberStyles.Any, new CultureInfo("pt-BR"), out decimal margemEBIT);
            decimal.TryParse(RemoverCaracteresEspeciais(nodes[54].InnerText), NumberStyles.Any, new CultureInfo("pt-BR"), out decimal margemLiquida);
            decimal.TryParse(RemoverCaracteresEspeciais(nodes[64].InnerText), NumberStyles.Any, new CultureInfo("pt-BR"), out decimal ROIC);
            decimal.TryParse(RemoverCaracteresEspeciais(nodes[69].InnerText), NumberStyles.Any, new CultureInfo("pt-BR"), out decimal ROE);
            decimal.TryParse(RemoverCaracteresEspeciais(nodes[67].InnerText), NumberStyles.Any, new CultureInfo("pt-BR"), out decimal dividendYield);
            decimal.TryParse(RemoverCaracteresEspeciais(nodes[72].InnerText), NumberStyles.Any, new CultureInfo("pt-BR"), out decimal EvEbitda);
            decimal.TryParse(RemoverCaracteresEspeciais(nodes[74].InnerText), NumberStyles.Any, new CultureInfo("pt-BR"), out decimal liquidezCorrente);
            double.TryParse(nodes[89].InnerText, NumberStyles.Any, new CultureInfo("pt-BR"), out double divBruta);
            double.TryParse(nodes[93].InnerText, NumberStyles.Any, new CultureInfo("pt-BR"), out double divLiquida);
            double.TryParse(nodes[97].InnerText, NumberStyles.Any, new CultureInfo("pt-BR"), out double patrimonioLiquido);
            double.TryParse(nodes[106].InnerText, NumberStyles.Any, new CultureInfo("pt-BR"), out double EBIT);

            if (divBruta == 0)
                divLiquida = 0;

            if (patrimonioLiquido == 0)
            {
                double.TryParse(nodes[93].InnerText, NumberStyles.Any, new CultureInfo("pt-BR"), out patrimonioLiquido);
            }

            acao.ValorMercado = valorMercado;
            acao.ValorFirma = valorFirma;
            acao.QuantidadeAcoes = qtd;

            acao.Codigo = nodes[1].InnerText;
            acao.Cotacao = cotacao;
            acao.Classe = nodes[5].InnerText;
            acao.Nome = nodes[9].InnerText.Replace(acao.Classe, "");
            acao.Setor = nodes[13].InnerText;

            acao.PL = PL;
            acao.LPA = LPA;
            acao.VPA = VPA;
            acao.P_VP = PVP;
            acao.ROIC = ROIC;
            acao.ROE = ROE;
            acao.DividendYield = dividendYield;
            acao.LiquidezCorrente = liquidezCorrente;
            acao.MargemLiquida = margemLiquida;
            acao.MargemBruta = margemBruta;
            acao.MargemEBIT = margemEBIT;
            acao.EV_EBITDA = EvEbitda;

            acao.DividaBruta = divBruta;
            acao.DividaLiquida = divLiquida;
            acao.EBIT = EBIT;
            acao.PatrimonioLiquido = patrimonioLiquido;

            return acao;
        }

        private string RemoverCaracteresEspeciais(string texto) =>
            texto.Replace("\n", "").Replace("%", "");

        private async ValueTask<string> LerHtmlSiteFundamentus(string acao)
        {
            string Url = "http://www.fundamentus.com.br/detalhes.php?papel=" + acao.ToUpper();
            var cookieJar = new CookieContainer();
            var request = (HttpWebRequest)WebRequest.Create(Url);
            request.ContentType = "text/html; charset=utf-8";
            request.CookieContainer = cookieJar;
            request.Accept = @"text/html, application/xhtml+xml, */*";
            request.Referer = Url;
            request.Headers.Add("Accept-Language", "pt-BR");
            request.UserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)";
            request.Host = @"www.fundamentus.com.br";

            var response = await request.GetResponseAsync();
            Encoding CorrectEncoding = Encoding.GetEncoding("iso-8859-1");

            using (var reader = new StreamReader(response.GetResponseStream(), CorrectEncoding))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
