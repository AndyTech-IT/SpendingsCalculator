using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SpendingsCalculator.Models
{
    public abstract class Condition
    {
        public abstract bool Is_Spending_Day(DateTime day);
        public int ID { get; set; }

        public static bool TryParse(string value, out Condition result)
        {
            result = null;
            string[] raw_data = value.Split(',');
            for (int i = 0; i < raw_data.Length; i++)
            {
                raw_data[i] = raw_data[i].Trim();
            }
            raw_data[0] = raw_data[0].ToLower();
            switch (raw_data[0])
            {
                case "yearly":
                    return Year_Cycle.TryParse(raw_data, out result);
                case "monthly":
                    return Month_Cycle.TryParse(raw_data, out result);
                case "weekly":
                    return Week_Cycle.TryParse(raw_data, out result);
                case "cycly":
                    return Iteration_Cycle.TryParse(raw_data, out result);
                case "complex":
                    return Complex_Condiction.TryParse(raw_data, out result);
                default:
                    return false;
            }
        }
    }

    public class Condition_Placeholder : Condition
    {
        public override bool Is_Spending_Day(DateTime day)
        {
            throw new NotImplementedException();
        }
    }

    public enum ConditionType
    {
        [Display(Name = "Year Cycle")]
        Year_Cycle,
        [Display(Name = "Month Cycle")]
        Month_Cycle,
        [Display(Name = "Week Cycle")]
        Week_Cycle,
        [Display(Name = "Iteration Cycle")]
        Iteration_Cycle,
        [Display(Name = "Complex Condiction")]
        Complex_Condiction
    }

    public class Year_Cycle : Condition
    {
        private readonly List<DateTime> Days;

        public static bool TryParse(string[] raw_data, out Condition result)
        {
            result = null;
            if (raw_data[0] != "yearly")
                return false;
            if (int.TryParse(raw_data[1], out int id) == false)
                return false;
            List<DateTime> days = new List<DateTime>();
            for (int i = 2; i < raw_data.Length; i++)
            {
                string[] date = raw_data[i].Split('.');
                if (int.TryParse(date[0], out int day) == false)
                    return false;
                if (int.TryParse(date[1], out int month) == false)
                    return false;
                days.Add(new DateTime(DateTime.Now.Year, month, day));
            }

            result = new Year_Cycle(days.ToArray()) { ID = id };
            return true;
        }

        public Year_Cycle(DateTime[] days)
        {
            Days = new List<DateTime>();
            int year = 0;
            bool first = true;
            foreach (var day in days)
            {
                if (first)
                {
                    first = false;
                    year = day.Year;
                }

                if (year != day.Year)
                {
                    throw new Exception($"You should put only dates with one year! ({year} != {day.Year})");
                }

                if (Is_Spending_Day(day))
                {
                    throw new Exception("Days should not be repeated twice!");
                }
                
                Days.Add(new DateTime(year, day.Month, day.Day));

            }
        }
        public override bool Is_Spending_Day(DateTime day) => Days.FindAll(d => d.Month == day.Month && d.Day == day.Day).Count != 0;
    }

    public class Month_Cycle : Condition
    {
        private readonly List<DateTime> Days;

        public static bool TryParse(string[] raw_data, out Condition result)
        {
            result = null;
            if (raw_data[0] != "monthly")
                return false;
            if (int.TryParse(raw_data[1], out int id) == false)
                return false;
            List<DateTime> days = new List<DateTime>();
            for (int i = 2; i < raw_data.Length; i++)
            {
                if (int.TryParse(raw_data[i], out int day) == false)
                    return false;
                days.Add(new DateTime(DateTime.Now.Year, DateTime.Now.Month, day));
            }

            result = new Month_Cycle(days.ToArray()) { ID = id };
            return true;
        }

        public Month_Cycle(DateTime[] days)
        {
            Days = new List<DateTime>();
            int year = 0;
            int month = 0;
            bool first = true;
            foreach (var day in days)
            {
                if (first)
                {
                    first = false;
                    year = day.Year;
                    month = day.Month;
                }

                if (year != day.Year)
                {
                    throw new Exception($"You should put only dates with one year! ({year} != {day.Year})");
                }
                if (month != day.Month)
                {
                    throw new Exception($"You should put only dates with one month! ({month} != {day.Month})");
                }

                if (Is_Spending_Day(day))
                {
                    throw new Exception("Days should not be repeated twice!");
                }

                Days.Add(new DateTime(year, month, day.Day));
            }
        }

        public override bool Is_Spending_Day(DateTime day) => Days.FindAll(d => d.Day == day.Day).Count != 0;
    }

    public class Week_Cycle : Condition
    {
        private readonly List<DayOfWeek> Days;

        public static bool TryParse(string[] raw_data, out Condition result)
        {
            result = null;
            if (raw_data[0] != "weekly")
                return false;
            if (int.TryParse(raw_data[1], out int id) == false)
                return false;
            List<DateTime> days = new List<DateTime>();
            for (int i = 2; i < raw_data.Length; i++)
            {
                if (int.TryParse(raw_data[i], out int day) == false)
                    return false;
                days.Add(new DateTime(DateTime.Now.Year, DateTime.Now.Month, day));
            }
            result = new Week_Cycle(days.ToArray()) { ID = id };
            return true;
        }

        public Week_Cycle(DateTime[] days)
        {
            Days = new List<DayOfWeek>();
            foreach (var day in days)
            {
                if (Is_Spending_Day(day))
                {
                    throw new Exception("Days should not be repeated twice!");
                }

                Days.Add(day.DayOfWeek);
            }
        }

        public override bool Is_Spending_Day(DateTime day) => Days.Contains(day.DayOfWeek);
    }

    public class Iteration_Cycle : Condition
    {
        private readonly DateTime Start;
        private readonly int Spending_Days;
        private int Wait_Days;

        public static bool TryParse(string[] raw_data, out Condition result)
        {
            result = null;
            if (raw_data[0] != "cycly")
                return false;
            if (int.TryParse(raw_data[1], out int id) == false)
                return false;

            if (DateTime.TryParse(raw_data[2], out DateTime start) == false)
                return false;
            if (int.TryParse(raw_data[3], out int spandings) == false)
                return false;
            if (int.TryParse(raw_data[4], out int waitings) == false)
                return false;

            result = new Iteration_Cycle(start, spandings, waitings) { ID = id };
            return true;
        }

        public Iteration_Cycle(DateTime start_day, int spanding = 1, int wait = 1)
        {
            Start = start_day;
            Spending_Days = spanding;
            Wait_Days = wait;
        }
        public override bool Is_Spending_Day(DateTime day)  => ((Start - day).TotalDays % (Spending_Days + Wait_Days)) < Spending_Days;

    }

    public enum Condition_Operator
    {
        And, 
        Or
    }

    public class Complex_Condiction : Condition
    {
        private readonly Condition Left;
        private readonly Condition Right;

        private readonly Condition_Operator Operator;
        private readonly bool Negative_Left;
        private readonly bool Negative_Right;


        public static bool TryParse(string[] raw_data, out Condition result)
        {
            result = null;
            if (raw_data[0] != "complex")
                return false;
            if (int.TryParse(raw_data[1], out int id) == false)
                return false;
            string left_value = "";
            string right_value = "";
            int index = 2;
            for (; index < raw_data.Length; index++)
            {
                if (raw_data[index].ToLower() == "and" || raw_data[index].ToLower() == "or")
                    break;
                left_value += raw_data[index] + ", ";
            }
            Condition_Operator @operator = raw_data[index].ToLower() == "and" ? Condition_Operator.And : Condition_Operator.Or;
            index++;
            for (; index < raw_data.Length; index++)
            {
                right_value += raw_data[index] + ", ";
            }
            left_value = left_value.Trim(new char[] {',', ' '});
            right_value = right_value.Trim(new char[] { ',', ' ' });
            bool negative_l = false;
            bool negative_r = false;
            if (left_value[0] == '!')
            {
                negative_l = true;
                left_value = left_value.Remove(0, 1);
            }
            if (right_value[0] == '!')
            {
                negative_r = true;
                right_value = right_value.Remove(0, 1);
            }

            if (Condition.TryParse(left_value, out Condition left) == false)
                return false;

            if (Condition.TryParse(right_value, out Condition right) == false)
                return false;


            result = new Complex_Condiction(left, @operator, right, negative_l, negative_r) { ID = id};
            return true;
        }

        public Complex_Condiction(Condition left, Condition_Operator @operator , Condition right, bool negative_left = false, bool negative_right = false)
        {
            Left = left;
            Right = right;
            Operator = @operator;
            Negative_Left = negative_left;
            Negative_Right = negative_right;
        }

        public override bool Is_Spending_Day(DateTime day)
        {
            switch (Operator)
            {
                case Condition_Operator.And:
                    return (Negative_Left != Left.Is_Spending_Day(day)) && (Negative_Right != Right.Is_Spending_Day(day));
                case Condition_Operator.Or:
                    return (Negative_Left != Left.Is_Spending_Day(day)) || (Negative_Right != Right.Is_Spending_Day(day));
                default:
                    throw new Exception("Operator not seted!");
            }
        }
    }
}