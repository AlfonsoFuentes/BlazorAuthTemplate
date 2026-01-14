using System.ComponentModel;

namespace Shared.Dtos.Projects.Plannings.Gantts
{
    public enum DependencyType
    {
        [Description("SS")]
        StartToStart,
        [Description("SF")]
        StartToFinish,
        [Description("FS")]
        FinishToStart,
        [Description("FF")]
        FinishToFinish
    }
   
    
}
