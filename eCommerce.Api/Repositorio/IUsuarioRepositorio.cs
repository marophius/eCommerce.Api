using eCommerce.Api.Models;

namespace eCommerce.Api.Repositorio
{
    public interface IUsuarioRepositorio
    {
        public List<Usuario> BuscarUsuarios();
        public Usuario BuscarUsuario(int id);
        public void InsertUsuario(Usuario usuario);
        public void UpdateUsuario(Usuario usuario);
        public void DeleteUsuario(int id);
    }
}
