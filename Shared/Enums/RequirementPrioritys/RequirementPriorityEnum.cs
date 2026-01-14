using Shared.Enums.ProjectNeedTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shared.Enums.RequirementPrioritys
{
    public class RequirementPriorityEnum : ValueObject
    {
        public static RequirementPriorityEnum Create(int id, string name, string color) => new RequirementPriorityEnum() { Id = id, Name = name, Color = color };


        public string Color { get; private set; } = string.Empty;

        // Definición de valores
        public static readonly RequirementPriorityEnum None = Create(-1, "None", "#BDBDBD");
        public static readonly RequirementPriorityEnum Low = Create(1, "Low", "#4CAF50");
        public static readonly RequirementPriorityEnum Medium = Create(2, "Medium", "#FF9800");
        public static readonly RequirementPriorityEnum High = Create(3, "High", "#F44336");   // Rojo
        public static readonly RequirementPriorityEnum Critical = Create(4, "Critical", "#212121"); // Negro
        // Operadores para facilidad de uso
        public static implicit operator string(RequirementPriorityEnum e) => e.Name;
        public static implicit operator RequirementPriorityEnum(string name) => GetType(name);

        public static List<RequirementPriorityEnum> List = new List<RequirementPriorityEnum>()
            {
            None,Low, Medium, High, Critical
            };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;
        public static RequirementPriorityEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;

        public static RequirementPriorityEnum GetType(string name) => List.Exists(x => x.Name == name) ? List.FirstOrDefault(x => x.Name == name)!
            : None;
    }

    public class RequirementTypeEnum : ValueObject
    {
        public static RequirementTypeEnum Create(int id, string name) => new RequirementTypeEnum() { Id = id, Name = name };
        public static readonly RequirementTypeEnum None = Create(0, "None");    // Verde
        public static readonly RequirementTypeEnum Business = Create(1, "Business");
        public static readonly RequirementTypeEnum Stakeholder = Create(2, "Stakeholder");
        public static readonly RequirementTypeEnum Functional = Create(3, "Functional");
        public static readonly RequirementTypeEnum NonFunctional = Create(4, "Non-Functional");
        public static readonly RequirementTypeEnum Transition = Create(5, "Transition");
        public static readonly RequirementTypeEnum Quality = Create(6, "Quality");

        public static implicit operator string(RequirementTypeEnum e) => e.Name;
        public static implicit operator RequirementTypeEnum(string name) => GetType(name);

        public static List<RequirementTypeEnum> List = new List<RequirementTypeEnum>()
            {
            None, Business, Stakeholder, Functional, NonFunctional, Transition, Quality
            };
        public static string GetName(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)!.Name : string.Empty;
        public static RequirementTypeEnum GetType(int id) => List.Exists(x => x.Id == id) ? List.FirstOrDefault(x => x.Id == id)! : None;

        public static RequirementTypeEnum GetType(string name) => List.Exists(x => x.Name == name) ? List.FirstOrDefault(x => x.Name == name)!
            : None;
    }


}
