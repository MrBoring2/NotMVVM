using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotMVVMProject.Models
{
    public class FilterItem
    {
        public string Property { get; set; }
        public string Title { get; set; }

        public FilterItem(string property, string title)
        {
            Property = property;
            Title = title;
        }
    }
}
