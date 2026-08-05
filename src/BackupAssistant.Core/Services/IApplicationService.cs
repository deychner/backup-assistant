namespace BackupAssistant.Services
{
    /// <summary>
    /// Abstracts the application-wide operations a view model needs from the host process.
    /// </summary>
    public interface IApplicationService
    {
        /// <summary>
        /// Gets the displayable version of the running application.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Shuts the application down.
        /// </summary>
        void Exit();
    }
}
