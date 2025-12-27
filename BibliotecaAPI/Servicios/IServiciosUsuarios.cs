using BibliotecaAPI.Entidades;

namespace BibliotecaAPI.Servicios
{
    public interface IServiciosUsuarios
    {
        Task<Usuario?> ObtenerUsuarioActual();
       
    }
}