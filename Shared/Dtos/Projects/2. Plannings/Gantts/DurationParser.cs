using System.Globalization;

namespace Shared.Dtos.Projects.Plannings.Gantts
{


    public static class DurationParser
    {
        public static (double amount, char unit, bool hadUnit)? TryParseDetailed(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            input = input.Trim().ToLowerInvariant();

            var numPart = string.Concat(input.TakeWhile(c => char.IsDigit(c) || c == '.'));
            var unitPart = string.Concat(input.SkipWhile(c => char.IsDigit(c) || c == '.').Take(1));

            if (string.IsNullOrEmpty(numPart)) return null;

            if (!double.TryParse(numPart, NumberStyles.Float, CultureInfo.InvariantCulture, out double amount))
                return null;

            char unit = '\0';
            bool hadUnit = false;

            if (!string.IsNullOrEmpty(unitPart) && "dwmqsy".Contains(unitPart[0]))
            {
                unit = unitPart[0];
                hadUnit = true;
            }

            return (amount, unit, hadUnit);
        }

        public static (double amount, char unit)? TryParse(string input)
        {
            var result = TryParseDetailed(input);
            return result.HasValue ? (result.Value.amount, result.Value.unit) : null;
        }

        public static DurationUnit UnitFromChar(char unit) => unit switch
        {
            'd' => DurationUnit.Days,
            'w' => DurationUnit.Weeks,
            'm' => DurationUnit.Months,
            'q' => DurationUnit.Quarters,
            's' => DurationUnit.Semesters,
            'y' => DurationUnit.Years,
            _ => DurationUnit.Days
        };

        public static char CharFromUnit(DurationUnit unit) => unit switch
        {
            DurationUnit.Days => 'd',
            DurationUnit.Weeks => 'w',
            DurationUnit.Months => 'm',
            DurationUnit.Quarters => 'q',
            DurationUnit.Semesters => 's',
            DurationUnit.Years => 'y',
            _ => 'd'
        };

        public static DateTime? AddDuration(DateTime startDate, string duration)
        {
            var parsed = TryParse(duration);
            if (!parsed.HasValue) return null;
            var (a, u) = parsed.Value;

            return u switch
            {
                'd' => startDate.AddDays(a),
                'w' => startDate.AddDays(a * 7),
                'm' => startDate.AddMonths((int)Math.Floor(a)).AddDays((a - Math.Floor(a)) * 30.44),
                'q' => startDate.AddMonths((int)Math.Floor(a * 3)).AddDays((a * 3 - Math.Floor(a * 3)) * 30.44),
                's' => startDate.AddMonths((int)Math.Floor(a * 6)).AddDays((a * 6 - Math.Floor(a * 6)) * 30.44),
                'y' => startDate.AddYears((int)Math.Floor(a)).AddDays((a - Math.Floor(a)) * 365.25),
                _ => null
            };
        }

        public static DateTime? SubtractDuration(DateTime endDate, string duration)
        {
            var parsed = TryParse(duration);
            if (!parsed.HasValue) return null;
            var (a, u) = parsed.Value;

            return u switch
            {
                'd' => endDate.AddDays(-a),
                'w' => endDate.AddDays(-a * 7),
                'm' => endDate.AddMonths(-(int)Math.Floor(a)).AddDays(-(a - Math.Floor(a)) * 30.44),
                'q' => endDate.AddMonths(-(int)Math.Floor(a * 3)).AddDays(-(a * 3 - Math.Floor(a * 3)) * 30.44),
                's' => endDate.AddMonths(-(int)Math.Floor(a * 6)).AddDays(-(a * 6 - Math.Floor(a * 6)) * 30.44),
                'y' => endDate.AddYears(-(int)Math.Floor(a)).AddDays(-(a - Math.Floor(a)) * 365.25),
                _ => null
            };
        }

        public static string ToDuration(DateTime start, DateTime end, DurationUnit unit)
        {
            if (start > end) (start, end) = (end, start);
            var days = (end - start).TotalDays;
            if (days < 1) days = 1;

            return unit switch
            {
                DurationUnit.Days => $"{(int)days}d",
                DurationUnit.Weeks => FormatWithDecimal(days / 7.0, 'w'),
                DurationUnit.Months => FormatWithDecimal(days / 30.44, 'm'),
                DurationUnit.Quarters => FormatWithDecimal(days / 91.32, 'q'),
                DurationUnit.Semesters => FormatWithDecimal(days / 182.64, 's'),
                DurationUnit.Years => FormatWithDecimal(days / 365.25, 'y'),
                _ => $"{(int)days}d"
            };
        }

        public static string ToDuration(DateTime start, DateTime end) =>
            ToDuration(start, end, DurationUnit.Days);

        public static double ParseOffset(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return 0;
            input = input.Trim().ToLowerInvariant();
            bool isNegative = input.StartsWith("-");
            if (isNegative) input = input[1..];

            var parsed = TryParse(input);
            if (!parsed.HasValue) return 0;

            var (a, u) = parsed.Value;
            var days = u switch
            {
                'd' => a,
                'w' => a * 7,
                'm' => a * 30.44,
                'q' => a * 91.32,
                's' => a * 182.64,
                'y' => a * 365.25,
                _ => a
            };
            return isNegative ? -days : days;
        }

        public static string FormatWithDecimal(double value, char unit)
        {
            if (Math.Abs(value - Math.Round(value)) < 0.05)
                return $"{(int)Math.Round(value)}{unit}";
            return $"{value:F1}{unit}";
        }
    }
}
