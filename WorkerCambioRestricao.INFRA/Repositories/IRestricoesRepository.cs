using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkerCambioRestricao.DOMAIN.Entities;

namespace WorkerCambioRestricao.INFRA.Repositories
{
    public interface IRestricoesRepository
    {
        void InserirRegistros(IEnumerable<RestricoesCSV> restricoes);
        string ObterParametroCambio(int id);
    }
}
