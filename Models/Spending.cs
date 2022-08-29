using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SpendingsCalculator.Models
{

    public class Spending
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Products")]
        public Product[] Products { get; set; }

        [Display(Name = "Condition")]
        public Condition Spending_Condition { get; set; }

        public static bool TryParse(string value, out Spending result)
        {
            result = null;
            string[] raw_data = value.Split(',');
            for (int i = 0; i < raw_data.Length; i++)
            {
                raw_data[i] = raw_data[i].Trim();
            }

            if (int.TryParse(raw_data[0], out int id) == false)
                return false;

            string title = raw_data[1];
            string description = raw_data[2];
            if (int.TryParse(raw_data[3], out int coundiction_id) == false)
                return false;
            Condition condition = new Condition_Placeholder() { ID = coundiction_id };

            List<Product> products = new List<Product>();
            for (int i = 4; i < raw_data.Length; i++)
            {
                if (int.TryParse(raw_data[i], out int product_id) == false)
                    return false;
                products.Add(new Product { ID = product_id });
            }
            result = new Spending { ID = id, Title = title, Description = description, Spending_Condition = condition, Products = products.ToArray() };
            return true;
        }
    }
}