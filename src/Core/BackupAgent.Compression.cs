namespace BackupAssistant.Core
{
    public partial class BackupAgent
    {
        string ShrinkSourceFileName(string fileName)
        {
            return GetAbbreviatedFileName(fileName, _caller.SourcePath);
        }

        string ShrinkDestinationFileName(string fileName)
        {
            return GetAbbreviatedFileName(fileName, _caller.DestinationPath);
        }

        string ExpandSourceFileName(string fileName)
        {
            return GetFullFileName(fileName, _caller.SourcePath);
        }
        string ExpandDestinationFileName(string fileName)
        {
            return GetFullFileName(fileName, _caller.DestinationPath);
        }

        string GetFullFileName(string abbreviatedFileName, string lengthenString)
        {
            return abbreviatedFileName.Replace("...", lengthenString);
        }

        string GetAbbreviatedFileName(string fileName, string shortenString)
        {
            return fileName.Replace(shortenString, "...");
        }
    }
}
