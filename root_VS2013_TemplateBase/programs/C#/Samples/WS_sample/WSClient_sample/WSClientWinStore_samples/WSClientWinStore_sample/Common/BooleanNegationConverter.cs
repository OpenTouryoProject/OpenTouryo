using System;
using Windows.UI.Xaml.Data;

namespace WSClientWinStore_sample.Common
{
    /// <summary>
    /// true を false に、および false を true に変換する値コンバーター。
    /// </summary>
    public sealed class BooleanNegationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !(value is bool && (bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return !(value is bool && (bool)value);
        }
    }
}
