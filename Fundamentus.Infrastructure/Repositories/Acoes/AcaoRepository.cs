using Fundamentus.Domain.Models;
using Fundamentus.Domain.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;

namespace Fundamentus.Infrastructure.Repositories.Acoes
{
    public class AcaoRepository : IAcaoRepository
    {        
        private readonly string _diretorioCache;
        private readonly string _arquivo;

        public AcaoRepository()
        {
            var hostFile = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            _diretorioCache = $"{hostFile}/CacheSites";
            _arquivo = $"{_diretorioCache}/Acoes.json";
        }

        public ValueTask<List<Acao>> ObterAcoes()
        {
            if (System.IO.Directory.Exists(_diretorioCache) && 
                System.IO.File.Exists(_arquivo))
            {
                var jsonSettings = new JsonSerializerSettings()
                {
                    Error = new EventHandler<ErrorEventArgs>((object sender, ErrorEventArgs errorArgs) =>
                    {
                        errorArgs.ErrorContext.Handled = true;
                    })
                };

                var acoes = JsonConvert.DeserializeObject<List<Acao>>(System.IO.File.ReadAllText(_arquivo));
                return new ValueTask<List<Acao>>(acoes);
            }

            return new ValueTask<List<Acao>>();
        }

        public ValueTask SalvarAcoes(List<Acao> acoes)
        {
            if (!System.IO.Directory.Exists(_diretorioCache))
                System.IO.Directory.CreateDirectory(_diretorioCache);

            var json = JsonConvert.SerializeObject(acoes);

            System.IO.File.WriteAllText(_arquivo, json);

            return new ValueTask();
        }
    }
}
