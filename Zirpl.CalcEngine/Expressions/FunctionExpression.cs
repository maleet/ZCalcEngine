using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zirpl.CalcEngine
{
    /// <summary>
    /// Function call expression, e.g. sin(0.5)
    /// </summary>
    class FunctionExpression : Expression
    {
        // ** fields
        FunctionDefinition _fn;
        public List<Expression> Parms { get; }
        private readonly CalculationEngine _engine;

        // ** ctor
        internal FunctionExpression()
        {
        }
        public FunctionExpression(FunctionDefinition function, List<Expression> parms, CalculationEngine engine)
        {
            _fn = function;
            Parms = parms;
            _engine = engine;
        }

        // ** object model
        override public object Evaluate()
        {
            return _fn.ContextFunction != null ? _fn.ContextFunction(_engine, Parms) : _fn.Function?.Invoke(Parms);
        }
        public override Expression Optimize()
        {
            bool allLits = true;
            if (Parms != null)
            {
                for (int i = 0; i < Parms.Count; i++)
                {
                    var p = Parms[i].Optimize();
                    Parms[i] = p;
                    if (p._token.Type != TokenType.LITERAL)
                    {
                        allLits = false;
                    }
                }
            }
            return allLits
                ? new Expression(this.Evaluate())
                : this;
        }
    }
}
