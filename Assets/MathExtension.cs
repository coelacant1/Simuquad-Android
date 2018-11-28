using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    static class MathExtension
    {/// <summary>
     /// Constrains the output of the input value to a maximum and minimum value.
     /// </summary>
     /// <param name="value"></param>
     /// <param name="minimum"></param>
     /// <param name="maximum"></param>
     /// <returns></returns>
        public static double Constrain(double value, double minimum, double maximum)
        {
            if (value > maximum)
            {
                value = maximum;
            }
            else if (value < minimum)
            {
                value = minimum;
            }

            return value;
        }

        public static double Exponential(double x, double maximum, double expo)
        {
            int tempSign = Math.Sign(x);
            double tempScaled = Math.Abs(x) / maximum;

            return Math.Pow(tempScaled, expo) * maximum * tempSign;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static double DegreesToRadians(double degrees)
        {
            return degrees / (180.0 / Math.PI);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static double RadiansToDegrees(double radians)
        {
            return radians * (180.0 / Math.PI);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static BetterVector DegreesToRadians(BetterVector degrees)
        {
            return degrees / (180.0 / Math.PI);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static BetterVector Vector(BetterVector radians)
        {
            return radians * (180.0 / Math.PI);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DoubleToCleanString(double value)
        {
            return String.Format("{0:0.00}", value).PadLeft(8);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public static void CleanPrint(params double[] values)
        {
            string fullString = "";

            for (int i = 0; i < values.Length; i++)
            {
                fullString += DoubleToCleanString(values[i]) + " ";
            }

            Console.WriteLine(fullString);
        }
    }
}
