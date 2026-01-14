using Shared.Dtos.Projects.Plannings.Gantts;
using Shared.Enums;
using System.Text.Json.Serialization;

namespace Shared.Dtos.Projects._2._Plannings.Communications
{
    public class CommunicationDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;

        public ActionCategory Type { get; set; }
        public ArtifactType Artifact { get; set; }
        public CommunicationTrigger Trigger { get; set; }

       
     

        // 2. La propiedad Objeto con "Magia" en el Setter
       
        public GanttDto? SelectedGanttTask { get; set; }

        public int DaysOffsetOrFrequency { get; set; }

        // ✅ ÚNICA LISTA DE VERDAD
        public List<StakeHolderSimpleDto> Receivers { get; set; } = new();
    }

    public class StakeHolderSimpleDto : IEquatable<StakeHolderSimpleDto>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        // 2. Método Equals tipado (Rápido y seguro)
        public bool Equals(StakeHolderSimpleDto? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            // La clave es comparar por ID único
            return Id == other.Id;
        }

        // 3. Override de Equals genérico (Para compatibilidad con LINQ y Frameworks)
        public override bool Equals(object? obj) => Equals(obj as StakeHolderSimpleDto);

        // 4. GetHashCode es CRÍTICO para HashSets y diccionarios internos de Blazor
        public override int GetHashCode() => Id.GetHashCode();

        // 5. ToString opcional (Si no usas ToStringFunc, mostrará esto)
        public override string ToString() => $"{Name} ({Role})";
    }
    public class CreateCommunication : CommunicationDto
    {

        // Selección de IDs de los StakeHolders

    }

    // COMANDO PARA EDITAR
    public class UpdateCommunication : CreateCommunication // Hereda para reutilizar campos
    {

    }

    // COMANDO PARA BORRAR
    public class DeleteCommunication
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
    }

    // 1. Obtener TODAS las comunicaciones del Proyecto (Para la "Matriz de Comunicaciones")
    public class GetAllProjectCommunications
    {
        public Guid ProjectId { get; set; }

        public GetAllProjectCommunications() { } // Constructor vacío para serialización
        public GetAllProjectCommunications(Guid projectId)
        {
            ProjectId = projectId;
        }
    }

    // 2. Obtener una comunicación específica por ID (Para Editar/Ver Detalle)
    public class GetCommunicationById
    {
        public Guid Id { get; set; }

        public GetCommunicationById() { }
        public GetCommunicationById(Guid id)
        {
            Id = id;
        }
    }

    // 3. Obtener comunicaciones por TAREA ESPECÍFICA (Para el Tab del Gantt)
    // Este es clave para tu requerimiento de "filtrar por tarea en el timeline"
    public class GetCommunicationsByTask
    {
        public Guid GanttTaskId { get; set; }

        public GetCommunicationsByTask() { }
        public GetCommunicationsByTask(Guid ganttTaskId)
        {
            GanttTaskId = ganttTaskId;
        }
    }
    public class ValidateCommunicationName
    {
        public Guid Id { get; set; }
        public string Name { set; get; } = string.Empty;
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderCommunication
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}

