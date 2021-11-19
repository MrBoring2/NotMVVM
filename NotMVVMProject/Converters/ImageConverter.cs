using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NotMVVMProject.Converters
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            if (string.IsNullOrEmpty(value.ToString()))
            {
                if (File.Exists("Images/not_found.png"))
                    return File.ReadAllBytes("Images/not_found.png");
                else return null;
            }
            else
            {
                
                if (File.Exists($"{value}"))
                {
                    return File.ReadAllBytes($"{value}");
                }
                else return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
