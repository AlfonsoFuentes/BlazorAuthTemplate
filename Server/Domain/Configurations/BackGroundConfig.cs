namespace Server.Domain.Configurations
{
    internal class BackGroundConfig : IEntityTypeConfiguration<BackGround>
    {
        public void Configure(EntityTypeBuilder<BackGround> builder)
        {
            builder.HasKey(ci => ci.Id);



        }

    }
}
