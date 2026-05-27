using FiapCloudGames.Users.Domain.Usuarios.Events;

namespace FiapCloudGames.Users.Application.Usuarios.Interfaces.Messaging
{
    public interface IUsuarioEventPublisher
    {
        Task PublicarUsuarioCriadoAsync(UsuarioCriadoEvent evento);
    }
}
