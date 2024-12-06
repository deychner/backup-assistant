using BackupAssistant.Extensions;

namespace BackupAssistant.Test.Extensions
{
    internal enum Test
    {
        One,
        Two
    }
    
    public class EnumCollectionExtensionTest
    {
        [Fact]
        public void ProvideValue()
        {
            EnumCollectionExtension collection = new() { EnumType = typeof(Test) };
            object? value = collection.ProvideValue(null!);

            Assert.NotNull(value);
            Assert.IsType<List<object>>(value);

            List<object> list = (List<object>)value;
            Assert.Equal(2, list.Count);
            Assert.Equal(Test.One, list[0]);
            Assert.Equal(Test.Two, list[1]);
        }

        [Fact]
        public void ProvideValue_NullType()
        {
            EnumCollectionExtension collection = new();
            object? value = collection.ProvideValue(null!);

            Assert.Null(value);
        }
    }
}
