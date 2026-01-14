using Shared.Dtos.Projects.Plannings.Gantts;
using Shared.ExtensionsMethods;

namespace Shared.Dtos.Projects._2._Plannings.Gantts
{

    public static class GanttCalculatorV3
    {
        // ============================================================================================
        // 1. MOTOR PRINCIPAL (Estructura basada en V2 para velocidad, lógica de V1)
        // ============================================================================================
        public static List<GanttDto> RecalculateAllTasks(this List<GanttDto> dtos)
        {
            if (dtos == null || !dtos.Any()) return new();

            // 1. OPTIMIZACIÓN (V2): Crear diccionarios para no usar .Where() en bucles
            var dtoDict = dtos.ToDictionary(d => d.Id);
            var childrenLookup = dtos.Where(d => d.ParentId.HasValue).ToLookup(d => d.ParentId!.Value);

            // 2. RE-VINCULACIÓN (V2): Llenar propiedades de navegación en memoria
            foreach (var task in dtos)
            {
                task.Children = childrenLookup[task.Id].OrderBy(x => x.Order).ToList();
                foreach (var dep in task.Dependencies)
                {
                    if (dtoDict.TryGetValue(dep.PredecessorId, out var pred))
                        dep.Predecessor = pred;
                }
            }

            // 3. WBS (V2): Asignar numeración jerárquica (1.1, 1.2...)
            var rootDtos = dtos.Where(d => !d.ParentId.HasValue).OrderBy(d => d.Order).ToList();
            int idnumber = 0;
            AssignWbsToDtos(rootDtos, childrenLookup, "", ref idnumber);

            // 4. CONFLICTOS (V1): Validar referencias circulares jerárquicas
            ValidateHierarchicalConflicts(dtos, dtoDict);

            // 5. ORDENAMIENTO (V2): Topological Sort para calcular dependencias en orden
            var orderedDtos = TopologicalSortDtos(dtos, childrenLookup);

            // 6. CÁLCULO DE FECHAS (FUSIÓN V1 + V2)
            foreach (var dto in orderedDtos)
            {
                // Aquí ocurre la magia segura
                RecalculateSingleTaskSafe(dto, childrenLookup, dtos);

                // Generar string para la UI
                dto.SummaryDependencies = GetSummaryDependencies(dto.Dependencies, dtoDict);
            }

            return dtos.OrderBy(x => x.IdNumber).ToList();
        }

        // ============================================================================================
        // 2. LÓGICA DE CÁLCULO SEGURA (Copia de V1 Logic)
        // ============================================================================================
        private static void RecalculateSingleTaskSafe(GanttDto dto, ILookup<Guid, GanttDto> childrenLookup, List<GanttDto> allDtos)
        {
            var start = dto.StartDate;
            var end = dto.EndDate;
            var dur = dto.Duration?.Trim() ?? "1d";

            // PASO A: Dependencias (V1 Logic)
            // Calculamos fechas sugeridas por los predecesores
            var (depStart, depEnd) = GetDependencyDates_V1_Logic(dto.Dependencies);

            // Aplicamos fechas de dependencias si el usuario no forzó esa fecha manualmente
            bool isStartManual = dto.LastModifiedField == GanttField.StartDate;
            bool isEndManual = dto.LastModifiedField == GanttField.EndDate;

            if (depStart.HasValue && !isStartManual) start = depStart;
            if (depEnd.HasValue && !isEndManual) end = depEnd;

            // PASO B: Herencia de Hijos (V2 Logic - La lógica de "Resumen")
            var children = childrenLookup[dto.Id].ToList();
            if (children.Any())
            {
                // Si tiene hijos, la tarea es un contenedor. Sus fechas las dictan los hijos.
                var validStarts = children.Where(c => c.StartDate.HasValue).Select(c => c.StartDate!.Value).ToList();
                var validEnds = children.Where(c => c.EndDate.HasValue).Select(c => c.EndDate!.Value).ToList();

                if (validStarts.Any()) start = validStarts.Min();
                if (validEnds.Any()) end = validEnds.Max();

                // Recalcular duración basada en el nuevo start/end abarcativo
                if (start.HasValue && end.HasValue)
                {
                    dur = DurationParser.ToDuration(start.Value, end.Value, DurationParser.UnitFromChar(dur.LastOrDefault()));
                }
             
            }
            else
            {
                // PASO C: Tarea Hoja (Sin hijos) -> Usamos la MATEMÁTICA PURA DE V1
                Recalculate_V1_Math(ref start, ref end, ref dur, dto.LastModifiedField);
            }

            dto.StartDate = start;
            dto.EndDate = end;
            dto.Duration = dur;
        }

