﻿
using System;
using System.Linq;

namespace HFM.Core.Client
{
    public static class NumberFormat
    {
        /// <summary>
        /// Gets the number format string using the given number of decimal places.
        /// </summary>
        public static string Get(int decimalPlaces)
        {
            return BuildFormat(decimalPlaces, "#,0");
        }

        /// <summary>
        /// Gets the number format string using the given number of decimal places.
        /// </summary>
        public static string Get(int decimalPlaces, string format)
        {
            return BuildFormat(decimalPlaces, format);
        }

        private static string BuildFormat(int decimalPlaces, string format)
        {
            return decimalPlaces <= 0 
                ? format 
                : String.Concat(format, ".", new String(Enumerable.Repeat('0', decimalPlaces).ToArray()));
        }
    }
}
