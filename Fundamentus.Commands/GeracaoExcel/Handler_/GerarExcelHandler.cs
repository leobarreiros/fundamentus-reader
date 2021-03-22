using ClosedXML.Excel;
using Fundamentus.Domain.Models;
using Fundamentus.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fundamentus.Commands.GeracaoExcel
{
    public class GerarExcelHandler : IRequestHandler<GerarExcelRequestDto, GerarExcelResponseDto>
    {
        private readonly IAcaoRepository _acaoRepository;
        private readonly string _hostFile;

        public GerarExcelHandler(IAcaoRepository acaoRepository)
        {
            _acaoRepository = acaoRepository;
            _hostFile = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        }

        public async Task<GerarExcelResponseDto> Handle(
            GerarExcelRequestDto request, CancellationToken cancellationToken)
        {
            var acoes = await _acaoRepository.ObterAcoes();

            return SalvarAcoesExcel(acoes);
        }

        private GerarExcelResponseDto SalvarAcoesExcel(List<Acao> acoes)
        {
            var response = new GerarExcelResponseDto();

            if (!acoes?.Any() ?? true)
            {
                response.Mensagem = "Nenhuma ação encontrada";
                return response;
            }

            using (var excel = new XLWorkbook())
            {
                var indice = 1;

                IXLWorksheet worksheet = excel.Worksheets.Add($"Ações {DateTime.Now.Year}");

                worksheet.Row(1).Style.Font.FontSize = 12;
                worksheet.Row(1).Style.Font.SetBold();
                worksheet.Row(1).Style.Font.FontColor = XLColor.White;
                worksheet.Row(1).Style.Fill.BackgroundColor = XLColor.Gray;
                worksheet.Row(1).Height = 20;
                worksheet.Column("S").Width = 21;
                worksheet.Column("R").Width = 17;
                worksheet.Column("Q").Width = 16;
                worksheet.Column("P").Width = 17;
                worksheet.Column("O").Width = 20;
                worksheet.Column("N").Width = 39;
                worksheet.Column("M").Width = 17;
                worksheet.Column("J").Width = 19;
                worksheet.Column("H").Width = 15;
                worksheet.Column("D").Width = 20;
                worksheet.Column("A").Width = 21;


                worksheet.Cell(1, 1).Value = "Nome";
                worksheet.Cell(1, 2).Value = "Classe";
                worksheet.Cell(1, 3).Value = "Código";
                worksheet.Cell(1, 4).Value = "Setor";
                worksheet.Cell(1, 5).Value = "Preço";
                worksheet.Cell(1, 6).Value = "P / L";
                worksheet.Cell(1, 7).Value = "P / PV";
                worksheet.Cell(1, 8).Value = "EV / EBITDA";
                worksheet.Cell(1, 9).Value = "LPA";
                worksheet.Cell(1, 10).Value = "Margem Líquida";
                worksheet.Cell(1, 11).Value = "ROE";
                worksheet.Cell(1, 12).Value = "ROIC";
                worksheet.Cell(1, 13).Value = "Dividend Yield";
                worksheet.Cell(1, 14).Value = "Dívida Líquida / Patrimônio Líquido";
                worksheet.Cell(1, 15).Value = "Liquidez Corrente";
                worksheet.Cell(1, 16).Value = "Margem Bruta";
                worksheet.Cell(1, 17).Value = "Margem EBIT";
                worksheet.Cell(1, 18).Value = "Dívida Líquida";
                worksheet.Cell(1, 19).Value = "Valor de Mercado";

                foreach (var acao in acoes)
                {
                    indice++;

                    worksheet.Cell(indice, 1).Value = acao.Nome;
                    worksheet.Cell(indice, 2).Value = acao.Classe;
                    worksheet.Cell(indice, 3).Value = acao.Codigo;
                    worksheet.Cell(indice, 4).Value = acao.Setor;
                    worksheet.Cell(indice, 5).Value = acao.Cotacao;
                    worksheet.Cell(indice, 6).Value = acao.PL;
                    worksheet.Cell(indice, 7).Value = acao.P_VP;
                    worksheet.Cell(indice, 8).Value = acao.EV_EBITDA;
                    worksheet.Cell(indice, 9).Value = acao.LPA;
                    worksheet.Cell(indice, 10).Value = acao.MargemLiquida;
                    worksheet.Cell(indice, 11).Value = acao.ROE;
                    worksheet.Cell(indice, 12).Value = acao.ROIC;
                    worksheet.Cell(indice, 13).Value = acao.DividendYield;
                    worksheet.Cell(indice, 14).Value = acao.DL_PatrimonioLiquido;
                    worksheet.Cell(indice, 15).Value = acao.LiquidezCorrente;
                    worksheet.Cell(indice, 16).Value = acao.MargemBruta;
                    worksheet.Cell(indice, 17).Value = acao.MargemEBIT;
                    worksheet.Cell(indice, 18).Value = acao.DividaLiquida;
                    worksheet.Cell(indice, 19).Value = acao.ValorMercado;
                }

                worksheet.SetAutoFilter();

                var diretorio = $"{_hostFile}/Excel";
                var arquivo = $"{diretorio}/indicadores.xlsx";

                if (!Directory.Exists(diretorio))
                    Directory.CreateDirectory(diretorio);

                excel.SaveAs(arquivo);

                response.Mensagem = "Arquivo gerado em:";
                response.Arquivo = Path.GetFullPath(arquivo);

                return response;
            }
        }
    }
}
