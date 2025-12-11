using Shared.Interfaces;

namespace Shared.Dtos.StakeHolders
{
    public class StakeHolderDto : IHasId
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Name {  get; set; } = string.Empty;
    }
    public class CreateStakeHolder : StakeHolderDto
    {

    }
    public class EditStakeHolder : StakeHolderDto
    {

    }
    public class GetAllStakeHolders
    {
        public Guid ProjectId { get; set; }
    }
    public class GetStakeHolderById
    {
        public Guid Id { set; get; }
    }
    public class ValidateStakeHolderName
    {
        public Guid Id { set; get; }
        public string Name { set; get; } = string.Empty;
        public Guid ProjectId { set; get; }
    }
    public class ValidateStakeHolderEmail
    {
        public Guid Id { set; get; }
        public string Email { set; get; } = string.Empty;
        public Guid ProjectId { set; get; }
    }

    public class DeleteStakeHolder
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
}
