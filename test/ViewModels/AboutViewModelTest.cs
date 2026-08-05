using BackupAssistant.Services;
using BackupAssistant.ViewModels;
using Moq;

namespace BackupAssistant.Test.ViewModels
{
    public class AboutViewModelTest
    {
        [Fact]
        public void ApplicationVersion()
        {
            Mock<IApplicationService> applicationServiceMock = new(MockBehavior.Strict);
            _ = applicationServiceMock.SetupGet(a => a.Version).Returns("2026.7.2.1");

            AboutViewModel instance = new(applicationServiceMock.Object);

            Assert.Equal("2026.7.2.1", instance.ApplicationVersion);
            applicationServiceMock.VerifyAll();
        }
    }
}
