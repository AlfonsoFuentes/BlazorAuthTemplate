using Shared.Dtos.General;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Dtos.Projects
{
    public class ProjectDto
    {

    }



    public class CreateProject : ProjectDto
    {
    }
   
    
    public class ValidateProjectName
    {
        public Guid Id { set; get; }
        public string Name { set; get; } = string.Empty;
    }
    
    public class DeleteProject
    {
        public Guid Id { set; get; }
    }
    
}
