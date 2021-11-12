using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotMVVMProject.Data
{
    public class BaseEntity
    {
        public object GetProperty(string property)
        {
            if (!string.IsNullOrEmpty(property))
            {
                return this.GetType().GetProperty(property).GetValue(this);
            }
            return null;
        }
    }
}
