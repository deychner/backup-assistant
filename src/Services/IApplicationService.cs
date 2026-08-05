namespace BackupAssistant.Services
{
    public interface IApplicationService
    {
        string ApplicationVersion { get; }

        void Shutdown();
    }
}
