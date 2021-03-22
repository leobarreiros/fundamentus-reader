using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fundamentus.Commands.GeracaoExcel
{
    public class GerarExcelRequestDto : IRequest<GerarExcelResponseDto>
    {
    }
}
