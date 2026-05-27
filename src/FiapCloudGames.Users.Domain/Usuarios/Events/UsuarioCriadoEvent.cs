namespace FiapCloudGames.Users.Domain.Usuarios.Events
{
    public class UsuarioCriadoEvent
    {
        public Guid Id { get; set; }
        public required string Nome { get; set; }
        public required string Email { get; set; }
    }
}
