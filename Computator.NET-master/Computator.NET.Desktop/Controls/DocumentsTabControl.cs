﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Computator.NET.Desktop.Services;
using Computator.NET.Localization;

namespace Computator.NET.Desktop.Controls
{
    public class DocumentsTabControl : TabControl
    {
        private TabPage addTabPage;
        private ToolStripMenuItem cloneTabToolStripMenuItem;
        private ToolStripMenuItem closeOtherTabsToolStripMenuItem;
        private ToolStripMenuItem closeTabsToTheLeftToolStripMenuItem;
        private ToolStripMenuItem closeTabsToTheRightToolStripMenuItem;
        private ToolStripMenuItem closeTabToolStripMenuItem;
        private ContextMenuStrip contextMenuStrip1;
        //public Dictionary<string,string> Documents { get; private set; }
        private int id = 1;
        private ToolStripMenuItem newTabToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        public DocumentsTabControl()
        {
            InitializeComponent();
            this.DpiScale();
            var h = Handle;
        }

        public void RenameTab(string oldName, string newName)
        {
            //if (TabPages.ContainsKey(oldName))
            {
                foreach (TabPage tabPage in TabPages)
                {
                    if (tabPage.Text == oldName)
                    {
                        tabPage.Text = newName;
                        break;
                    }
                }
            }
        }

        private void InitializeComponent()
        {
            var resources = new ComponentResourceManager(typeof(DocumentsTabControl));
            contextMenuStrip1 = new ContextMenuStrip();
            newTabToolStripMenuItem = new ToolStripMenuItem();
            cloneTabToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            closeTabToolStripMenuItem = new ToolStripMenuItem();
            closeOtherTabsToolStripMenuItem = new ToolStripMenuItem();
            closeTabsToTheRightToolStripMenuItem = new ToolStripMenuItem();
            closeTabsToTheLeftToolStripMenuItem = new ToolStripMenuItem();
            addTabPage = new TabPage();
            SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // this
            // 
            ContextMenuStrip = contextMenuStrip1;
            //TabPages.Add("NewFile1", "NewFile1", 1);
            TabPages.Add(addTabPage);
            Dock = DockStyle.Fill;
            Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Regular,
                GraphicsUnit.Point, 0);


            var imageList1 = new ImageList
            {
                ImageStream = (ImageListStreamer) resources.GetObject("imageList1.ImageStream"),
                TransparentColor = Color.Transparent
            };
            imageList1.Images.SetKeyName(0, "saved.png");
            imageList1.Images.SetKeyName(1, "unsaved.png");

            var imageList2 = new ImageList
            {
                TransparentColor = Color.Transparent,
                ImageSize = imageList1.ImageSize.DpiScale()
            };

            for (int i = 0; i < imageList1.Images.Count; i++)
            {
                imageList2.Images.Add(imageList1.Images[i]);
            }

            ImageList = imageList2;

