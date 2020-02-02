﻿using System;
using System.Linq;
using Computator.NET.Core.Abstract.Controls;
using Computator.NET.Core.Abstract.Views;
using Computator.NET.Core.Evaluation;
using Computator.NET.Core.Model;
using Computator.NET.Core.NumericalCalculations;
using Computator.NET.Core.Services.ErrorHandling;
using Computator.NET.DataTypes;
using Computator.NET.DataTypes.Events;
using Computator.NET.Localization;

namespace Computator.NET.Core.Presenters
{
    public class NumericalCalculationsPresenter
    {
        private ISharedViewState _sharedViewState;
        private readonly ITextProvider _expressionTextProvider;
        private readonly Type _derivationType = typeof(Func<Func<double, double>, double, uint, double, double>);

        private readonly IErrorHandler _errorHandler;

        private readonly Type _functionRootType =
            typeof(Func<Func<double, double>, double, double, double, uint, double>);


        private readonly Type _integrationType = typeof(Func<Func<double, double>, double, double, double, double>);
        private readonly INumericalCalculationsView _view;


        private readonly IExpressionsEvaluator expressionsEvaluator;


        private CalculationsMode _calculationsMode;


        public NumericalCalculationsPresenter(INumericalCalculationsView view, IErrorHandler errorHandler, ISharedViewState sharedViewState, IExceptionsHandler exceptionsHandler, ITextProvider expressionTextProvider, ICodeEditorView customFunctionsEditor, IExpressionsEvaluator expressionsEvaluator)
        {
            _view = view;
            _errorHandler = errorHandler;
            _sharedViewState = sharedViewState;
            _exceptionsHandler = exceptionsHandler;
            _expressionTextProvider = expressionTextProvider;
            this.customFunctionsEditor = customFunctionsEditor;
            this.expressionsEvaluator = expressionsEvaluator;
            _view.SetOperations(NumericalMethodsInfo.Instance._methods.Keys.ToArray());
            _view.SelectedOperation = NumericalMethodsInfo.Instance._methods.Keys.First();
            _view.OperationChanged += _view_OperationChanged;

            _view_OperationChanged(null, null);

            EventAggregator.Instance.Subscribe<CalculationsModeChangedEvent>(c => _calculationsMode = c.CalculationsMode);

            _sharedViewState.DefaultActions[ViewName.NumericalCalculations] = _view_ComputeClicked;
            _view.ComputeClicked += _view_ComputeClicked;
        }

        public T Cast<T>(object input)
        {
            return (T) input;
        }

        private void _view_ComputeClicked(object sender, EventArgs e)
        {
            if (_calculationsMode == CalculationsMode.Real)
            {
                try
                {
                    customFunctionsEditor.ClearHighlightedErrors();
                    var function = expressionsEvaluator.Evaluate(_expressionTextProvider.Text,
                        customFunctionsEditor.Text, _calculationsMode);

                    Func<double, double> fx = x => function.Evaluate(x);

                    var result = double.NaN;
                    var eps = _view.Epsilon;

                    if (_view.SelectedOperation == Strings.Derivative ||
                        _view.SelectedOperation == Strings.Function_root)
                    {
                        if (double.IsNaN(eps))
                        {
                            _errorHandler.DisplayError(Strings.GivenΕIsNotValid, Strings.Error);
                            return;
                        }
                        if (!(eps > 0.0) || !(eps <= 1))
                        {
                            _errorHandler.DisplayError(
                                Strings.GivenΕIsNotValidΕShouldBeSmallPositiveNumber, Strings.Error);
                            return;
                        }
                    }

                    var parametersStr = "";


                    if (_view.SelectedOperation == Strings.Integral)
                    {
                        result =
                            ((dynamic)
                                Convert.ChangeType(
                                    NumericalMethodsInfo.Instance._methods[_view.SelectedOperation][_view.SelectedMethod
                                        ],
                                    _integrationType))
                                (fx, _view.A, _view.B, _view.N);
                        parametersStr = $"a={_view.A.ToMathString()}; b={_view.B.ToMathString()}; N={_view.N}";
                    }
                    else if (_view.SelectedOperation == Strings.Derivative)
                    {
                        result =
                            ((dynamic)
                                Convert.ChangeType(
                                    NumericalMethodsInfo.Instance._methods[_view.SelectedOperation][_view.SelectedMethod
                                        ],
                                    _derivationType))
                                (fx, _view.X, _view.Order, eps);
                        parametersStr = $"n={_view.Order}; x={_view.X.ToMathString()}; ε={eps.ToMathString()}";
                    }
                    else if (_view.SelectedOperation == Strings.Function_root)
                    {
                        result =
                            ((dynamic)
                                Convert.ChangeType(
                                    NumericalMethodsInfo.Instance._methods[_view.SelectedOperation][_view.SelectedMethod
                                        ],
                                    _functionRootType))
                                (fx, _view.A, _view.B, eps, _view.N);
                        parametersStr =
                            $"a={_view.A.ToMathString()}; b={_view.B.ToMathString()}; ε={eps.ToMathString()}; N={_view.N}";
                    }

                    _view.AddResult(_expressionTextProvider.Text,
                        _view.SelectedOperation,
                        _view.SelectedMethod,
                        parametersStr,
                        result.ToMathString());
                }
                catch (Exception ex)
                {
                    _exceptionsHandler.HandleException(ex);
                }
            }
            else
            {
                _errorHandler.DisplayError(
                    Strings
                        .Only_Real_mode_is_supported_in_Numerical_calculations_right_now__more_to_come_in_next_versions_ +
                    Environment.NewLine +
                    Strings.Check__Real___f_x___mode,
                    Strings.Warning_);
            }
        }
        private IExceptionsHandler _exceptionsHandler;
        private ICodeEditorView customFunctionsEditor;

        private void _view_OperationChanged(object sender, EventArgs e)
        {
            _view.SetMethods(NumericalMethodsInfo.Instance._methods[_view.SelectedOperation].Keys.ToArray());
            _view.SelectedMethod = NumericalMethodsInfo.Instance._methods[_view.SelectedOperation].Keys.First();

            _view.StepsVisible = _view.IntervalVisible = _view.DerrivativeVisible = _view.ErrorVisible = false;
            if (_view.SelectedOperation == Strings.Integral)
            {
                _view.StepsVisible = _view.IntervalVisible = true;
            }
            else if (_view.SelectedOperation == Strings.Derivative)
            {
                _view.DerrivativeVisible = _view.ErrorVisible = true;
            }
            else if (_view.SelectedOperation == Strings.Function_root)
            {
                _view.StepsVisible = _view.IntervalVisible = _view.ErrorVisible = true;
            }
        }
    }
}