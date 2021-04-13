using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Zirpl.CalcEngine

{
    public static class ExpressionHelper
    {
        internal static string ParseBindings(Expression expression, string expressionString, CalculationOptions calculationOptions, object result)
        {
            List<string> list = new List<string>
            {
                ParseAsFunction(expressionString, expression, calculationOptions), 
                ParseAsBinding(expressionString, expression, calculationOptions), 
                ParseAsVariable(expressionString, expression, calculationOptions), 
                ParseAsBinary(expressionString, expression, calculationOptions)
            };
            
            var replace = string.Join(", ", list.Where(s => !string.IsNullOrEmpty(s))).Replace("**", "*");
            var value = GetResult(result);
            if (!replace.EndsWith(value))
            {
                replace = "(" + replace + ")" + value;
            }

            return replace;
        }

        private static string ParseAsFunction(string expressionString, Expression expression, CalculationOptions calculationOptions)
        {
            if (expression is FunctionExpression functionExpression)
            {
                var methodName = functionExpression._fn.FunctionName;
                List<string> list = new List<string>();
                foreach (var functionExpressionParam in functionExpression.Parms)
                {
                    list.Add(ParseAsFunction(expressionString, functionExpressionParam, calculationOptions));
                    list.Add(ParseAsBinding(expressionString, functionExpressionParam, calculationOptions));
                    list.Add(ParseAsVariable(expressionString, functionExpressionParam, calculationOptions));
                }
                return methodName + "(" + string.Join(", ", list.Where(s => !string.IsNullOrEmpty(s)))+")" + GetResult(functionExpression.Value);
            }

            return null;
        }

        private static string ParseAsBinary(string expressionString, Expression expression, CalculationOptions calculationOptions)
        {
            if (expression is BinaryExpression binaryExpression)
            {
                List<string> list = new List<string>();

                var expressionString1 = ParseAsBinding(expressionString, binaryExpression._lft, calculationOptions);
                list.Add(expressionString1);
                var expressionString2 = ParseAsBinding(expressionString, binaryExpression._rgt, calculationOptions, binaryExpression._token);
                list.Add(expressionString2);
                var expressionString3 = ParseAsVariable(expressionString, binaryExpression._lft, calculationOptions);
                list.Add(expressionString3);
                var expressionString4 = ParseAsVariable(expressionString, binaryExpression._rgt, calculationOptions, binaryExpression._token);
                list.Add(expressionString4);
                var expressionString5 = ParseAsBinary(expressionString, binaryExpression._lft, calculationOptions);
                list.Add(expressionString5);
                var expressionString6 = ParseAsBinary(expressionString, binaryExpression._rgt, calculationOptions);
                list.Add(expressionString6);
                var expressionString7 = ParseAsFunction(expressionString, binaryExpression._lft, calculationOptions);
                list.Add(expressionString7);
                var expressionString8 = ParseAsFunction(expressionString, binaryExpression._rgt, calculationOptions);
                list.Add(expressionString8);
                return string.Join(" " + binaryExpression._token.Value + " ", list.Where(s => !string.IsNullOrEmpty(s)));
            }

            return null;
        }

        private static string ParseAsVariable(string expressionString, Expression functionExpressionParm, CalculationOptions calculationOptions, Token binaryExpressionToken = null)
        {
            if (functionExpressionParm is VariableExpression variableExpression)
            {
                return FormatAsType(variableExpression.Value) ?? variableExpression._name;
            }

            if (functionExpressionParm.GetType() == typeof(Expression))
            {
                var tokenValue = functionExpressionParm._token.Value;
                if (!(tokenValue is string) && tokenValue is IEnumerable enumerable)
                {
                    return "[" + string.Join(", ", enumerable.Cast<object>().Select(FormatAsType)) + "]";
                }
                
                return FormatAsType(tokenValue);
            }

            return null;
        }

        private static string ParseAsBinding(string expressionString, Expression functionExpressionParm, CalculationOptions calculationOptions, Token binaryExpressionToken = null)
        {
            if (functionExpressionParm is BindingExpression bindingExpression)
            {
                return FormatAsType(bindingExpression.Value) ?? bindingExpression._expression;
            }

            return null;
        }

        private static string GetResult(object functionExpressionValue)
        {
            return "/*{" + FormatAsType(functionExpressionValue) + "}*/";
        }

        private static string FormatAsType(object variableExpressionValue)
        {
            if (variableExpressionValue is string)
            {
                return "'" + variableExpressionValue + "'";
            }

            if (variableExpressionValue is decimal value)
            {
                return value.ToString("G29", CultureInfo.InvariantCulture);
            }
            if (variableExpressionValue is double d)
            {
                return d.ToString("G29", CultureInfo.InvariantCulture);
            }
            return variableExpressionValue?.ToString();
        }

        public static Dictionary<string, object> GetBindingValues(Expression expression)
        {
            Dictionary<string, object> objects = new Dictionary<string, object>();
            if (expression is FunctionExpression functionExpression)
            {
                foreach (var functionExpressionParm in functionExpression.Parms)
                {
                    var bindingValues = GetBindingValues(functionExpressionParm);
                    AddOrUpdate(bindingValues);
                }
            }

            if (expression is BinaryExpression binaryExpression)
            {
                AddOrUpdate(GetBindingValues(binaryExpression._lft));
                AddOrUpdate(GetBindingValues(binaryExpression._rgt));
            }

            if (expression is VariableExpression variableExpression)
            {
                objects[variableExpression._name] = variableExpression.Value;
            }

            if (expression is BindingExpression bindingExpression)
            {
                var bindingPath = bindingExpression._expression;
                var bindingExpressionValue = GetString(bindingExpression.Value).Replace("System.Object", "?");
                objects[bindingPath] = bindingExpression.Value;
            }

            return objects;

            void AddOrUpdate(Dictionary<string, object> bindingValues)
            {
                foreach (var keyValuePair in bindingValues)
                {
                    if (!objects.ContainsKey(keyValuePair.Key) || keyValuePair.Value != null)
                    {
                        objects[keyValuePair.Key] = keyValuePair.Value;    
                    }
                }
            }
        }

        private static string GetString(object o)
        {
            if (!(o is string) && o is IEnumerable enumerable)
            {
                List<string> list = new List<string>();
                foreach (var item in enumerable)
                {
                    list.Add(item.ToString());
                }

                return string.Join(", ", list);
            }

            return (o ?? "").ToString();
        }

        public static double TryConvertToDouble(this object o)
        {
            try
            {
                return Convert.ToDouble(o);
            }
            catch (InvalidCastException e)
            {
            }

            return 0;
        }

        public static bool IsNumber(this object value)
        {
            return value is sbyte
                   || value is byte
                   || value is short
                   || value is ushort
                   || value is int
                   || value is uint
                   || value is long
                   || value is ulong
                   || value is float
                   || value is double
                   || value is decimal;
        }
    }
}