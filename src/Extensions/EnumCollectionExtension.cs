using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

namespace BackupAssistant.Extensions
{
    public class EnumCollectionExtension : MarkupExtension
    {
        public Type? EnumType { get; set; }

        public override object? ProvideValue(IServiceProvider _)
        {
            if (EnumType != null)
            {
                return CreateEnumValueList(EnumType);
            }

            return default;
        }

        private static List<object> CreateEnumValueList(Type enumType)
        {
            return [.. Enum.GetNames(enumType).Select(name => Enum.Parse(enumType, name))];
        }
    }
}
