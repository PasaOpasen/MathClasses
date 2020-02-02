using Computator.NET.Core.Abstract.Controls;
using Computator.NET.Core.Abstract.Services;
using Computator.NET.Core.Model;
using Computator.NET.Localization.Menu;

namespace Computator.NET.Core.Menu.Commands.EditCommands
{
    public class RedoCommand : CommandBase
    {
        private readonly ICanFileEdit customFunctionsCodeEditor;
        private readonly ICanFileEdit scriptingCodeEditor;
        private ISharedViewState _sharedViewState;
        private IApplicationManager _applicationManager;

        public RedoCommand(ICanFileEdit scriptingCodeEditor, ICanFileEdit customFunctionsCodeEditor, ISharedViewState sharedViewState, IApplicationManager applicationManager)
        {
            //this.Icon = Resources.copyToolStripButtonImage;
            Text = MenuStrings.redoToolStripMenuItem_Text;
            ToolTip = MenuStrings.redoToolStripMenuItem_Text;
            ShortcutKeyString = "Ctrl+Y";
            this.scriptingCodeEditor = scriptingCodeEditor;
            this.customFunctionsCodeEditor = customFunctionsCodeEditor;
            _sharedViewState = sharedViewState;
            _applicationManager = applicationManager;
            // this.mainFormView = mainFormView;
        }


        public override void Execute()
        {
            if ((int) _sharedViewState.CurrentView < 4)
            {
                _applicationManager.SendStringAsKey("^Y");
                //expressionTextBox.do()
            }
            else if ((int) _sharedViewState.CurrentView == 4)
                //scriptingCodeEditor.Focus();
            {
                if (scriptingCodeEditor.Focused)
                    scriptingCodeEditor.Redo();
                else
                    _applicationManager.SendStringAsKey("^Y");
            }
            else
            {
                if (customFunctionsCodeEditor.Focused)
                    customFunctionsCodeEditor.Redo();
                else
                    _applicationManager.SendStringAsKey("^Y");
            }

            _applicationManager.SendStringAsKey("^Y");
        }
    }
}