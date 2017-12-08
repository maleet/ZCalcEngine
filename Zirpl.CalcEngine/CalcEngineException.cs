using System;

namespace Zirpl.CalcEngine
{
    public class CalcEngineException : ArgumentException
    {
        public string Expression { get; }
        public object DataContext { get; }

        public CalcEngineException(string message, string expression, object dataContext) : base(message)
        {
            Expression = expression;
            DataContext = dataContext;
        }
    }
}