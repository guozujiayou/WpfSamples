using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WpfPrismDemo.EnumDto;

namespace WpfPrismDemo.Converters
{
    internal class ProductCodeToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProductCode product && parameter is ProductCode target)
            {
                return product == target;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {            
            return Binding.DoNothing;
        }
    }
}
