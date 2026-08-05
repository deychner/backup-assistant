using System.Collections.Generic;

namespace BackupAssistant.DataModels
{
    /// <summary>
    /// The persisted shape of the user's settings. This is what gets serialized to
    /// <c>settings.json</c>, so property names here are part of the on-disk format.
    /// </summary>
    public class BackupSettings
    {
        public string Source { get; set; } = string.Empty;

        public string Destination { get; set; } = string.Empty;

        public int BackupType { get; set; } = (int)DataModels.BackupType.Incremental;

        public IList<string> Filters { get; set; } = [];
    }
}
