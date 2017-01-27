using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace LLVM.ClangFormat
{
    public sealed class TypeConverterUtils
    {
        // Generic enum type converter that automatically converts enums <-> strings by
        // injecting/removing spaces into camel-cased strings. You must derive your own
        // type and pass the typeof(YourEnum) to the base constructor, rather than provide
        // a generic type due to limitations of C# attributes.
        public class EnumStringTypeConverter : EnumConverter
        {
            private Type enumType;
            public EnumStringTypeConverter(Type enumType) : base(enumType)
            {
                this.enumType = enumType;
            }

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string))
                    return true;

                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                string str = value as string;

                if (str != null)
                {
                    str.Replace(" ", "");
                    return Enum.Parse(enumType, str.Replace(" ", ""), false);
                }

                return base.ConvertFrom(context, culture, value);
            }

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string))
                {
                    var enumString = ((Enum)value).ToString();
                    return Regex.Replace(enumString, "([A-Z])", " $1").Trim();
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}
