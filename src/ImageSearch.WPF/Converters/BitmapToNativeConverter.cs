using System;
using System.Globalization;
using System.Windows.Data;
using Splat;

namespace ImageSearch.WPF.Converters
{
    internal sealed class BitmapToNativeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IBitmap bitmap)
            {
                return bitmap.ToNative();
            }

            return Binding.DoNothing;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
