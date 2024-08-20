using BackupAssistant.Extensions;

namespace BackupAssistant.Test.Extensions
{
    public class DataTypeExtensionsTest
    {
        [Fact]
        public void ReplaceFirst_Nothing()
        {
            string input = "string does not contain text to replace";
            string actual = input.ReplaceFirst("old", "new");

            Assert.Equal(input, actual);
        }

        [Fact]
        public void ReplaceFirst_SingleByDefault()
        {
            string input = "one instance of old to replace";
            string actual = input.ReplaceFirst("old", "new");

            Assert.Equal("one instance of new to replace", actual);
        }

        [Fact]
        public void ReplaceFirst_Single()
        {
            string input = "multiple instances of old to replace gets old";
            string actual = input.ReplaceFirst("old", "new");

            Assert.Equal("multiple instances of new to replace gets old", actual);
        }
    }
}
