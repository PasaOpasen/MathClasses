﻿using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Computator.NET.Core.Abstract.Controls;
using Computator.NET.Core.Model;
using Computator.NET.Localization;

namespace Computator.NET.Desktop.Controls
{
    public sealed class DirectoryTree : TreeView, IDirectoryTree
    {
        private string _path;
        //private ContextMenu ctxMenu;
        //private ButtonClick _buttonClicked;

        private TreeNode ctxNode;
        private int id = 1;

        public DirectoryTree()
        {
            InitializeComponent();
            AfterSelect += _AfterSelect;

            ContextMenu = new ContextMenu(new[]
            {
                new MenuItem(Strings.Directory_tree_New_file, (o, e) =>
                {
                    var attr = File.GetAttributes(ctxNode.FullPath);

                    var newNode = attr.HasFlag(FileAttributes.Directory)
                        ? ctxNode.Nodes.Add(Strings.Directory_tree_New_file + " " + id)
                        : ctxNode.Parent.Nodes.Add(Strings.Directory_tree_New_file + " " + id);
                    id++;

                    LabelEdit = true;
                    if (!newNode.IsEditing)
                    {
                        newNode.BeginEdit();
                    }
                    //CodeEditorWrapper?.NewDocument();
                    //RefreshDisplay();
                    //_buttonClicked = ButtonClick.New;
                }),
                new MenuItem(Strings.Rename_file, (o, e) =>
                {
                    //oldPath = ctxNode.FullPath;
                    if (ctxNode == TopNode)
                        return;
                    LabelEdit = true;

                    if (!ctxNode.IsEditing)
                    {
                        ctxNode.BeginEdit();
                        //ctxNode.EndEdit(true);
                        /* if (oldPath != ctxNode.FullPath)
                    {
                        System.IO.File.Move(oldPath, ctxNode.FullPath);
                        CodeEditorWrapper?.RenameDocument(oldPath, ctxNode.FullPath);
                    }*/
                        //RefreshDisplay();
                        //_buttonClicked = ButtonClick.Rename;
                    }
                }),
                new MenuItem(Strings.Delete_file, (o, e) =>
                {
                    if (ctxNode == TopNode)
                        return;
                    CodeEditorWrapper?.RemoveTab(ctxNode.FullPath);
                    File.Delete(ctxNode.FullPath);
                    Nodes.Remove(ctxNode);
                    SelectedNode = TopNode;
                    //RefreshDisplay();
                    //_buttonClicked =  ButtonClick.Delete;
                })
            });

            NodeMouseClick += DirectoryTree_NodeMouseDoubleClick;
            //NodeMouseDoubleClick += DirectoryTree_NodeMouseDoubleClick;
            AfterLabelEdit += treeView1_AfterLabelEdit;
        }

