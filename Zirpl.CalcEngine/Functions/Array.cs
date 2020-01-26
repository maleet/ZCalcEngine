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
                var input = parms[0].Evaluate();
                var enumerable = input as IEnumerable ?? new Object[]{};
                var target = parms[1].Evaluate() ?? "";

                var targets = target as IEnumerable ?? new Object[] {target};
                
                if (input is string s)
                {
                    char separator = ';';
                    if (parms.Count == 3)
                    {
                        separator = (parms[3].Evaluate() as string ?? ";")[0];
                    }

                    enumerable = s.Split(separator);
                    foreach (var o in targets)
                    {
                        if (enumerable.Cast<object>().Contains(o.ToString()))
                        {
                            return true;
                        }
                    }
                    
                    return false;
                }
                
                foreach (var o in targets)
                {
                    if (enumerable.Cast<object>().Contains(o))
                    {
                        return true;
                    }
                }
                
                return false;
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
        }
    }
}