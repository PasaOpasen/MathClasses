﻿using System;
using System.ComponentModel;
using System.Windows.Forms;
using Computator.NET.Core.Benchmarking;
using Computator.NET.DataTypes.Properties;
using Computator.NET.DataTypes.Text;
using Computator.NET.Localization;

namespace Computator.NET.Desktop.Benchmarking
{
    public partial class BenchmarkForm : Form
    {
        private readonly Benchmark benchmark;

        public BenchmarkForm()
        {
            InitializeComponent();

            benchmark = new Benchmark();


            functionsTestBackgroundWorker.DoWork += benchmark.mathFunctionsCalculationSpeedTest;
            functionsTestBackgroundWorker.ProgressChanged += FunctionsTestBackgroundWorker_ProgressChanged;
            functionsTestBackgroundWorker.RunWorkerCompleted += FunctionsTestBackgroundWorker_RunWorkerCompleted;

            memoryTestBackgroundWorker.DoWork += benchmark.memoryAllocationSpeedTest;
            memoryTestBackgroundWorker.ProgressChanged += memoryTestBackgroundWorker_ProgressChanged;
            memoryTestBackgroundWorker.RunWorkerCompleted += memoryTestBackgroundWorker_RunWorkerCompleted;
            this.Icon = GraphicsResources.computator_net_icon;
            this.memoryTestRichTextBox.Font = CustomFonts.GetScriptingFont(10.2F);
            this.functionsTestRichTextBox.Font = CustomFonts.GetScriptingFont(10.2F);
        }

        private void FunctionsTestBackgroundWorker_RunWorkerCompleted(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show(
                    Strings.MathFunctionsCalculationSpeedTestCancelledByUser, Strings.Canceled);
                functionsTestProgressBar.Value = 0;
            }

            else if (e.Error != null)
            {
                // ReSharper disable LocalizableElement
                MessageBox.Show(Strings.Error + ": " + e.Error.Message,
                    Strings.Error + "!");
                // ReSharper restore LocalizableElement
                functionsTestProgressBar.Value = 0;
            }

            else
            {
                MessageBox.Show(
                    Strings.MathFunctionsCalculationSpeedTestDoneSuccesfullyCheckOutYourPointsResults,
                    Strings.Done);
                // ReSharper disable once LocalizableElement
                functionsTestRichTextBox.Text = DateTime.Now.ToShortDateString() + " " +
                                                DateTime.Now.ToShortTimeString() + Strings.Result +
                                                benchmark.Points +
                                                Strings.Points + functionsTestRichTextBox.Text;
            }
        }

        private void FunctionsTestBackgroundWorker_ProgressChanged(object sender,
            ProgressChangedEventArgs e)
        {
            functionsTestProgressBar.Value = e.ProgressPercentage;
        }

        private void memoryTestBackgroundWorker_RunWorkerCompleted(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show(
                    Strings.MemoryAllocationSpeedTestCancelledByUser, Strings.Canceled);
                memoryTestProgressBar.Value = 0;
            }

            else if (e.Error != null)
            {
                // ReSharper disable LocalizableElement
                MessageBox.Show(Strings.Error + ": " + e.Error.Message, Strings.Error + "!");
                // ReSharper restore LocalizableElement
                memoryTestProgressBar.Value = 0;
            }

            else
            {
                MessageBox.Show(
                    Strings.MemoryAllocationSpeedTestDoneSuccesfullyCheckOutYourPointsResult,
                    Strings.Done);
                // ReSharper disable once LocalizableElement
                memoryTestRichTextBox.Text = DateTime.Now.ToShortDateString() + " " +
                                             DateTime.Now.ToShortTimeString() +
                                             Strings.Result + benchmark.Points +
                                             Strings.Points +
                                             memoryTestRichTextBox.Text;
            }
        }

        private void memoryTestBackgroundWorker_ProgressChanged(object sender,
            ProgressChangedEventArgs e)
        {
            memoryTestProgressBar.Value = e.ProgressPercentage;
        }

        private void startMemoryTestButton_Click(object sender, EventArgs e)
        {
            if (!memoryTestBackgroundWorker.IsBusy)
                memoryTestBackgroundWorker.RunWorkerAsync();
        }

        // private Benchmark functionsBenchmark;

        private void cancelMemoryTestButton_Click(object sender, EventArgs e)
        {
            if (memoryTestBackgroundWorker.IsBusy)
                memoryTestBackgroundWorker.CancelAsync();
        }

        private void startFunctionsTestButton_Click(object sender, EventArgs e)
        {
            if (!functionsTestBackgroundWorker.IsBusy)
                functionsTestBackgroundWorker.RunWorkerAsync();
        }

        private void cancelFunctionsTestButton_Click(object sender, EventArgs e)
        {
            if (functionsTestBackgroundWorker.IsBusy)
                functionsTestBackgroundWorker.CancelAsync();
        }
    }
}