using InterlockedExtensions = BackupAssistant.Extensions.Interlocked;

namespace BackupAssistant.Test.Extensions
{
    public class InterlockedTest
    {
        [Fact]
        public void Add_SingleThread_ReturnsSum()
        {
            float location = 10F;

            float result = InterlockedExtensions.Add(ref location, 5F);

            Assert.Equal(15F, result);
            Assert.Equal(15F, location);
        }

        [Fact]
        public async Task Add_ConcurrentCallers_ExercisesRetryLoop_AndSumsCorrectly()
        {
            float total = 0F;
            const int threadCount = 20;
            const int incrementsPerThread = 500;

            IEnumerable<Task> tasks = Enumerable.Range(0, threadCount).Select(threadIndex => Task.Run(() =>
            {
                for (int i = 0; i < incrementsPerThread; i++)
                {
                    _ = InterlockedExtensions.Add(ref total, 1F);
                }
            }));

            await Task.WhenAll(tasks);

            Assert.Equal((float)(threadCount * incrementsPerThread), total);
        }
    }
}
