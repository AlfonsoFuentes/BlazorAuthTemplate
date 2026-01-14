namespace Shared.Interfaces
{
    // --- DTO Base de Lectura ---
    public interface IModelDto
    {
        Guid Id { get; set; }
        int Order { get; set; }
        bool IsEditable => true;
    }
}