﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Zirpl.CalcEngine
{
    /// <summary>
    /// Function definition class (keeps function name, parameter counts, and delegate).
    /// </summary>
    public class FunctionDefinition
    {
        public string FunctionName { get; }

        // ** fields
        public int ParmMin, ParmMax;
        public CalcEngineFunction Function;
        public CalcEngineContextFunction ContextFunction;

        // ** ctor
        public FunctionDefinition(int parmMin, int parmMax, CalcEngineFunction function, string functionName)
        {
            FunctionName = functionName;
            ParmMin = parmMin;
            ParmMax = parmMax;
            Function = function;
        }
        
        public FunctionDefinition(int parmMin, int parmMax, CalcEngineContextFunction function, string functionName)
        {
            FunctionName = functionName;
            ParmMin = parmMin;
            ParmMax = parmMax;
            ContextFunction = function;
        }
    }
}
