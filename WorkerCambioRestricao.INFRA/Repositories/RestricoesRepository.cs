using Dapper;
using System.Data;
using System.Data.SqlClient;
using WorkerCambioRestricao.DOMAIN.Entities;

namespace WorkerCambioRestricao.INFRA.Repositories
{
    public class RestricoesRepository : IRestricoesRepository
    {
        private readonly string _connectionString;
        
        public RestricoesRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InserirRegistros(IEnumerable<RestricoesCSV> restricoes)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var transaction = conn.BeginTransaction();

            try
            {
                conn.Execute("DELETE FROM ListaRestricoes; " +
                    "DBCC CHECKIDENT('ListaRestricoes', RESEED, 0); ", transaction: transaction);

                foreach (var restricao in restricoes)
                {
                    conn.Execute("INSERT INTO ListaRestricoes (vchNomeCompleto, vchDocumento, vchListaOrigem) " +
                                 "VALUES (@NOME, @CPFCNPJ, @FONTE_INFORMACAO);", new
                                 {
                                     NOME = restricao.NOME,
                                     CPFCNPJ = restricao.CPFCNPJ,
                                     FONTE_INFORMACAO = restricao.FONTE_INFORMACAO
                                 }, transaction: transaction);
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw (new Exception(ex.Message));
            }

        }

        public string ObterParametroCambio(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = "SELECT vchParametro FROM ParametroCambio WHERE smlCodigo = @SmlCodigo";

            string? parametro = conn.QueryFirstOrDefault<string>(sql, new { SmlCodigo = id });

            if (parametro != null)
            {
                return parametro;
            }
            else
            {
                return "";
            }
        }

    }
}
