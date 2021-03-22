using ClosedXML.Excel;
using Fundamentus.Domain.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Fundamentus.Infrastructure.DI;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Fundamentus.Commands.ExtracaoIndicadores.Fundamentus;
using Fundamentus.Commands.GeracaoExcel;
using System.Diagnostics;

namespace Fundamentus.AppReader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            await host.RunAsync();            
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(_ =>
                {
                    var serviceCollection = new ServiceCollection();
                    serviceCollection.AddRepositories();
                    serviceCollection.AddHandlers();        
                    var serviceProvider = serviceCollection.BuildServiceProvider();

                    var mediador = serviceProvider.GetService<IMediator>();

                    IniciarApp(mediador);
                });

        static void IniciarApp(IMediator mediador)
        {
            CultureInfo.CreateSpecificCulture("pt-BR");
            var hostFile = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var config = new ConfigurationBuilder().AddJsonFile($"{hostFile}/appsettings.json", true, true).Build();
            var codigosAcoes = config.GetSection("acoes").GetChildren().ToArray().Select(c => c.Value);
            var executarAcao = true;

            

            do
            {
                DateTime? ultimaLeituraAcoes = null; 

                if (File.Exists($"{hostFile}/CacheSites/Acoes.json"))
                    ultimaLeituraAcoes = File.GetLastWriteTime($"{hostFile}/CacheSites/Acoes.json");

                Console.WriteLine("");
                Console.WriteLine("Digite o número de um dos comandos abaixo para iniciar:");
                Console.Write($"1. Ler indicadores ações site Fundamentus");
                if (ultimaLeituraAcoes.HasValue)
                    Console.Write($" (última leitura em {ultimaLeituraAcoes.Value})");
                Console.WriteLine("");
                Console.WriteLine("2. Gerar arquivo Excel");
                Console.WriteLine("3. Sair");
                var input = Console.ReadLine();
                Console.WriteLine("");

                switch (input)
                {
                    case "1":
                        Console.WriteLine("Iniciando leitura dos indicados, aguarde por favor.");
                        Console.WriteLine("");
                        mediador.Send(new ExtrairIndicadoresFundamentusRequestDto() { CodigosAcao = codigosAcoes }).GetAwaiter().GetResult();
                        break;
                    case "2":
                        Console.WriteLine("Arquivo excel sendo gerado, aguarde por favor.");
                        Console.WriteLine("");
                        var resultado = mediador.Send(new GerarExcelRequestDto()).GetAwaiter().GetResult();

                        Console.WriteLine(resultado.Mensagem);
                        Console.WriteLine(resultado.Arquivo);

                        break;
                    case "3":
                        Environment.Exit(-1);
                        break;
                    default:
                        Console.WriteLine("Comando não reconhecido!");
                        break;
                }

            } while (executarAcao);
        }
    }
}
