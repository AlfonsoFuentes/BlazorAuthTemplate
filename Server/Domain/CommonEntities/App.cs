namespace Server.Domain.CommonEntities
{
    public class App : Entity, ITennant
    {
        public override bool IsTennanted => true;

        public Guid CurrentProjectId { get; set; }
        public string TenantId { get; set; } = string.Empty;

        public static App Create()
        {
            return new App()
            {
                Id = Guid.NewGuid(),
            };

        }
    }


}
