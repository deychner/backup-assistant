using BackupAssistant.Services;
using BackupAssistant.ViewModels;
using Moq;

namespace BackupAssistant.Test.ViewModels
{
    public class AboutViewModelTest
    {
        [Fact]
        public void ApplicationVersion_ReturnsValueFromApplicationService()
        {
            Mock<IApplicationService> applicationServiceMock = new(MockBehavior.Strict);
            _ = applicationServiceMock.Setup(a => a.ApplicationVersion).Returns("1.2.3.4");

            AboutViewModel viewModel = new(applicationServiceMock.Object);

            Assert.Equal("1.2.3.4", viewModel.ApplicationVersion);
        }
    }
}