        public IDocumentsEditor CodeEditorWrapper { get; set; }

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                if (_path != null)
                    RefreshDisplay();
            }
        }

        public event DirectorySelectedDelegate DirectorySelected;

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (CodeEditorWrapper == null)
                return;
            //MessageBox.Show(e.Label);
            //MessageBox.Show(e.Node.Text);
            if (e.Label == null && !File.Exists(e.Node.FullPath))
                // there was no change in default name and file does not yet exists - we need to create a new one
            {
                createFile(e.Node.FullPath);
                e.Node.EndEdit(false);
                return;
            }
            if (e.Label == null) //no changes to already existing file's name - do nothing
                return;

            if (e.Label.Length > 0)
            {
                if (e.Label.IndexOfAny(new[] {'/', '\\', ':', '*', '<', '>', '|', '?', '"'}) == -1)
                {
                    // Stop editing without canceling the label change.


                    if (e.Node.Text != e.Label)
                    {
                        var oldPath = e.Node.FullPath;
                        e.Node.Text = e.Label;
                        if (oldPath != e.Node.FullPath || !File.Exists(oldPath))
                        {
                            File.Delete(e.Node.FullPath);
                            //if(File.Exists(oldPath))


                            var containsDocument = CodeEditorWrapper.ContainsDocument(oldPath);
                            if (containsDocument)
                            {
                                File.Move(oldPath, e.Node.FullPath);
                                CodeEditorWrapper?.RenameDocument(oldPath, e.Node.FullPath);
                            }
                            else
                            {
                                createFile(e.Node.FullPath);
                            }
                        }
                    }

                    e.Node.EndEdit(false);
                }
                else
                {
                    // Cancel the label edit action, inform the user, and 
                    //  place the node in edit mode again.
                    e.CancelEdit = true;
                    MessageBox.Show(Strings.InvalidLabel +
                                    Strings.The_invalid_characters_are,
                        Strings.Label_Edit_error);
                    e.Node.BeginEdit();
                }
            }
            else
            {
                /* Cancel the label edit action, inform the user, and 
                       place the node in edit mode again. */
                e.CancelEdit = true;
                MessageBox.Show(
                    Strings.InvalidLabel +
                    Strings.The_label_cannot_be_blank
                    ,
                    Strings.Label_Edit_error);
                e.Node.BeginEdit();
            }
        }

        private void createFile(string fullPath)
        {
            var sr = File.CreateText(fullPath);
            sr.Close();
            CodeEditorWrapper.NewDocument(fullPath);
        }

        private void DirectoryTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //ContextMenu.Show();
            if (e.Button == MouseButtons.Right)
            {
                ctxNode = e.Node;
                //ctxMenu.Show(this,e.Location);
            }
        }

        private void InitializeComponent()
        {
            Font = new Font(FontFamily.GenericSansSerif, 9.0F, FontStyle.Regular, GraphicsUnit.Point);
        }

        private void _AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (CodeEditorWrapper == null) return;

            if (CodeEditorWrapper.ContainsDocument(SelectedNode.FullPath))
            {
                if (CodeEditorWrapper.CurrentFileName != SelectedNode.FullPath)
                {
                    CodeEditorWrapper.SwitchTab(SelectedNode.FullPath);
                    //CodeEditorWrapper.CurrentFileName = this.SelectedNode.FullPath;
                    //CodeEditorWrapper.SwitchDocument(this.SelectedNode.FullPath);
                }
            }
            else if (File.Exists(SelectedNode.FullPath))
            {
                CodeEditorWrapper.NewDocument(SelectedNode.FullPath);
            }
        }

        // This is public so a Refresh can be triggered manually.
        public void RefreshDisplay()
        {
            // Erase the existing tree.
            Nodes.Clear();

            // Set the first node.
            var rootNode = new TreeNode(_path);
            Nodes.Add(rootNode);

            // Fill the first level and expand it.

            Fill(rootNode);
            Nodes[0].Expand();

            if (ctxNode == null)
                ctxNode = rootNode;
        }

        private void Fill(TreeNode dirNode)
        {
            var dir = new DirectoryInfo(dirNode.FullPath);

            // An exception could be thrown in this code if you don't
            // have sufficient security permissions for a file or directory.
            // You can catch and then ignore this exception.

            foreach (var dirItem in dir.GetDirectories())
            {
                // Add node for the directory.
                var newNode = new TreeNode(dirItem.Name);

                dirNode.Nodes.Add(newNode);
                newNode.Nodes.Add("*");
            }
            foreach (var dirItem in dir.GetFiles())
            {
                // Add node for the directory.
                var newNode = new TreeNode(dirItem.Name);
                dirNode.Nodes.Add(newNode);
                //newNode.Nodes.Add("*");
            }
        }

        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            base.OnBeforeExpand(e);

            // If a dummy node is found, remove it and read the real directory list.
            // ReSharper disable once LocalizableElement
            if (e.Node.Nodes[0].Text == "*")
            {
                e.Node.Nodes.Clear();
                Fill(e.Node);
            }
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);

            // Raise the DirectorySelected event.
            DirectorySelected?.Invoke(this,
                new DirectorySelectedEventArgs(e.Node.FullPath));
        }
    }
}