using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace BackupAssistant.Services
{
    [ExcludeFromCodeCoverage]
    public class SettingsService : ISettingsService
    {
        public int BackupType
        {
            get => Properties.Settings.Default.BackupType;
            set { Properties.Settings.Default.BackupType = value; }
        }

        public string Destination
        {
            get => Properties.Settings.Default.Destination;
            set { Properties.Settings.Default.Destination = value; }
        }

        public StringCollection Filters
        {
            get => Properties.Settings.Default.Filters;
            set { Properties.Settings.Default.Filters = value; }
        }

        public string Source
        {
            get => Properties.Settings.Default.Source;
            set { Properties.Settings.Default.Source = value; }
        }

        public void Save()
        {
            Properties.Settings.Default.Save();
        }
    }
}
