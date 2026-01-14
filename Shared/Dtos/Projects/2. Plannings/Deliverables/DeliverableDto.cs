using Shared.Dtos.Projects.Plannings.Resources;

namespace Shared.Dtos.Projects.Plannings.Deliverables
{
    public class DeliverableDto
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;

    }
    public class CreateDeliverable : DeliverableDto
    {

    }
    public class EditDeliverable : DeliverableDto
    {

    }
    public class GetAllDeliverables
    {
        public Guid ProjectId { get; set; }
    }
    public class GetDeliverableById
    {
        public Guid Id { set; get; }
    }
    public class DeleteDeliverable
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderDeliverable
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
    public class ValidateDeliverableName
    {
        public Guid Id { set; get; }
        public string Name { set; get; } = string.Empty;
        public Guid ProjectId { set; get; }
    }
}
