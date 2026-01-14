using Shared.Dtos.General;

namespace Shared.Dtos.Starts.LearnedLessonsByProjects
{
    public class LearnedLessonsByProjectDto : GeneralDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order {  get; set; }
        public Guid ProjectId { get; set; }
       
    }
    public class CreateLearnedLessonsByProject     : LearnedLessonsByProjectDto
    {
        
    }
    public class EditLearnedLessonsByProject : LearnedLessonsByProjectDto
    {

    }
    public class GetAllLearnedLessonsByProjects
    {
          public Guid ProjectId { get; set; }
    }
    public class GetLearnedLessonsByProjectById
    {
        public Guid Id {  set; get; }
    }
    public class ValidateLearnedLessonsByProjectName
    {
        public Guid Id { set; get; }
        public string Name {  set; get; }    = string.Empty;
        public Guid ProjectId { set; get; } 
    }
    
    public class DeleteLearnedLessonsByProject
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupLearnedLessonsByProject
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderLearnedLessonsByProject
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
