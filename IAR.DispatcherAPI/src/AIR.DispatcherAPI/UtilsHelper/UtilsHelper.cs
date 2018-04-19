using System;
using System.Text;

namespace IAR.DispatcherAPI.UtilsHelper
{
    public static class UtilsHelper
    {
        #region >> Parse To Functions

        public static int ParseInt(object originalValue, int defaultValue)
        {
            int returnValue = defaultValue;
            if (!IsEmpty(originalValue))
            {
                try
                {
                    returnValue = Convert.ToInt32(originalValue);
                }
                catch
                {
                    returnValue = defaultValue;
                }
            }

            return returnValue;
        }

        public static bool ParseBool(object originalValue, bool defaultValue)
        {
            bool returnValue = defaultValue;

            if (!IsEmpty(originalValue))
            {
                string stringValue = originalValue.ToString().Trim().ToLower();

                switch (stringValue)
                {
                    case "1":
                    case "yes":
                    case "true":
                        returnValue = true;
                        break;
                    default:
                        returnValue = false;
                        break;
                }
            }
            return returnValue;
        }

        public static long ParseLong(object stringValue, long defaultValue)
        {
            long returnValue = defaultValue;

            if (!IsEmpty(stringValue))
            {
                try
                {
                    returnValue = long.Parse(stringValue.ToString());
                }
                catch
                {
                }
            }
            return returnValue;
        }

        public static double ParseDouble(object stringValue, double defaultValue)
        {
            double returnValue = defaultValue;
            if (!IsEmpty(stringValue))
            {
                try
                {
                    returnValue = double.Parse(stringValue.ToString());
                }
                catch
                {
                }
            }
            return returnValue;
        }

        public static DateTime ParseDateTime(object stringValue, DateTime defaultValue)
        {
            DateTime returnValue = defaultValue;

            if (!IsEmpty(stringValue))
            {
                try
                {
                    returnValue = Convert.ToDateTime(stringValue.ToString());
                }
                catch
                {
                }
            }
            return returnValue;
        }

        public static string ParseString(object value)
        {
            return !IsNull(value) ? value.ToString() : string.Empty;
        }

        #endregion

        #region >> Custom Functions

        public static bool isInt(string stringValue)
        {

            bool returnValue = false;

            try
            {
                if (stringValue != null && stringValue != string.Empty)
                {
                    int.Parse(stringValue);
                    returnValue = true;
                }
            }
            catch (Exception)
            {
                // ignore exception
            }

            return returnValue;
        }


        public static bool IsNull(object value)
        {
            return value == null || value == DBNull.Value ? true : false;
        }

        public static bool IsEmpty(object value)
        {
            return IsNull(value) || value.ToString().Trim().Length < 1 ? true : false;
        }

        /// <summary>
        /// Generates a random string with the given length
        /// </summary>
        /// <param name="size">Size of the string</param>
        /// <param name="lowerCase">If true, generate lowercase string</param>
        /// <returns>Random string</returns>
        public static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        #endregion

        public static string CleanRegEx(string Search)
        {
            StringBuilder sb = new StringBuilder(Search);
            // Reserved Characters
            sb.Replace(".", "\\.");
            sb.Replace("^", "\\^");
            sb.Replace("|", "\\|");
            sb.Replace("*", "\\*");
            sb.Replace("+", "\\+");
            sb.Replace("?", "\\?");
            sb.Replace("(", "\\(");
            sb.Replace(")", "\\)");
            sb.Replace("[", "\\[");
            return sb.ToString();
        }
    }
}
