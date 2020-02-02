﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Computator.NET.Core.Evaluation;
using Computator.NET.DataTypes;
using Computator.NET.DataTypes.Functions;
using Microsoft.Extensions.Logging;

namespace Computator.NET.WebApi.Services
{

    public interface IFunctionsProvider
    {
        Function GetFunction(string equation, string customFunctionsCode);
        Function GetFunction(string equation, CalculationsMode calculationsMode, string customFunctionsCode);
    }
    public class FunctionsProvider : IFunctionsProvider
    {
        private readonly IModeDeterminer _modeDeterminer;
        private readonly IExpressionsEvaluator _expressionsEvaluator;

        private readonly Dictionary<string,Function> _functionsCache = new Dictionary<string, Function>();

        public FunctionsProvider(IModeDeterminer modeDeterminer, IExpressionsEvaluator expressionsEvaluator)
        {
            _modeDeterminer = modeDeterminer;
            _expressionsEvaluator = expressionsEvaluator;
        }

        public Function GetFunction(string equation, string customFunctionsCode)
        {
            var mode = _modeDeterminer.DetermineMode(equation);
            return GetFunction(equation, mode, customFunctionsCode);
        }

        public Function GetFunction(string equation, CalculationsMode calculationsMode, string customFunctionsCode)
        {
            var key = $"{equation}{calculationsMode}{customFunctionsCode}";
            if (!_functionsCache.ContainsKey(key))
            {
                var func = _expressionsEvaluator.Evaluate(equation, customFunctionsCode, calculationsMode);
                _functionsCache.Add(key,func);
            }

            return _functionsCache[key];
        }
    }
}