        // COPIA EXACTA DE TU MÉTODO "Recalculate" de la V1 (Solo cambiado a private)
        // ===========================================================================
        // MOTOR MATEMÁTICO (Extrado de V1 -> Recalculate)
        // ===========================================================================

        public static void Recalculate_V1_Math(ref DateTime? start, ref DateTime? end, ref string duration, GanttField? lastModified)
        {
            // Regla de Oro: Sin fecha de inicio, no hay cálculo posible.
            if (!start.HasValue) return;

            // Aseguramos que duration tenga algo válido para evitar crash
            if (string.IsNullOrWhiteSpace(duration)) duration = "1d";

            switch (lastModified)
            {
                case GanttField.EndDate:
                    // CASO A: Usuario cambió la FECHA FIN.
                    // Objetivo: Recalcular cuánto dura la tarea ahora.
                    if (end.HasValue)
                    {
                        // 1. Detectamos qué unidad está usando (días, semanas, etc.) para respetarla
                        var currentUnitChar = DurationParser.TryParse(duration)?.unit ?? 'd';
                        var unitEnum = DurationParser.UnitFromChar(currentUnitChar);

                        // 2. Calculamos la nueva duración usando tu parser
                        // Fórmula: Duration = End - Start
                        duration = DurationParser.ToDuration(start.Value, end.Value, unitEnum);
                    }
                    break;

                case GanttField.Duration:
                    // CASO B: Usuario cambió la DURACIÓN (ej: escribió "3w").
                    // Objetivo: Empujar la fecha fin hacia adelante.
                    // Fórmula: End = Start + Duration
                    end = DurationParser.AddDuration(start.Value, duration);
                    break;

                case GanttField.StartDate:
                default:
                    // CASO C: Usuario movió el INICIO (o es un cálculo inicial).
                    // Objetivo: Mover la tarea completa manteniendo su duración constante.
                    // Fórmula: End = Start + Duration
                    end = DurationParser.AddDuration(start.Value, duration);
                    break;
            }
        }

        // COPIA EXACTA DE TU MÉTODO "GetDependencyDates" de la V1
        private static (DateTime? startDate, DateTime? endDate) GetDependencyDates_V1_Logic(List<GanttDependencyDto> dependencies)
        {
            if (dependencies == null || !dependencies.Any()) return (null, null);

            DateTime? earliestStart = null;
            DateTime? earliestEnd = null;

            foreach (var dep in dependencies)
            {
                if (dep.IsCircularConflict) continue;
                var pred = dep.Predecessor; // Usamos el objeto ya vinculado en memoria
                if (pred == null || !pred.StartDate.HasValue || !pred.EndDate.HasValue) continue;

                // Lógica de Lag exacta de V1
                var parsedLag = DurationParser.TryParse(dep.Lag);
                var lagDays = parsedLag?.amount ?? 0.0;

                if (parsedLag.HasValue)
                {
                    lagDays = parsedLag.Value.unit switch
                    {
                        'd' => parsedLag.Value.amount,
                        'w' => parsedLag.Value.amount * 7,
                        'm' => parsedLag.Value.amount * 30.44,
                        'q' => parsedLag.Value.amount * 91.32,
                        's' => parsedLag.Value.amount * 182.64,
                        'y' => parsedLag.Value.amount * 365.25,
                        _ => parsedLag.Value.amount
                    };
                }

                var lag = TimeSpan.FromDays(lagDays);

                switch (dep.Type)
                {
                    case DependencyType.FinishToStart: // FS
                        var baseFsStart = pred.EndDate.Value + lag;
                        // Ajuste continuo
                        if (Math.Abs(lagDays) < 0.01) baseFsStart = baseFsStart.AddDays(1);
                        earliestStart = MaxDate(earliestStart, baseFsStart);
                        break;

                    case DependencyType.StartToStart: // SS
                        var ssStart = pred.StartDate.Value + lag;
                        earliestStart = MaxDate(earliestStart, ssStart);
                        break;

                    case DependencyType.FinishToFinish: // FF
                        var ffEnd = pred.EndDate.Value + lag;
                        earliestEnd = MaxDate(earliestEnd, ffEnd);
                        break;

                    case DependencyType.StartToFinish: // SF
                        var sfEnd = pred.StartDate.Value + lag;
                        earliestEnd = MaxDate(earliestEnd, sfEnd);
                        break;
                }
            }
            return (earliestStart, earliestEnd);
        }