            Location = new Point(0, 0);
            Margin = new Padding(5, 4, 5, 4);
            Name = "this";
            SelectedIndex = 0;
            Size = new Size(100, 23);
            TabIndex = 0;
            SelectedIndexChanged += this_SelectedIndexChanged;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[]
            {
                newTabToolStripMenuItem,
                cloneTabToolStripMenuItem,
                toolStripSeparator1,
                closeTabToolStripMenuItem,
                closeOtherTabsToolStripMenuItem,
                closeTabsToTheRightToolStripMenuItem,
                closeTabsToTheLeftToolStripMenuItem
            });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(191, 142);
            // 
            // newTabToolStripMenuItem
            // 
            newTabToolStripMenuItem.Name = "newTabToolStripMenuItem";
            newTabToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.T;
            newTabToolStripMenuItem.Size = new Size(190, 22);
            newTabToolStripMenuItem.Text = Strings.New_tab;
            newTabToolStripMenuItem.Click += newTabToolStripMenuItem_Click;
            // 
            // cloneTabToolStripMenuItem
            // 
            cloneTabToolStripMenuItem.Name = "cloneTabToolStripMenuItem";
            cloneTabToolStripMenuItem.Size = new Size(190, 22);
            cloneTabToolStripMenuItem.Text = Strings.Clone_tab;
            cloneTabToolStripMenuItem.Click += cloneTabToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(187, 6);
            // 
            // closeTabToolStripMenuItem
            // 
            closeTabToolStripMenuItem.Name = "closeTabToolStripMenuItem";
            closeTabToolStripMenuItem.Size = new Size(190, 22);
            closeTabToolStripMenuItem.Text = Strings.Close_tab;
            closeTabToolStripMenuItem.Click += closeTabToolStripMenuItem_Click;
            // 
            // closeOtherTabsToolStripMenuItem
            // 
            closeOtherTabsToolStripMenuItem.Name = "closeOtherTabsToolStripMenuItem";
            closeOtherTabsToolStripMenuItem.Size = new Size(190, 22);
            closeOtherTabsToolStripMenuItem.Text = Strings.Close_other_tabs;
            closeOtherTabsToolStripMenuItem.Click += closeOtherTabsToolStripMenuItem_Click;
            // 
            // closeTabsToTheRightToolStripMenuItem
            // 
            closeTabsToTheRightToolStripMenuItem.Name = "closeTabsToTheRightToolStripMenuItem";
            closeTabsToTheRightToolStripMenuItem.Size = new Size(190, 22);
            closeTabsToTheRightToolStripMenuItem.Text =
                Strings.Close_tabs_to_the_right;
            closeTabsToTheRightToolStripMenuItem.Click += closeTabsToTheRightToolStripMenuItem_Click;
            // 
            // closeTabsToTheLeftToolStripMenuItem
            // 
            closeTabsToTheLeftToolStripMenuItem.Name = "closeTabsToTheLeftToolStripMenuItem";
            closeTabsToTheLeftToolStripMenuItem.Size = new Size(190, 22);
            closeTabsToTheLeftToolStripMenuItem.Text =
                Strings.Close_tabs_to_the_left;
            closeTabsToTheLeftToolStripMenuItem.Click += closeTabsToTheLeftToolStripMenuItem_Click;
            // 
            // tabPage1
            // 

            // 
            // addTabPage
            // 
            addTabPage.Location = new Point(4, 33);
            addTabPage.Margin = new Padding(2);
            addTabPage.Name = "addTabPage";
            addTabPage.Padding = new Padding(2);
            //addTabPage.Size = new Size(1002, 0);
            addTabPage.TabIndex = 1;
            // ReSharper disable once LocalizableElement
            addTabPage.Text = "( + )";

            addTabPage.UseVisualStyleBackColor = true;

            // 
            // DocumentsTabControl
            // 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            // this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

            Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular,
                GraphicsUnit.Point, 0);

            //   addTabPage.Font = CustomFonts.GetMathFont(Font.Size);
            Margin = new Padding(0);
          //  MinimumSize = new Size(800, 27);
            Name = "DocumentsTabControl";
         //   Size = new Size(1010, 27);
            AutoSize = true;
            ResumeLayout(false);
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            
        }

        public void AddTab(string filename = "")
        {
            if (TabPages.ContainsKey(filename))
                return;

            var tabPage = new TabPage();
            if (string.IsNullOrEmpty(filename))
            {
                tabPage.Text = Strings.AddTab_NewFile + id;
                tabPage.ImageIndex = 1;
                tabPage.Name = Strings.AddTab_NewFile + id;
            }
            else
            {
                tabPage.Text = filename;
                tabPage.ImageIndex = 0;
                tabPage.Name = filename;
            }
            TabPages.Insert(TabPages.Count - 1, tabPage);
            SelectedIndex = TabPages.IndexOf(tabPage);
            id++;
        }

        private void closeTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPages.Remove(SelectedTab);
        }

        private void closeOtherTabsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (TabPage tabPage in TabPages)
            {
                if (tabPage != SelectedTab && tabPage != addTabPage)
                    TabPages.Remove(tabPage);
            }
            // Documents.Remove(this.SelectedTab.Text);

            //this.TabPages.();
        }

        private void closeTabsToTheRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (var i = SelectedIndex + 1; TabPages[i] != SelectedTab && TabPages[i] != addTabPage;)
            {
                TabPages.RemoveAt(i);
            }
        }

        private void closeTabsToTheLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (var i = 0; TabPages[i] != SelectedTab && TabPages[i] != addTabPage;)
            {
                TabPages.RemoveAt(i);
            }
        }

        private void newTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddTab();
        }

        private void cloneTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void this_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedTab == addTabPage)
            {
                AddTab();
            }
        }
    }
}