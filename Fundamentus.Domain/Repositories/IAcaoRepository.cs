using Fundamentus.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fundamentus.Domain.Repositories
{
    public interface IAcaoRepository
    {
        ValueTask<List<Acao>> ObterAcoes();

        ValueTask SalvarAcoes(List<Acao> acoes);
    }
}
