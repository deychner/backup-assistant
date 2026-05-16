using Microsoft.UI.Xaml.Markup;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BackupAssistant.Extensions
{
    public class EnumCollectionExtension : MarkupExtension
    {
        public Type? EnumType { get; set; }

        protected override object? ProvideValue()
        {
            return EnumType != null ? CreateEnumValueList(EnumType) : (object?)default;
        }

        private static List<object> CreateEnumValueList(Type enumType)
        {
            return [.. Enum.GetNames(enumType).Select(name => Enum.Parse(enumType, name))];
        }
    }
}

