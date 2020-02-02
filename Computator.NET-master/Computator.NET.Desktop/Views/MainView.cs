﻿#define PREFER_NATIVE_METHODS_OVER_SENDKING_SHORTCUT_KEYS
using System;
using System.Windows.Forms;
using Computator.NET.Core.Abstract.Views;
using Computator.NET.DataTypes.Properties;
using Computator.NET.Desktop.Services;

namespace Computator.NET.Desktop.Views
{
    public sealed partial class MainView : Form, IMainView
    {
        #region initialization and construction

        private MainView()
        {
            InitializeComponent();
            symbolicCalculationsTabPage.Enabled = false;
            this.MakeSureItsNotBiggerThanScreen();
            this.Icon = GraphicsResources.computator_net_icon;
        }

        public MainView(MenuStripView menuStripView, ToolBarView toolbarView, CalculationsView calculationsView, NumericalCalculationsView numericalCalculationsView, ScriptingView scriptingView, CustomFunctionsView customFunctionsView, ChartingView chartingView, ExpressionView expressionView) : this()
        {
            expressionView.Dock=DockStyle.Top;
            Controls.Add(expressionView);

            toolbarView.Dock=DockStyle.Top;
            Controls.Add(toolbarView);

            menuStripView.Dock = DockStyle.Top;
            Controls.Add(menuStripView);

            chartingView.Dock = DockStyle.Fill;
            chartingTabPage.Controls.Add(chartingView);

            calculationsView.Dock = DockStyle.Fill;
            calculationsTabPage.Controls.Add(calculationsView);

            numericalCalculationsView.Dock = DockStyle.Fill;
            numericalCalculationsTabPage.Controls.Add(numericalCalculationsView);

            scriptingView.Dock = DockStyle.Fill;
            scriptingTabPage.Controls.Add(scriptingView);

            customFunctionsView.Dock = DockStyle.Fill;
            customFunctionsTabPage.Controls.Add(customFunctionsView);
        }

        #endregion

        #region IMainForm


        public string ModeText
        {
            get { return modeToolStripDropDownButton.Text; }
            set { modeToolStripDropDownButton.Text = value; }
        }


        public event EventHandler ModeForcedToReal
        {
            add { dd212ToolStripMenuItem.Click += value; }
            remove { dd212ToolStripMenuItem.Click -= value; }
        }

        public event EventHandler ModeForcedToComplex
        {
            add { fdsfdsToolStripMenuItem.Click += value; }
            remove { fdsfdsToolStripMenuItem.Click -= value; }
        }

        public event EventHandler ModeForcedToFxy
        {
            add { mode3DFxyToolStripMenuItem.Click += value; }
            remove { mode3DFxyToolStripMenuItem.Click -= value; }
        }

        public string StatusText
        {
            set { toolStripStatusLabel1.Text = value; }
        }

        public int SelectedViewIndex
        {
            get { return tabControl1.SelectedIndex; }
            set { tabControl1.SelectedIndex = value; }
        }



        public event EventHandler SelectedViewChanged
        {
            add { tabControl1.SelectedIndexChanged += value; }
            remove { tabControl1.SelectedIndexChanged -= value; }
        }

        #endregion
    }
}