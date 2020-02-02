using System;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Computator.NET.Core.Compilation;
using Computator.NET.Core.Constants;
using Computator.NET.Core.Functions;
using Computator.NET.Core.NumericalCalculations;
using Computator.NET.DataTypes;
using Computator.NET.DataTypes.Functions;
using Computator.NET.DataTypes.Utility;
using Computator.NET.Localization;
using NLog;

namespace Computator.NET.Core.Evaluation
{
    public interface IExpressionsEvaluator
    {
        Function Evaluate(string input, string customFunctionsCode, CalculationsMode calculationsMode);
    }

    public class ExpressionsEvaluator : IExpressionsEvaluator
    {
        private const string LambdaScript = MatrixFunctions.ToCode + ScriptingFunctions.ToCode + @"
            public static void CustomFunction(System.Action<string> CONSOLE_OUTPUT_CALLBACK)
            {
                CONSOLE_OUTPUT = CONSOLE_OUTPUT_CALLBACK;
            ";
        private const string LambdaX = @"           
            public static double CustomFunction(double x)
            {
                return ";
        private const string LambdaZ = @"           
            public static System.Numerics.Complex CustomFunction(System.Numerics.Complex z)
            {
                double x=Re(z); double y=Im(z);
                return ";
        private const string LambdaXy = @"           
            public static double CustomFunction(double x, double y)
            {
                return ";
        private const string LambdaXyz = @"           
            public static double CustomFunction(double x, double y, double z)
            {
                return ";

        protected const string Begin = @"
        using MathNet.Numerics.Distributions;//we need it cause of added by dlls distributions" +
                                       TslCompiler.TypesAliases + @"
        namespace FunctionsCreatorNS
        {
            public class FunctionsCreator 
            {
        " + MathematicalConstants.ToCode + PhysicalConstants.ToCode + ElementaryFunctions.ToCode +
                                       SpecialFunctions.ToCode + StatisticsFunctions.ToCode + Integral.ToCode +
                                       Derivative.ToCode + FunctionRoot.ToCode;

        protected const string End = @";
                }
            }            
    ///*{additional_objects}*///

        }";

        private readonly TslCompiler _tslCompiler;

        private readonly Regex _implicitFunctionRegex = new Regex(@"=[^>]", RegexOptions.Compiled);
        private string _customFunctionsCSharpCode;
        private Type _delegateType;
        private string _functionSignature;
        protected string InitCodeAfterFunctionSignature = string.Empty;

        protected string AdditionalObjectsCode;
        protected string AdditionalUsings;
        protected string CustomFunctionsTslCode;
        protected FunctionType FunctionType;

        protected readonly ILogger Logger;
        protected string MainCSharpCode;
        protected string MainTslCode;
        protected NativeCompiler NativeCompiler;

        public ExpressionsEvaluator()
        {
            NativeCompiler = new NativeCompiler();
            NativeCompiler.AddDll(PathUtility.GetFullPath("Computator.NET.DataTypes.dll"));

            _tslCompiler = new TslCompiler();
            Logger = LogManager.GetLogger(GetType().FullName);
            AdditionalObjectsCode = "";
            AdditionalUsings = "";
        }


        private bool IsImplicit
            =>
                FunctionType == FunctionType.ComplexImplicit || FunctionType == FunctionType.Real2DImplicit ||
                FunctionType == FunctionType.Real3DImplicit;

        public Function Evaluate(string input, string customFunctionsCode, CalculationsMode calculationsMode)
        {
            FunctionType = FunctionTypeFromCalculationsMode(calculationsMode, _implicitFunctionRegex.IsMatch(input));


            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException(Strings.Expression_should_not_be_empty_);

            MainTslCode = input;
            CustomFunctionsTslCode =
                !string.IsNullOrWhiteSpace(customFunctionsCode)
                    ? customFunctionsCode
                    : "";


            var function = Compile();
            return new Function(function, FunctionType) {TslCode = MainTslCode, CsCode = MainCSharpCode};
        }


        private FunctionType FunctionTypeFromCalculationsMode(CalculationsMode calculationsMode, bool isImplicit)
        {
            switch (calculationsMode)
            {
                case CalculationsMode.Real:
                    return isImplicit ? FunctionType.Real2DImplicit : FunctionType.Real2D;
                case CalculationsMode.Complex:
                    return isImplicit ? FunctionType.ComplexImplicit : FunctionType.Complex;
                case CalculationsMode.Fxy:
                    return isImplicit ? FunctionType.Real3DImplicit : FunctionType.Real3D;

                case CalculationsMode.Error:
                default:
                    throw new ArgumentOutOfRangeException(nameof(calculationsMode), calculationsMode, null);
            }
        }

        protected void SetVariablesAndSignatures()
        {
            switch (FunctionType)
            {
                case FunctionType.Real2D:
                    //_tslCompiler.Variables.Add("x");
                    _functionSignature = LambdaX;
                    _delegateType = typeof(Func<double, double>);
                    break;
                case FunctionType.Complex:
                    //  _tslCompiler.Variables.Add("z");
                    // _tslCompiler.Variables.Add("i");
                    _functionSignature = LambdaZ;
                    _delegateType = typeof(Func<Complex, Complex>);
                    break;
                case FunctionType.Real2DImplicit:
                case FunctionType.Real3D:
                    //  _tslCompiler.Variables.Add("x");
                    //  _tslCompiler.Variables.Add("y");
                    _delegateType = typeof(Func<double, double, double>);
                    _functionSignature = LambdaXy;
                    break;
                case FunctionType.Real3DImplicit:
                    //    _tslCompiler.Variables.Add("x");
                    //    _tslCompiler.Variables.Add("y");
                    //    _tslCompiler.Variables.Add("z");
                    _functionSignature = LambdaXyz;
                    _delegateType = typeof(Func<double, double, double, double>);
                    break;
                case FunctionType.Scripting:
                    _delegateType = typeof(Action<Action<string>>);
                    _functionSignature = LambdaScript;
                    break;
            }
            _functionSignature += InitCodeAfterFunctionSignature;
        }

        private void TransformImplicitToExplicit()
        {
            if (IsImplicit)
            {
                var match = _implicitFunctionRegex.Match(MainCSharpCode);


                MainCSharpCode = MainCSharpCode.Substring(0, match.Index) + "-(" +
                                 MainCSharpCode.Substring(match.Index + 1) + ")";
            }
        }

        protected Delegate Compile()
        {
            SetVariablesAndSignatures();
            _customFunctionsCSharpCode = _tslCompiler.TransformToCSharp(CustomFunctionsTslCode);
            MainCSharpCode = _tslCompiler.TransformToCSharp(MainTslCode);
            TransformImplicitToExplicit();

            var fullCode = BuildCode();

            try
            {
                var assembly = NativeCompiler.Compile(fullCode);
                var cls = assembly.GetType("FunctionsCreatorNS.FunctionsCreator");
                var method = cls.GetMethod("CustomFunction", BindingFlags.Static | BindingFlags.Public);
                return Delegate.CreateDelegate(_delegateType, method);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"{ex.Message}{Environment.NewLine}{nameof(MainTslCode)} = '{MainTslCode}'{Environment.NewLine}{nameof(MainCSharpCode)} = '{MainCSharpCode}'{Environment.NewLine}{nameof(_customFunctionsCSharpCode)} = '{_customFunctionsCSharpCode}'");

                if (ex is CompilationException)
                    throw;

                var message =
                    Strings
                        .ErrorInExpressionSyntaxOneOfUsedFunctionsDoesNotExistIsIncompatibleWithGivenArgumentsOrYouJustMadeAMistakeWritingExpression;
                message += Environment.NewLine + Strings.Details;
                message += Environment.NewLine + ex.Message;

                Logger.Error(ex,$"{message}{Environment.NewLine}{nameof(MainTslCode)} = '{MainTslCode}'{Environment.NewLine}{nameof(MainCSharpCode)} = '{MainCSharpCode}'{Environment.NewLine}{nameof(_customFunctionsCSharpCode)} = '{_customFunctionsCSharpCode}'");

                throw new EvaluationException(message, ex);
            }
        }

        private string BuildCode()
        {
            var codeBuilder = new StringBuilder();
            NativeCompiler.CustomFunctionsStartOffsetLine = AdditionalUsings.Count(c => c == '\n') +
                                                            Begin.Count(c => c == '\n');
            NativeCompiler.CustomFunctionsEndOffsetLine = NativeCompiler.CustomFunctionsStartOffsetLine +
                                                          _customFunctionsCSharpCode.Count(c => c == '\n') + 1;

            NativeCompiler.MainCodeStarOffsetLine = NativeCompiler.CustomFunctionsEndOffsetLine - 1 +
                                                    _functionSignature.Count(c => c == '\n');
            NativeCompiler.MainCodeEndOffsetLine = NativeCompiler.MainCodeStarOffsetLine +
                                                   MainCSharpCode.Count(c => c == '\n') + 1;


            codeBuilder.Append(AdditionalUsings);
            codeBuilder.Append(Begin);


            codeBuilder.Append(_customFunctionsCSharpCode);
          

            codeBuilder.Append(_functionSignature);

/////
            codeBuilder.Append(MainCSharpCode);
            codeBuilder.Append(End.Replace("///*{additional_objects}*///", AdditionalObjectsCode));
            return codeBuilder.ToString();
        }
    }
}