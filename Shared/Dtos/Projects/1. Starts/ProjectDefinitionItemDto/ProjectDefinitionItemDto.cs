using Shared.Enums.ProjectDefinitionTypes;
using Shared.Interfaces;

namespace Shared.Dtos.ProjectDefinitions
{
    public class ProjectDefinitionItemDto  : IModelDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Order { get; set; }

        // Este campo le dice al FrontEnd si es un Objetivo, Alcance, etc.
        public ProjectDefinitionType Type { get; set; }
        
    }

    // --- Comandos (Create / Update / Delete) ---

    public class CreateProjectDefinitionItem : ProjectDefinitionItemDto
    {
        // Heredamos para tener Name, Description, ProjectId y Type
    }

    public class EditProjectDefinitionItem : ProjectDefinitionItemDto
    {
        // Heredamos para tener Id, Name, Description
    }

    public class DeleteProjectDefinitionItem
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public ProjectDefinitionType Type { get; set; } // Necesario para limpiar la caché correcta
    }

    public class DeleteGroupProjectDefinitionItem
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { get; set; }
        public ProjectDefinitionType Type { get; set; }
    }

    public class ChangeOrderProjectDefinitionItem
    {
        public Guid Id { get; set; }
        public int NewOrder { get; set; }
        public Guid ProjectId { get; set; }
        public ProjectDefinitionType Type { get; set; }
    }

    // --- Queries (Lectura) ---

    public class GetAllProjectDefinitions
    {
        public Guid ProjectId { get; set; }
        public ProjectDefinitionType Type { get; set; } // Obligatorio para filtrar
    }

    public class GetProjectDefinitionById
    {
        public Guid Id { get; set; }
    }

    // --- Validaciones ---

    public class ValidateProjectDefinitionName
    {
        public Guid Id { get; set; } // Guid.Empty si es nuevo
        public string Name { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public ProjectDefinitionType Type { get; set; } // Para validar duplicados solo en su propia lista
    }
}