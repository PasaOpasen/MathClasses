using System.Diagnostics;
using Computator.NET.DataTypes;
using Computator.NET.DataTypes.Information;
using Computator.NET.Localization.Menu;

namespace Computator.NET.Core.Menu.Commands.HelpCommands
{
    public class BugReportingCommand : CommandBase
    {
        public BugReportingCommand()
        {
            //this.ShortcutKeyString = "Shift+6";
            //this.Icon = Resources.exponentation;
            Text = MenuStrings.BugReporting_Text;
            ToolTip = MenuStrings.BugReporting_Text;
        }


        public override void Execute()
        {
            Process.Start(AppRelatedUrls.IssuesUrl);
            //new BugReportingForm().ShowDialog( /*this*/);
        }
    }
}