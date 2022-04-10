namespace BackupAssistant.DataModels
{
    internal class FilterItem
    {
        public bool IsChecked { get; set; } = false;
        public string Path { get; set; } = string.Empty;
    }
}
