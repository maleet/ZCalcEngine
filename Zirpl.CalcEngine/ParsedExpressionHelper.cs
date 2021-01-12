using System.Collections;
using System.Collections.Generic;

namespace Zirpl.CalcEngine
{
    public class ParsedExpressionHelper
    {
        internal static string ParseBindings(Expression expression, string expressionString)
        {
            expressionString = ParseAsFunction(expressionString, expression);
            expressionString = ParseAsBinding(expressionString, expression);
            expressionString = ParseAsVariable(expressionString, expression);
            expressionString = ParseAsBinary(expressionString, expression);
            
            return expressionString.Replace("**", "*");
        }

        private static string ParseAsFunction(string expressionString, Expression expression)
        {
            if (expression is FunctionExpression functionExpression)
            {
                foreach (var functionExpressionParm in functionExpression.Parms)
                {
                    expressionString = ParseAsFunction(expressionString, functionExpressionParm);
                    expressionString = ParseAsBinding(expressionString, functionExpressionParm);
                    expressionString = ParseAsVariable(expressionString, functionExpressionParm);
                }
            }

            return expressionString;
        }

        private static string ParseAsBinary(string expressionString, Expression expression)
        {
            if (expression is BinaryExpression binaryExpression)
            {
                expressionString = ParseAsBinding(expressionString, binaryExpression._lft);
                expressionString = ParseAsBinding(expressionString, binaryExpression._rgt, binaryExpression._token);
                expressionString = ParseAsVariable(expressionString, binaryExpression._lft);
                expressionString = ParseAsVariable(expressionString, binaryExpression._rgt, binaryExpression._token);
                expressionString = ParseAsBinary(expressionString, binaryExpression._lft);
                expressionString = ParseAsBinary(expressionString, binaryExpression._rgt);
                expressionString = ParseAsFunction(expressionString, binaryExpression._lft);
                expressionString = ParseAsFunction(expressionString, binaryExpression._rgt);
            }

            return expressionString;
        }

        private static string ParseAsVariable(string expressionString, Expression functionExpressionParm, Token binaryExpressionToken = null)
        {
            if (functionExpressionParm is VariableExpression variableExpression)
            {
                var s = GetString(variableExpression.Value);
                if (binaryExpressionToken != null && binaryExpressionToken.Type == TokenType.MULDIV)
                {
                    s = "*"+s;
                }
                
                expressionString = expressionString.Replace(variableExpression._name, s);
            }

            return expressionString;
        }

        private static string ParseAsBinding(string expressionString, Expression functionExpressionParm, Token binaryExpressionToken = null)
        {
            if (functionExpressionParm is BindingExpression bindingExpression)
            {
                var bindingPath = bindingExpression._expression;
                var bindingExpressionValue = GetString(bindingExpression.Value).Replace("System.Object", "?");
                if (binaryExpressionToken != null && binaryExpressionToken.Type == TokenType.MULDIV)
                {
                    bindingExpressionValue = "*"+bindingExpressionValue;
                }
                
                expressionString = expressionString.Replace(bindingPath, bindingExpressionValue);
            }
            return expressionString;
        }

        private static string GetString(object o)
        {
            if(!(o is string) && o is IEnumerable enumerable)
            {
                List<string> list = new List<string>();
                foreach(var item in enumerable)
                {
                    list.Add(item.ToString());
                }
                
                return string.Join(", ", list);
            }

            return (o ?? "").ToString();
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
                    objects[keyValuePair.Key] = keyValuePair.Value;
                }
            }
        }
    }
}