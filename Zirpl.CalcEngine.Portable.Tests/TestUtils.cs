using System;
using System.Collections.Generic;

namespace Zirpl.CalcEngine.Tests
{
    public static class TestUtils
    {
        public static void Test<T>(this CalculationEngine engine, string expression, T expectedResult, string expectedParsedExpression = null)
        {
            var result = engine.Evaluate<T>(expression);

            if (expectedParsedExpression != null)
            {
                if (!string.Equals(expectedParsedExpression, engine.ParsedExpression))
                {
                    var msg = $"error: {engine.ParsedExpression} should equal {expectedParsedExpression}";
                    throw new Exception(msg);
                }
            }
            
            if (!Equals(result, expectedResult))
            {
                var msg = $"error: {expression} gives {result}, should give {expectedResult}";
                throw new Exception(msg);
            }

            Console.WriteLine(engine.ParsedExpression);
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