using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpendingsCalculator.Models
{
    public class ProductCategory
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public static bool TryParse(string value, out ProductCategory result)
        {
            result = null;

            string[] raw_data = value.Split(',');
            if (raw_data.Length != 3)
                return false;

            for (int i = 0; i < 3; i++)
            {
                raw_data[i] = raw_data[i].Trim();
            }
            if (int.TryParse(raw_data[0], out int id) == false)
                return false;
            result = new ProductCategory { ID = id, Title = raw_data[1], Description = raw_data[2] };
            return true;
        }
    }
}