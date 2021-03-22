using Fundamentus.Commands.ExtracaoIndicadores.Fundamentus;
using Fundamentus.Commands.GeracaoExcel;
using Fundamentus.Domain.Repositories;
using Fundamentus.Infrastructure.Repositories.Acoes;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;


namespace Fundamentus.Infrastructure.DI
{
    public static class ServiceCollectionExtension
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAcaoRepository>(p =>
                new AcaoRepository());
        }

        public static void AddHandlers(this IServiceCollection services)
        {
            services.AddMediatR(typeof(ExtrairIndicadoresFundamentusHandler).Assembly);

            services.AddScoped<IRequestHandler<ExtrairIndicadoresFundamentusRequestDto, ExtrairIndicadoresFundamentusResponseDto>>(p =>
                 new ExtrairIndicadoresFundamentusHandler(
                     p.GetRequiredService<IAcaoRepository>()));

            services.AddScoped<IRequestHandler<GerarExcelRequestDto, GerarExcelResponseDto>>(p =>
                 new GerarExcelHandler(
                     p.GetRequiredService<IAcaoRepository>()));
        }
    }
}
