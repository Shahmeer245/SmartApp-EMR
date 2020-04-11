using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class AgeHelper
    {
        DateTime startDate;
        int years;
        int months;
        int days;


        public AgeHelper(DateTime _date)
        {
            startDate = _date;
        }

        public bool isAgeMatched(DateTime birthDate, Enum.DayWeekMthYr fromUnit, int fromValue, Enum.DayWeekMthYr toUnit, int toValue)
        {           
            long ageInDays, ageInMonths;
            float ageInYears, ageInWeeks;
            bool fromValueMatched = false, toValueMatched= false;

            ageInDays = this.getAgeInDays(birthDate);
            ageInMonths = this.getAgeInMonths(birthDate);
            ageInWeeks = this.getAgeInWeeks(birthDate);
            ageInYears = this.getAgeInYears(birthDate);


            switch (fromUnit)
            {
                case Enum.DayWeekMthYr.Days:
                    if (ageInDays >= fromValue)
                        fromValueMatched = true;
                    break;
                case Enum.DayWeekMthYr.Weeks:
                    if (ageInWeeks >= fromValue)
                        fromValueMatched = true;
                    break;
                case Enum.DayWeekMthYr.Months:
                    if (ageInMonths >= fromValue)
                        fromValueMatched = true;
                    break;
                case Enum.DayWeekMthYr.Years:
                    if (ageInYears >= fromValue)
                        fromValueMatched = true;
                    break;
            }

            switch (toUnit)
            {
                case Enum.DayWeekMthYr.Days:
                    if (ageInDays <= toValue)
                        toValueMatched = true;
                    break;
                case Enum.DayWeekMthYr.Weeks:
                    if (ageInWeeks <= toValue)
                        toValueMatched = true;
                    break;
                case Enum.DayWeekMthYr.Months:
                    if (ageInMonths <= toValue)
                        toValueMatched = true;
                    break;
                case Enum.DayWeekMthYr.Years:
                    if (ageInYears <= toValue)
                        toValueMatched = true;
                    break;
            }

            if (fromValueMatched && toValueMatched)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        int yearDiff(DateTime d1, DateTime d2)
        {
            /*DateTime boundary;
            int offset;
            d2 = d2.AddDays(-1);
            if (d1 < d2)
            {
                boundary = new DateTime(d2.Year, d1.Month, d1.Day);
                offset = (d2 - boundary).TotalDays < 0 ? -1 : 0;
            }
            else
            {                
                boundary = new DateTime(d1.Year, d2.Month, d2.Day);
                offset = (d1 - boundary).TotalDays < 0 ? 1 : 0;
            }

            return d1.Year - d2.Year - offset;*/

            int years = d1.Year - d2.Year;

            if (d2.Month == d1.Month &&// if the start month and the end month are the same
                d1.Day < d2.Day)// BUT the end day is less than the start day
            {
                years--;
            }
            else if (d1.Month < d2.Month)// if the end month is less than the start month
            {
                years--;
            }

            return years;
        }

        public long getAgeInDays(DateTime _date)
        {
            return Convert.ToInt64((startDate - _date).TotalDays);
        }

        public long getAgeInMonths(DateTime _date)
        {
            long ageInMonths;
            int yr = yearDiff(startDate, _date);
            this.getYearsMonthsDaysFromDate(_date);

            ageInMonths = (yr * 12) + months;

            return ageInMonths;
        }

        public float getAgeInWeeks(DateTime _date)
        {
            return this.getAgeInDays(_date) / 7;
        }

        public float getAgeInYears(DateTime _date)
        {
            float ageInYears;
            int yrs = yearDiff(startDate, _date);
            this.getYearsMonthsDaysFromDate(_date);

            ageInYears = yrs + (months / 12);

            return ageInYears;
        }

        public void getYearsMonthsDaysFromDate(DateTime _date)
        {       
            int currentMonth;
            int currentDay;
            int currentYear;

            int noOfdaysInPreviousMonthFromCurrent;
            int previousMonthFromCurrent;

            int birthYear;
            int birthMonth;
            int birthDay;

            int counterOfMonths, counterOfDays;

            bool isMonthCrossed;
            bool willMonthsCrossed;

            years = this.yearDiff(startDate, _date);
            months = 0;
            days = 0;

            currentYear = startDate.Year;
            currentMonth = startDate.Month;
            currentDay = startDate.Day;

            birthYear = _date.Year;
            birthMonth = _date.Month;
            birthDay = _date.Day;

            previousMonthFromCurrent = 0;
            noOfdaysInPreviousMonthFromCurrent = 0;

            counterOfMonths = 0;
            counterOfDays = 0;

            isMonthCrossed = false;
            willMonthsCrossed = false;

            //calculating previous month from current date
            if (currentMonth == 1)
            {
                previousMonthFromCurrent = 12;
            }
            else
            {
                previousMonthFromCurrent = currentMonth - 1;
            }

            //calculating no fo days in previous month from current days
            switch (previousMonthFromCurrent)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    noOfdaysInPreviousMonthFromCurrent = 31;
                    break;
                case 4:
                case 6:
                case 9:
                case 11:
                    noOfdaysInPreviousMonthFromCurrent = 30;
                    break;
                case 2:
                    if ((currentYear % 4) == 0)
            {
                        noOfdaysInPreviousMonthFromCurrent = 29;
                    }
            else
            {
                        noOfdaysInPreviousMonthFromCurrent = 28;
                    }
                    break;
            }


            if (birthMonth == currentMonth && currentYear == birthYear) // if age is less than a month i.e. birthdate's month is equal to current date's month
            {
                months = 0;
            }
            else if (birthMonth == currentMonth && years > 0 && currentDay >= birthDay) // if age is n years and/or n/0 day
            {
                months = 0;
            }
            else //calculating n months
            {
                months++;

                counterOfMonths = birthMonth;

                if (counterOfMonths == 12)
                {
                    counterOfMonths = 1;
                }
                else
                {
                    counterOfMonths++;
                }
                while (true)
                {
                    if (counterOfMonths == currentMonth)
                    {
                        break;
                    }
                    else
                    {
                        months++;

                        if (counterOfMonths == 12)
                        {
                            counterOfMonths = 1;
                        }
                        else
                        {
                            counterOfMonths++;
                        }
                    }
                }
            }

            if (birthDay > currentDay)
            {
                willMonthsCrossed = true;
            }
            else
            {
                willMonthsCrossed = false;
            }

            while (true)
            {
                if (birthDay == currentDay)
                {
                    if (isMonthCrossed == true)
                    {
                        months--;
                    }
                    break;
                }
                else
                {
                    days++;

                    if (birthDay >= noOfdaysInPreviousMonthFromCurrent && willMonthsCrossed && !isMonthCrossed)
                    {
                        birthDay = 1;
                        isMonthCrossed = true;
                    }
                    else
                    {
                        birthDay++;
                    }
                }
            }
        }
    }
}
