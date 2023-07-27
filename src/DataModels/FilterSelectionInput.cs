using System.Collections.Generic;

namespace BackupAssistant.DataModels
{
    public class FilterSelectionInput
    {
        public string RootPath { get; set; } = string.Empty;

        public IEnumerable<string>? ExistingFilters { get; set; }
    }
}
