using Shared.Dtos.StakeHolders;
using Shared.Enums.StakeHolderTypes;
using Shared.Interfaces;

namespace Shared.Dtos.Starts.StakeHolderInsideProjectInsideProjects
{
    public class StakeHolderInsideProjectDto : IHasId
    {
        public Guid Id { get; set; }
        public StakeHolderDto? StakeHolderInsideProject { get; set; } = null!;
        public Guid StakeHolderId => StakeHolderInsideProject == null ? Guid.Empty : StakeHolderInsideProject.Id;
        public Guid ProjectId { get; set; }
        public StakeHolderRoleEnum Role { get; set; } = StakeHolderRoleEnum.None;
        public string Name => StakeHolderInsideProject == null ? string.Empty : StakeHolderInsideProject.Name;
        public string Area => StakeHolderInsideProject == null ? string.Empty : StakeHolderInsideProject.Area;
        public string Email => StakeHolderInsideProject == null ? string.Empty : StakeHolderInsideProject.Email;
        public string PhoneNumber => StakeHolderInsideProject == null ? string.Empty : StakeHolderInsideProject.PhoneNumber;
    }
    public class CreateStakeHolderInsideProject : StakeHolderInsideProjectDto
    {

    }
    public class EditStakeHolderInsideProject : StakeHolderInsideProjectDto
    {

    }
    public class GetAllStakeHolderInsideProjects
    {
        public Guid ProjectId { get; set; }
    }
    public class GetStakeHolderInsideProjectById
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
        public Guid StakeHolderId { set; get; }
    }
    public class ValidateStakeHolderInsideProjectName
    {
        public Guid Id { set; get; }
        public string Name { set; get; } = string.Empty;
        public Guid ProjectId { set; get; }
        public Guid StakeHolderId { set; get; }
    }

    public class DeleteStakeHolderInsideProject
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
        public Guid StakeHolderId { set; get; }
    }
}
