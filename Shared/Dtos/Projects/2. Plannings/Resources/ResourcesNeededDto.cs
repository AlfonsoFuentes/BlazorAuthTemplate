namespace Shared.Dtos.Projects.Plannings.Resources
{
    public class ResourcesNeededDto
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Speciality { get; set; } = string.Empty;
    }
    public class CreateResourcesNeeded : ResourcesNeededDto
    {

    }
    public class EditResourcesNeeded : ResourcesNeededDto
    {

    }
    public class GetAllResourcesNeeded
    {
        public Guid ProjectId { get; set; }
    }
    public class GetResourcesNeededById
    {
        public Guid Id { set; get; }
    }
    public class DeleteResourcesNeeded
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderResourcesNeeded
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
