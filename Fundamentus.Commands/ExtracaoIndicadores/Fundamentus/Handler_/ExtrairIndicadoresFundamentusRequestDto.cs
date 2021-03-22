using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fundamentus.Commands.ExtracaoIndicadores.Fundamentus
{
    public class ExtrairIndicadoresFundamentusRequestDto : IRequest<ExtrairIndicadoresFundamentusResponseDto>
    {
        public IEnumerable<string> CodigosAcao { get; set; }
    }
}
