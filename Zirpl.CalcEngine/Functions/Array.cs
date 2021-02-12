using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Zirpl.CalcEngine
{
    static class Array
    {
        public static void Register(CalculationEngine ce)
        {
            ce.RegisterFunction("Range", 2, 3, parms =>
            {
                double min = (double) parms[0].Evaluate();
                double max = (double) parms[1].Evaluate();
                double step = 1;
                if (parms.Count > 2)
                {
                    step = (double) parms[2].Evaluate();
                }

                return Enumerable
                    .Repeat(min, (int) ((max - min) / step) + 1)
                    .Select((tr, ti) => tr + (step * ti))
                    .ToArray();
            });

            ce.RegisterFunction("Array", 0, int.MaxValue, parms =>
            {
                var objects = new List<object>();
                if (parms != null)
                {
                    foreach (var v in parms)
                    {
                        objects.Add(v.Evaluate());
                    }
                }

                return objects.ToArray();
            });

            ce.RegisterFunction("ArrayString", 1, int.MaxValue, parms =>
            {
                var objects = new List<object>();
                if (parms != null)
                {
                    foreach (var v in parms)
                    {
                        objects.AddRange(v.Evaluate().ToString().Split(';').ToList());
                    }
                }

                return objects.ToArray();
            });
            ce.RegisterFunction("CONTAINS", 2, 3, parms =>
            {
                var containsTrimStartChars = ce.Options.Functions.ContainsTrimStartChars.ToArray();
                var input = parms[0].Evaluate();
                var includes = input as IList ?? new[] {input};
                var target = parms[1].Evaluate() ?? "";
                var excludes = new List<string>();

                if (input is string s)
                {
                    char separator = ';';
                    if (parms.Count == 3)
                    {
                        separator = (parms[2].Evaluate() as string ?? ";")[0];
                    }

                    includes = s.Split(separator);

                    excludes = includes.Cast<object>().Where(o => ((string) o).Trim().StartsWith("!")).Select(o => o.ToString().Trim().TrimStart('!')).ToList();
                    includes = includes.Cast<object>().Where(o => !((string) o).Trim().StartsWith("!")).Select(o => o.ToString().Trim()).ToList();
                }

                var targets = target as IEnumerable ?? new Object[] {target};
                if (target is string t)
                {
                    targets = new[] {t};
                }

                var result = false;
                foreach (var o in targets)
                {
                    if (includes.Cast<object>().Any(o1 => string.Equals(o1?.ToString().TrimStart(containsTrimStartChars), o?.ToString().TrimStart(containsTrimStartChars), StringComparison.InvariantCultureIgnoreCase)))
                    {
                        result = true;
                    }
                    if (excludes.Any(o1 => string.Equals(o1?.ToString().TrimStart(containsTrimStartChars), o?.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                    {
                        result = false;
                        break;
                    }
                }

                return result;
            });

            ce.RegisterFunction("Map", 0, int.MaxValue, parms =>
            {
                var objects = new List<object>();
                if (parms != null)
                {
                    foreach (var v in parms)
                    {
                        IEnumerable o = (IEnumerable) v.Evaluate();

                        var enumerable = o.Cast<object>();

                        objects.AddRange(enumerable.Where(o1 => !objects.Contains(o1)));
                    }
                }

                return objects.ToArray();
            });
            ce.RegisterFunction("XLookup", 3, 4, parms =>
            {
                var search = parms[0].Evaluate();
                var keys = ((IEnumerable) parms[1].Evaluate()).Cast<object>().ToList();
                var values = ((IEnumerable) parms[2].Evaluate()).Cast<object>().ToList();
                var defaultValue = parms[3].Evaluate();

                var isNumber = search.IsNumber();
                if (isNumber)
                {
                    search = search.TryConvertToDouble();
                }
                
                var ix = keys.FindIndex(o =>
                {
                    if (isNumber)
                    {
                        return Equals(o?.TryConvertToDouble(), search);
                    }
                    return Equals(o, search);
                });
                if (ix >= 0)
                {
                    return values[ix];
                }

                return defaultValue;
            });
        }
    }
}