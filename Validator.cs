using System;
using System.ComponentModel;

namespace ATM.Utility
{
    /// <summary>
    /// This class takes care of user input validation
    /// </summary>
    public static class Validator
    {
        public static T Convert<T>(this string input)
        {
            bool valid = false;
            string rawInput;

            while (!valid)
            {
                rawInput = Utility.GetRawInput(input);

                try
                {
                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    if (converter != null)
                        // convert raw input string to object
                        return (T)converter.ConvertFromString(rawInput);
                    return default;
                }

                catch
                {
                    Utility.PrintMessage("Invalid input, try again.", false);
                }
            }
            return default;
        }
    }
}
