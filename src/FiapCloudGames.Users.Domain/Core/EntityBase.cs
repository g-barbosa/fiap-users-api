namespace FiapCloudGames.Users.Domain.Core
{
    /// <summary>
    /// Classe base para entidades do domínio, contendo atributos comuns de identificação e auditoria.
    /// </summary>
    public class EntityBase
    {
        /// <summary>
        /// Obtém ou define o identificador único da entidade.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Obtém ou define a data e hora de criação do registro.
        /// </summary>
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Obtém ou define a data e hora da última atualização do registro.
        /// </summary>
        public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
    }
}
