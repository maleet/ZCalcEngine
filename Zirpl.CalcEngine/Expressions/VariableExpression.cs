using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zirpl.CalcEngine
{
    /// <summary>
    /// Simple variable reference.
    /// </summary>
    class VariableExpression : Expression
    {
        public CalculationEngine CalculationEngine { get; }
        public object Value { get; set; }
        IDictionary<string, object> _dct;
        internal string _name;
        List<Expression> _parms;

        public VariableExpression(CalculationEngine engine, IDictionary<string, object> dct, string name, List<Expression> parms)
        {
            CalculationEngine = engine;
            _dct = dct;
            _name = name;
            _parms = parms;
        }

        public override object Evaluate()
        {
            var value = GetValue();
            if (CalculationEngine.LogBindingExpressionValues)
            {
                this.Value = value;
            }

            return value;
        }

        private object GetValue()
        {
            if (_dct[_name] is IEnumerable && _parms?.Count > 0)
                return GetValue(_dct[_name], _parms);
            else
                return _dct[_name];
        }

        object GetValue(object obj, List<Expression> par)
        {
            if (obj != null)
            {
                var namedList = (IList) obj;
                int index = (int) par[0];
                if (index >= 0 && index < namedList.Count)
                    return namedList[index];
                else
                    return null;
            }
            else
                return null;
        }
    }
}