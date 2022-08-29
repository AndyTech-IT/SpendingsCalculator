using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpendingsCalculator.Models
{

    public static class Calculator
    {
        public static CalculatingResult Calculate(DateTime start_day, DateTime end_day, Spending[] spendings)
        {
            double total_days = (end_day - start_day).TotalDays;
            if (total_days <= 0)
                throw new Exception("Wrong range!");

            CalculatingResult result = new CalculatingResult() { Days = new List<Calculating_Day>(), Total_Amount = 0, Categorys_Amount = new Dictionary<string, double>() };

            for (DateTime day = start_day; (end_day - day).TotalDays >= 0; day = day.AddDays(1))
            {
                Calculating_Day day_data = new Calculating_Day() 
                {
                    Date = new DateTime( day.Year, day.Month, day.Day), 
                    Spendings = new List<Spending>(), Total_Amount = 0, 
                    Categorys_Amount = new Dictionary<string, double>() 
                };

                foreach (var spending in spendings)
                {
                    if (spending.Spending_Condition.Is_Spending_Day(day) == true)
                    {
                        day_data.Spendings.Add(spending);
                        foreach(Product product in spending.Products)
                        {
                            double amount = product.Price * product.Quantity;
                            day_data.Total_Amount += amount;
                            foreach(var category in product.Categories)
                            {
                                if (day_data.Categorys_Amount.ContainsKey(category.Title))
                                {
                                    day_data.Categorys_Amount[category.Title] += amount;
                                }
                                else
                                {
                                    day_data.Categorys_Amount.Add(category.Title, amount);
                                }
                            }
                        }
                        foreach (var category in day_data.Categorys_Amount)
                        {
                            if (result.Categorys_Amount.ContainsKey(category.Key))
                            {
                                result.Categorys_Amount[category.Key] += category.Value;
                            }
                            else
                            {
                                result.Categorys_Amount.Add(category.Key, category.Value);
                            }
                        }
                    }
                }
                result.Total_Amount += day_data.Total_Amount;
                result.Days.Add(day_data);
            }

            return result;
        }
    }
}