        // ============================================================================================
        // 3. ESTRUCTURA Y VALIDACIONES (V2 Optimization + V1 Validation)
        // ============================================================================================

        // V2 Optimized
        private static void AssignWbsToDtos(List<GanttDto> siblings, ILookup<Guid, GanttDto> childrenLookup, string parentWbs, ref int idnumber)
        {
            for (int i = 0; i < siblings.Count; i++)
            {
                idnumber++;
                var dto = siblings[i];
                dto.WbsCode = string.IsNullOrEmpty(parentWbs) ? (i + 1).ToString() : $"{parentWbs}.{i + 1}";
                dto.IdNumber = idnumber;

                var children = childrenLookup[dto.Id].OrderBy(c => c.Order).ToList();
                if (children.Any()) AssignWbsToDtos(children, childrenLookup, dto.WbsCode, ref idnumber);
            }
        }

        // V2 Optimized
        public static List<GanttDto> TopologicalSortDtos(List<GanttDto> dtos)
        {
            if (dtos == null || !dtos.Any()) return new List<GanttDto>();

            // 1. Construir el índice rápido (Optimización V3)
            var childrenLookup = dtos.Where(d => d.ParentId.HasValue)
                                     .ToLookup(d => d.ParentId!.Value);

            // 2. Llamar a la versión core
            return TopologicalSortDtos(dtos, childrenLookup);
        }

        // ===========================================================================
        // LÓGICA CORE (Ajustada para garantizar orden visual)
        // ===========================================================================
        public static List<GanttDto> TopologicalSortDtos(List<GanttDto> dtos, ILookup<Guid, GanttDto> childrenLookup)
        {
            var result = new List<GanttDto>();
            var visited = new HashSet<Guid>();

            void Dfs(GanttDto dto)
            {
                if (visited.Contains(dto.Id)) return;
                visited.Add(dto.Id);

                // IMPORTANTE: Procesar hijos en orden visual (.Order)
                // Esto garantiza que 'IndentRight' encuentre al hermano correcto
                var sortedChildren = childrenLookup[dto.Id].OrderBy(c => c.Order);

                foreach (var child in sortedChildren)
                {
                    Dfs(child);
                }
                result.Add(dto);
            }

            // Empezar por raíces ordenadas
            foreach (var root in dtos.Where(d => !d.ParentId.HasValue).OrderBy(d => d.Order))
            {
                Dfs(root);
            }

            return result;
        }

        // V1 Logic (Requerido para validación visual)
        public static string GetSummaryDependencies(List<GanttDependencyDto> dependencies, Dictionary<Guid, GanttDto> dtoDict)
        {
            if (dependencies == null || !dependencies.Any()) return string.Empty;
            return string.Join(", ", dependencies.OrderBy(d => d.Order).Select(dep =>
            {
                if (!dtoDict.TryGetValue(dep.PredecessorId, out var pred)) return "";
                var typeAbbr = dep.Type.GetDescription();
                var lag = dep.Lag?.Trim() ?? "0d";
                var text = lag == "0d" ? $"{pred.IdNumber}{typeAbbr}" : $"{pred.IdNumber}{typeAbbr}+{lag}";
                return dep.IsCircularConflict ? $"[CONFLICT:{text}]" : text;
            }).Where(s => !string.IsNullOrEmpty(s)));
        }

        // V1 Logic (Requerido para integridad)
        public static void ValidateHierarchicalConflicts(List<GanttDto> allDtos, Dictionary<Guid, GanttDto> dtoDict)
        {
            foreach (var dto in allDtos)
            {
                if (dto.Dependencies == null) continue;
                foreach (var dep in dto.Dependencies)
                {
                    bool isParent = dto.ParentId == dep.PredecessorId;
                    bool isDescendant = IsDescendant(dto.Id, dep.PredecessorId, dtoDict);

                    dep.IsCircularConflict = isParent || isDescendant;
                    if (dep.IsCircularConflict)
                    {
                        var predIdNum = dtoDict.TryGetValue(dep.PredecessorId, out var p) ? p.IdNumber.ToString() : "?";
                        dep.ConflictMessage = isParent ? $"Conflict: Task {predIdNum} is Parent" : $"Conflict: Task {predIdNum} is Descendant";
                    }
                }
            }
        }

