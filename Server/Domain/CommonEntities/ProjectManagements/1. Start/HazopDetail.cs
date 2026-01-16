using Shared.Enums.Hazops;

namespace Server.Domain.CommonEntities.ProjectManagements
{
    public class HazopDetail  :Entity
    {
       
        public Guid HazopNodeId { get; set; }
        public HazopNode HazopNode { get; set; } = null!;
        // Parámetros técnicos (Flujo, Presión, Temperatura, etc.)
        public HazopParameter Parameter { get; set; }
        public HazopGuideWord GuideWord { get; set; }

        // Campos de análisis
        public string Deviation => $"{GuideWord} {Parameter}"; // Propiedad calculada o guardada
        public string Causes { get; set; } = string.Empty;
        public string Consequences { get; set; } = string.Empty;
        public string Safeguards { get; set; } = string.Empty; // Protecciones existentes

        // Recomendaciones técnicas para la fase de Planning
        public string Recommendations { get; set; } = string.Empty;
    }
    internal class HazopDetailConfig : IEntityTypeConfiguration<HazopDetail>
    {
        public void Configure(EntityTypeBuilder<HazopDetail> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);

           
        }

    }
}
