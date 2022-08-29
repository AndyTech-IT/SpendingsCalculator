using System.Collections.Generic;

namespace SpendingsCalculator.Models
{
    public class CalculatingResult
    {
        public List<Calculating_Day> Days { get; set; }
        public double Total_Amount { get; set; }
        public Dictionary<string, double> Categorys_Amount { get; set; }
    }
}