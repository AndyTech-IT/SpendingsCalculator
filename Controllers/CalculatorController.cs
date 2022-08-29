using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using SpendingsCalculator.Models;

namespace SpendingsCalculator.Controllers
{
    public class CalculatorController : Controller
    {
        // GET: Calculator
        public ActionResult Index()
        {
            return View(new Calculator_ViewModel());
        }

        public ActionResult Calculate()
        {
            string begin = Request.Form.Get("begin");
            string end = Request.Form.Get("end");
            string[] categorys = Request.Form.Get("categorys").Replace('\r', ' ').Split('\n');
            string[] products = Request.Form.Get("products").Replace('\r', ' ').Split('\n');
            string[] conditions = Request.Form.Get("conditions").Replace('\r', ' ').Split('\n');
            string[] spendings = Request.Form.Get("spendings").Replace('\r', ' ').Split('\n');

            if (DateTime.TryParse(begin, out DateTime start) == false)
                throw new Exception("Wrong begin date format!");
            if (DateTime.TryParse(end, out DateTime finish) == false)
                throw new Exception("Wrong end date format!");

            ProductCategory[] categories_arr = new ProductCategory[categorys.Length];
            for (int i = 0; i < categorys.Length; i++)
            {
                if (ProductCategory.TryParse(categorys[i], out ProductCategory category) == false)
                    throw new Exception($"Wrong category[{i}] format!");
                categories_arr[i] = category;
            }

            Product[] products_arr = new Product[products.Length];
            for (int i = 0; i < products.Length; i++)
            {
                if (Product.TryParse(products[i], out Product product) == false)
                    throw new Exception($"Wrong product[{i}] format!");
                for (int j = 0; j < product.Categories.Count; j++)
                {
                    product.Categories[j] = categories_arr.First(c => c.ID == product.Categories[j].ID);
                }
                products_arr[i] = product;
            }

            Condition[] conditions_arr = new Condition[conditions.Length];
            for (int i = 0; i < conditions.Length; i++)
            {
                if (Condition.TryParse(conditions[i], out Condition condition) == false)
                    throw new Exception($"Wrong condition[{i}] format!");
                conditions_arr[i] = condition;
            }

            Spending[] spendings_arr = new Spending[spendings.Length];
            for(int i = 0; i < spendings.Length; i++)
            {
                if (Spending.TryParse(spendings[i], out Spending spending) == false)
                    throw new Exception($"Wrong spending[{i}] format!");

                for (int j = 0; j < spending.Products.Length; j++)
                    spending.Products[j] = products_arr.First(p => p.ID == spending.Products[j].ID);

                spending.Spending_Condition = conditions_arr.First(p => p.ID == spending.Spending_Condition.ID);
                spendings_arr[i] = spending;
            }

            CalculatingResult result = Calculator.Calculate(start, finish, spendings_arr);
            return View("Calculate", result);
        }
    }
}