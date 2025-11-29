namespace Shared.Enums.TasksRelationTypes
{
    public class TasksRelationTypeEnum : ValueObject
    {
        public override string ToString()
        {
            return Id == -1 ? Name : $"{Name}";
        }

        public static TasksRelationTypeEnum Create(int id, string name) => new TasksRelationTypeEnum() { Id = id, Name = name };
        public static TasksRelationTypeEnum None = Create(-1, "None");
        public static TasksRelationTypeEnum FinishStart = Create(0, "FS");// Una tarea comienza cuando la anterior termina
        public static TasksRelationTypeEnum StartStart = Create(1, "SS"); //Ambas tareas comienzan al mismo tiempo
        public static TasksRelationTypeEnum StartFinish = Create(2, "SF");//Una tarea debe iniciarse para que otra termine
        
        public static TasksRelationTypeEnum FinishFinish = Create(3, "FF");//Ambas tareas terminan al mismo tiempo


        public static List<TasksRelationTypeEnum> List = new List<TasksRelationTypeEnum>()
        {
            None, FinishStart, StartStart, StartFinish, FinishFinish

        };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;

        public static TasksRelationTypeEnum GetType(string type) => List.Exists(x => x.Name.Equals(type)) ?
            List.FirstOrDefault(x => x.Name.Equals(type))! : None;

        public static TasksRelationTypeEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;

    }
}