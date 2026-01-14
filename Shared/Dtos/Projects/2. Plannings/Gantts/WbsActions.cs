using Shared.Dtos.Projects._2._Plannings.Gantts;

namespace Shared.Dtos.Projects.Plannings.Gantts
{

    public static class WbsActions
    {
        // ✅ Mover arriba
        public static bool CanMoveUp(GanttDto task, List<GanttDto> allTasks)
        {
            if (task == null || allTasks == null) return false;
            var siblings = GetSiblings(task, allTasks);
            var index = siblings.FindIndex(t => t.Id == task.Id); // ✅ por Id
            return index > 0;
        }

        // ✅ Mover abajo
        public static bool CanMoveDown(GanttDto task, List<GanttDto> allTasks)
        {
            if (task == null || allTasks == null) return false;
            var siblings = GetSiblings(task, allTasks);
            var index = siblings.FindIndex(t => t.Id == task.Id); // ✅ por Id
            return index >= 0 && index < siblings.Count - 1;
        }

        // ✅ Indent izquierda
        public static bool CanIndentLeft(GanttDto task) =>
            task?.ParentId.HasValue == true;

        // ✅ Indent derecha (CORREGIDO: orden jerárquico exacto como en GanttView)
        public static bool CanIndentRight(GanttDto task, List<GanttDto> allTasks)
        {
            if (task == null || allTasks == null) return false;
            return GetIndentRightTarget(task, allTasks) != null;
        }

        // --- Métodos privados ---
        private static List<GanttDto> GetSiblings(GanttDto task, List<GanttDto> allTasks) =>
            allTasks
                .Where(t => t.ParentId == task.ParentId)
                .OrderBy(t => t.Order)
                .ToList();

        public static GanttDto? GetIndentRightTarget(GanttDto task, List<GanttDto> allTasks)
        {
            if (task == null || allTasks == null) return null;

            // ✅ Orden jerárquico idéntico al de GanttView (TopologicalSortDtos)
            var ordered = GanttCalculatorV3.TopologicalSortDtos(allTasks);
            var index = ordered.FindIndex(t => t.Id == task.Id);
            if (index <= 0) return null;

            var taskLevel = task.WbsCode.Count(c => c == '.');

            // 🔹 Buscar hacia atrás la PRIMERA tarea con el MISMO nivel
            for (int i = index - 1; i >= 0; i--)
            {
                var candidate = ordered[i];
                var candLevel = candidate.WbsCode.Count(c => c == '.');

                if (candLevel == taskLevel)
                {
                    // ✅ Evitar ciclos (como en GanttView)
                    if (!IsAncestor(task, candidate, allTasks))
                        return candidate;
                }
            }

            return null;
        }

        private static bool IsAncestor(GanttDto child, GanttDto ancestor, List<GanttDto> allTasks)
        {
            var current = child;
            while (current.ParentId.HasValue)
            {
                if (current.ParentId == ancestor.Id) return true;
                current = allTasks.FirstOrDefault(t => t.Id == current.ParentId);
                if (current == null) break;
            }
            return false;
        }
    }
}
