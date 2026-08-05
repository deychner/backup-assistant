using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BackupAssistant.Services
{
    public interface IBackupService
    {
        Task RunFullBackupAsync(
            string source,
            string destination,
            ICollection<string> filterItems,
            IProgress<BackupProgress> progress,
            CancellationToken token);

        Task RunIncrementalBackupAsync(
            string source,
            string destination,
            ICollection<string> filterItems,
            IProgress<BackupProgress> progress,
            CancellationToken token);
    }

    public class BackupProgress
    {
        public int? Progress { get; set; }
        public bool? IsIndeterminate { get; set; }
        public string? Status { get; set; } = string.Empty;
    }
}