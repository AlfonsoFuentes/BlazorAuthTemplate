using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.CommonEntities
{
    public class Brand : Entity
    {

        public string Name { get; set; } = string.Empty;
        



    }
    internal class BrandConfig : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.HasKey(ci => ci.Id);
            builder.HasQueryFilter(x => x.IsDeleted == false);


        }

    }

}
