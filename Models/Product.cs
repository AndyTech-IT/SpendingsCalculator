using System;
using System.Collections.Generic;

namespace SpendingsCalculator.Models
{
    public class Product
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public List<ProductCategory> Categories { get; set; }

        internal static bool TryParse(string value, out Product result)
        {
            result = null;

            string[] raw_data = value.Split(',');
            if (raw_data.Length < 5)
                return false;

            for (int i = 0; i < raw_data.Length; i++)
            {
                raw_data[i] = raw_data[i].Trim();
            }

            if (int.TryParse(raw_data[0], out int id) == false)
                return false;
            if (int.TryParse(raw_data[2], out int quantity) == false)
                return false;
            if (double.TryParse(raw_data[3].Replace('.', ','), out double price) == false)
                return false;

            List<ProductCategory> categories = new List<ProductCategory>();
            for (int i = 4; i < raw_data.Length; i++)
            {
                if (int.TryParse(raw_data[i], out int category_id) == false)
                    return false;
                categories.Add(new ProductCategory { ID = category_id });
            }

            result = new Product { ID = id, Title = raw_data[1], Quantity = quantity, Price = price, Categories = categories };
            return true;
        }
    }
}