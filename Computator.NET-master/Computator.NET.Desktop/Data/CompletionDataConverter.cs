#if !__MonoCS__
using System.Collections.Generic;
using System.Linq;
using Computator.NET.Core.Autocompletion;
using Computator.NET.Desktop.Controls.CodeEditors.AvalonEdit;

namespace Computator.NET.Desktop.Data
{
    public static class CompletionDataConverter
    {
        public static List<CompletionData>
            ConvertAutocompleteItemsToCompletionDatas(
                AutocompleteItem[] autocompleteItems)
        {
            return
                autocompleteItems.Select(ToCompletionData).ToList();
        }

        public static CompletionData ToCompletionData(AutocompleteItem autocompleteItem)
        {
            return new CompletionData(autocompleteItem.Text, autocompleteItem.MenuText, autocompleteItem.ImageIndex, autocompleteItem.Details);
        }
    }
}
#endif