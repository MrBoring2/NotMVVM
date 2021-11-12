using NotMVVMProject.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NotMVVMProject.Converters
{
    public class ProductMaterialsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string materials = string.Empty;
            int index = 0;
            var list = value as List<MaterialToProduct>;
            foreach (var item in list)
            {
                materials += item.MaterialName;

                if (index + 1 < list.Count())
                    materials += ", ";
                index++;
            }
            return materials;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
