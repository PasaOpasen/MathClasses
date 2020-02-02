﻿// Computator.NET Copyright © 2016 - 2017 Pawel Troka

using System;

namespace Computator.NET.Core.Abstract.Views
{
    public interface ICalculationsView
    {
        string XLabel { set; }
        string YLabel { set; }

        bool YVisible { set; }

        double X { get; }
        double Y { get; }
        event EventHandler CalculateClicked;

        void AddResult(string expression, string arguments, string result);
    }
}