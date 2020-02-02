﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Computator.NET.Core.Abstract;
using Computator.NET.Core.Functions;
using Computator.NET.Desktop.Benchmarking;

namespace Computator.NET.Desktop.Dialogs
{
    public class WinFormsDialogFactory : IDialogFactory
    {
        private readonly Dictionary<string, Type> _dialogsTypes=new Dictionary<string, Type>
        {
            {"about",  typeof(AboutBox1)},
            {"bugs",  typeof(BugReportingForm)},
            {"benchmark",  typeof(BenchmarkForm)},
            {"changelog",  typeof(ChangelogForm)},
            {"loading",  typeof(LoadingScreen)},
            {"read",  typeof(ReadForm)},
            {"settings",  typeof(SettingsForm)},
        };

        public bool ShowDialog(string name)
        {
            return (Activator.CreateInstance(_dialogsTypes[name]) as Form)?.ShowDialog() == DialogResult.OK;
        }
    }
}