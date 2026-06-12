namespace Monorepo.Domain.Commons.Entities.Audit;

/// <summary>
/// Classe base para entidades que devem ser auditadas.
/// Define as colunas de auditoria com anotações precisas para corresponder ao schema do banco de dados existente.
/// </summary>
public abstract record AuditableEntity
{
    public required string UsuarioAtualizacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public required string Maquina { get; set; }
    public required string Login { get; set; }
}