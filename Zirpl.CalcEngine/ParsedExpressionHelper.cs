using System.Collections;
using System.Collections.Generic;

namespace Zirpl.CalcEngine
{
    public class ParsedExpressionHelper
    {
        internal static string ParseBindings(Expression expression, string expressionString)
        {
            if (expression is FunctionExpression functionExpression)
            {
                foreach (var functionExpressionParm in functionExpression.Parms)
                {
                    expressionString = ParseAsBinding(expressionString, functionExpressionParm);
                    expressionString = ParseAsVariable(expressionString, functionExpressionParm);
                }
            }
            
            expressionString = ParseAsBinding(expressionString, expression);
            expressionString = ParseAsVariable(expressionString, expression);
            expressionString = ParseAsBinary(expressionString, expression);
            
            return expressionString;
        }

        private static string ParseAsBinary(string expressionString, Expression expression)
        {
            if (expression is BinaryExpression binaryExpression)
            {
                expressionString = ParseAsBinding(expressionString, binaryExpression._lft);
                expressionString = ParseAsBinding(expressionString, binaryExpression._rgt);
                expressionString = ParseAsVariable(expressionString, binaryExpression._lft);
                expressionString = ParseAsVariable(expressionString, binaryExpression._rgt);
                expressionString = ParseAsBinary(expressionString, binaryExpression._lft);
                expressionString = ParseAsBinary(expressionString, binaryExpression._rgt);
            }

            return expressionString;
        }

        private static string ParseAsVariable(string expressionString, Expression functionExpressionParm)
        {
            if (functionExpressionParm is VariableExpression variableExpression)
            {
                expressionString = expressionString.Replace(variableExpression._name, GetString(variableExpression.Value));
            }

            return expressionString;
        }

        private static string ParseAsBinding(string expressionString, Expression functionExpressionParm)
        {
            if (functionExpressionParm is BindingExpression bindingExpression)
            {
                var bindingPath = bindingExpression._expression;
                var bindingExpressionValue = GetString(bindingExpression.Value);
                expressionString = expressionString.Replace(bindingPath, bindingExpressionValue);
            }
            return expressionString;
        }

        private static string GetString(object o)
        {
            if(o is IEnumerable enumerable)
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
    }
}