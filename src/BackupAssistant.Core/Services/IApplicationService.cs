namespace BackupAssistant.Services
{
    /// <summary>
    /// Abstracts the application-wide operations a view model needs from the host process.
    /// <para>
    /// <c>Environment.Exit</c> and static <c>Assembly.GetExecutingAssembly()</c> calls are not
    /// mockable, so they live behind this interface instead of in the view models.
    /// </para>
    /// </summary>
    public interface IApplicationService
    {
        /// <summary>
        /// Gets the displayable version of the running application.
        /// </summary>
        string ApplicationVersion { get; }

        /// <summary>
        /// Shuts the application down gracefully, so that logging gets a chance to flush rather
        /// than the process being torn down mid-write.
        /// </summary>
        void Shutdown();
    }
}