        private static bool IsDescendant(Guid potentialParentId, Guid targetId, Dictionary<Guid, GanttDto> dtoDict)
        {
            var current = dtoDict.TryGetValue(targetId, out var t) ? t : null;
            while (current?.ParentId != null)
            {
                if (current.ParentId == potentialParentId) return true;
                current = dtoDict.TryGetValue(current.ParentId.Value, out var p) ? p : null;
            }
            return false;
        }

        // --- Helpers Auxiliares copiados de V1 para autosuficiencia ---
        private static string CalculateDuration_Helper(DateTime start, DateTime end, char unit)
        {
            // Usa tu helper existente DurationParser
            return DurationParser.ToDuration(start, end, DurationParser.UnitFromChar(unit));
        }

        private static DateTime? MaxDate(DateTime? a, DateTime? b) => a.HasValue && b.HasValue ? (a > b ? a : b) : a ?? b;

   

        
        // ===========================================================================
        // MÉTODOS DE SOPORTE UI (Versión Definitiva Integrada con DurationParser)
        // ===========================================================================

        /// <summary>
        /// Recalcula una tarea específica considerando su contexto (Hijos y Dependencias).
        /// Usa DurationParser para cálculos de Lag y Duración.
        /// </summary>
        public static void RecalculateDto(GanttDto dto, List<GanttDto> allDtos)
        {
            var start = dto.StartDate;
            var end = dto.EndDate;
            // Aseguramos que duration tenga formato válido o default "0d"
            var dur = string.IsNullOrWhiteSpace(dto.Duration) ? "0d" : dto.Duration.Trim();

            // ---------------------------------------------------------
            // PASO 1: HERENCIA DE HIJOS (Prioridad Máxima)
            // ---------------------------------------------------------
            var children = allDtos.Where(c => c.ParentId == dto.Id).ToList();

            // Si tu UI necesita la propiedad Children poblada:
            dto.Children = children;

            if (children.Any())
            {
                // Lógica V1: El padre se vuelve un "Resumen" de sus hijos
                var childStarts = children.Where(c => c.StartDate.HasValue).Select(c => c.StartDate!.Value).ToList();
                var childEnds = children.Where(c => c.EndDate.HasValue).Select(c => c.EndDate!.Value).ToList();

                if (childStarts.Any() && childEnds.Any())
                {
                    start = childStarts.Min();
                    end = childEnds.Max();

                    // Recalculamos la duración del padre basada en su nuevo rango
                    if (start.HasValue && end.HasValue)
                    {
                        // Obtenemos la unidad actual (d, w, m...) para respetarla
                        var currentUnit = DurationParser.TryParse(dur)?.unit ?? 'd';
                        var unitEnum = DurationParser.UnitFromChar(currentUnit);

                        // Usamos TU DurationParser para calcular la nueva duración
                        dur = DurationParser.ToDuration(start.Value, end.Value, unitEnum);
                    }
                }
            }
            else
            {
                // ---------------------------------------------------------
                // PASO 2: DEPENDENCIAS (Solo si no tiene hijos)
                // ---------------------------------------------------------
                DateTime? maxDepEnd = null;

                if (dto.Dependencies != null && dto.Dependencies.Any())
                {
                    foreach (var dep in dto.Dependencies)
                    {
                        var pred = allDtos.FirstOrDefault(x => x.Id == dep.PredecessorId);

                        if (pred != null && pred.EndDate.HasValue)
                        {
                            // ✅ CORRECCIÓN: Usamos DurationParser.AddDuration directamente
                            // Si el Lag es inválido, asumimos 0 días (usamos la fecha original)
                            var potentialStart = DurationParser.AddDuration(pred.EndDate.Value, dep.Lag)
                                                 ?? pred.EndDate.Value;

                            if (maxDepEnd == null || potentialStart > maxDepEnd)
                            {
                                maxDepEnd = potentialStart;
                            }
                        }
                    }
                }

                // Regla V1: La dependencia empuja el Start si no fue editado manualmente
                bool isStartManual = dto.LastModifiedField == GanttField.StartDate;

                if (maxDepEnd.HasValue && !isStartManual)
                {
                    start = maxDepEnd.Value;
                }

                // ---------------------------------------------------------
                // PASO 3: CÁLCULO MATEMÁTICO PURO (Start + Dur -> End)
                // ---------------------------------------------------------
                Recalculate_V1_Math(ref start, ref end, ref dur, dto.LastModifiedField);
            }

            // 4. Asignar valores finales
            dto.StartDate = start;
            dto.EndDate = end;
            dto.Duration = dur;
        }
    }
}