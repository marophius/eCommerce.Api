using System.Data;

namespace eCommerce.Api.Repositorio
{
    public class UsuarioProcedureRepository
    {
        private IDbConnection _connection;

        public UsuarioProcedureRepository(IDbConnection connection)
        {
            _connection = connection;
        }
    }
}
