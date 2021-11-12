using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotMVVMProject.Servises
{
    /// <summary>
    /// с ним будет долго поэтому пофиг на него
    /// </summary>
    public class DataBaseServise
    {
        private static DataBaseServise instanse;
        public static DataBaseServise Instance
        {
            get
            {
                if (instanse is null)
                {
                    instanse = new DataBaseServise();
                }
                return instanse;
            }
        }

        #region EFContextMethods
        public void GetProducts()
        {

        }
        public void DeleteProduct()
        {

        }

        public void UpdateProduct()
        {

        }

        public void GetProdcut()
        {

        }
        #endregion

    }
}
