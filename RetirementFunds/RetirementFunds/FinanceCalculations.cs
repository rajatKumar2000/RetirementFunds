﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RetirementFunds
{
    //A static class of the financial calculations present within the Retirement app.
    //Note: interest here does not only refer to interest, but any type of rate growth.
    static class FinanceCalculations
    {
        //Finds the future value of a sum of money principal after a set amount of time with a growth rate of interest
        //with the option to set the number of compounding periods in every unit of time.
        public static decimal FutureValue(decimal principal, double time, double interest, int nCompPeriods = 1)
        {
            return principal * (decimal)Math.Pow(1 + interest / nCompPeriods, time * nCompPeriods);
        }

        //The inverse of the FutureValue() method; i.e. given a future sum of money, what is the present worth.
        public static decimal PresentValue(decimal principal, double time, double interest, int nCompPeriods = 1)
        {
            return principal * (decimal)Math.Pow(1 + interest / nCompPeriods, -1 * time * nCompPeriods);
        }

        //Converts a rate with a specific frequency of compounding period to another frequency
        public static double EquivelantRate(double rate, int firstCompound, int secondCompound)
        {
            return secondCompound * (double)(Math.Pow(1 + rate / firstCompound, (double)firstCompound / secondCompound) - 1);
        }

        //For fixed annuities (i.e. static payments), this finds the future value. startImmediately is if the annuity starts  
        //at t=0 or one compound period after. If payment frequency does not match the number of compounding periods per time unit,
        //interest must be converted to an effective rate in the time frame of the payment frequency, then payment frequency takes the place
        //of nCompPeriods in the original formula.
        public static decimal FutureFixedAnnuityValue(decimal annuity, double time, double interest, int nCompPeriods = 1, int startImmediately = 0, int paymentFrequency = 1)
        {
            if (paymentFrequency == nCompPeriods)
            {
                return annuity / (decimal)(interest / nCompPeriods) * (decimal)(Math.Pow(1 + interest / nCompPeriods, time * nCompPeriods) - 1) * 
                    (decimal)(1 + interest / nCompPeriods * startImmediately);
            }

            double a = EquivelantRate(0.01f, 1, 12);
            return annuity / (decimal)(a / paymentFrequency) * (decimal)(Math.Pow(1 + a / paymentFrequency, time * paymentFrequency) - 1) * 
                (decimal)(1 + a / paymentFrequency * startImmediately);
        }

        //Finds the future value for an annuity where the annuity grows over time. Two different formulas if interest=growth or not.
        public static decimal FutureVariableAnnuityValue(decimal annuity, double time, double interest, double growth, int nCompPeriods = 1, int startImmediately = 0, int paymentFrequency = 1)
        {
            if (growth != interest)
            {
                if (paymentFrequency == nCompPeriods)
                {
                    return annuity / ((decimal)((interest - growth) / nCompPeriods)) * (decimal)(Math.Pow(1 + interest / nCompPeriods, time * nCompPeriods) - 
                        Math.Pow(1 + growth / nCompPeriods, time * nCompPeriods)) * (decimal)(1 + interest / nCompPeriods * startImmediately);
                }

                interest = EquivelantRate(interest, nCompPeriods, paymentFrequency);
                return annuity / ((decimal)((interest - growth) / paymentFrequency)) * (decimal)(Math.Pow(1 + interest / paymentFrequency, time * paymentFrequency) - 
                    Math.Pow(1 + growth / paymentFrequency, time * paymentFrequency)) * (decimal)(1 + interest / paymentFrequency * startImmediately);
            }

            if (paymentFrequency == nCompPeriods)
            {
                return annuity * (decimal)(nCompPeriods * time) * (decimal)Math.Pow(1 + interest / nCompPeriods, time * nCompPeriods - 1) * 
                    (decimal)(1 + interest / nCompPeriods * startImmediately);
            }

            interest = EquivelantRate(interest, nCompPeriods, paymentFrequency);
            return annuity * (decimal)(paymentFrequency * time) * (decimal)Math.Pow(1 + interest / paymentFrequency, time * paymentFrequency - 1) * 
                (decimal)(1 + interest / paymentFrequency * startImmediately);
        }

        //Returns the percentile from an array of decimals
        public static double Percentile(double[] sequence, double excelPercentile)
        {
            Array.Sort(sequence);
            int N = sequence.Length;
            double n = (N - 1) * excelPercentile + 1;
            // Another method: double n = (N + 1) * excelPercentile;
            if (n == 1) return sequence[0];
            else if (n == N) return sequence[N - 1];
            else
            {
                int k = (int)n;
                double d = n - k;
                return sequence[k - 1] + d * (sequence[k] - sequence[k - 1]);
            }
        }        
    }
}
