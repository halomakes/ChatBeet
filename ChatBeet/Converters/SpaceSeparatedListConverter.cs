﻿using System;
using System.ComponentModel;
using System.Globalization;

namespace ChatBeet.Converters;

[TypeConverter(typeof(string[]))]
public class SpaceSeparatedListConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        if (sourceType == typeof(string))
        {
            return true;
        }
        return base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string s)
        {
            return s.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }
        return base.ConvertFrom(context, culture, value);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType == typeof(string))
        {
            return value.ToString();
        }
        return base.ConvertTo(context, culture, value, destinationType);
    }
}
