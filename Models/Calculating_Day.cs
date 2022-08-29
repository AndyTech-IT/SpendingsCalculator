using System;
using System.Collections.Generic;

namespace SpendingsCalculator.Models
{
    public class Calculating_Day
    {
        public DateTime Date { get; set; }
        public List<Spending> Spendings { get; set; }
        public double Total_Amount { get; set; }
        public Dictionary<string, double> Categorys_Amount { get; set; } 
    }
}
