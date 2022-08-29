using System.ComponentModel.DataAnnotations;

namespace SpendingsCalculator.Models
{
    public class Calculator_ViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Select a correct license")]
        public ConditionType ConditionType { get; set; }
    }
}
