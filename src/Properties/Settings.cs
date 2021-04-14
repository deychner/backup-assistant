using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace BackupAssistant.Properties
{
    internal sealed partial class Settings : ApplicationSettingsBase
    {
        void Handle_SettingsLoaded(object sender, SettingsLoadedEventArgs e)
        {
            try
            {
                if (this.UpgradeRequired)
                {
                    this.Upgrade();
                    this.UpgradeRequired = false;
                    this.Save();
                }
            }
            catch
            {
                // The configuration file could not be parsed.
            }
        }
    }
}
