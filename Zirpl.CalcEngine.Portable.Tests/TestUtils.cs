using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zirpl.CalcEngine.Portable.Tests
{
    public static class TestUtils
    {
        public static void Test<T>(this CalculationEngine engine, string expression, T expectedResult)
        {
            var result = engine.Evaluate<T>(expression);

            if (!object.Equals(result, expectedResult))
            {
                var msg = string.Format("error: {0} gives {1}, should give {2}", expression, result, expectedResult);
                throw new Exception(msg);
            }
        }

        private static bool IsArray(object value)
        {
            Type valueType = value.GetType();
            return valueType.IsArray;
        }

        static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i])) return false;
            }

            return true;
        }
    }
}