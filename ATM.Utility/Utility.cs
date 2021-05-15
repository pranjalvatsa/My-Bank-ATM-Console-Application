using System;
using System.Globalization;
using System.Text;
using System.Threading;

namespace ATM.Utility
{
    public static class Utility
    {

        private static CultureInfo culture = new CultureInfo("en-IN");
        private static long tranId;


        public static long GetTransactionId()
        {
            return ++tranId;
        }

        /// <summary>
        /// Show the press enter to continue message
        /// </summary>
        public static void PressEnterMessage()
        {
            Console.WriteLine("\n Press enter to continue.");
            Console.ReadKey();
        }

        /// <summary>
        /// Get input from user
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string GetRawInput (string message)
        {
            Console.Write($"Enter {message}: ");
            return Console.ReadLine();
        }
        /// <summary>
        /// Prints the message in the correct color code
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="success"></param>
        public static void PrintMessage(string msg, bool success)
        {
            if (success)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else
                Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(msg);
            Console.ResetColor();
            PressEnterMessage();

        }
        /// <summary>
        /// Get user input without showing the characters on keyboard.
        /// Remove char if backspace is pressed, append to user input otherwise.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetHiddenConsoleInput(string text)
        {
            bool prompt = true;

            StringBuilder input = new StringBuilder();
            while (true)
            {
                if (prompt)
                    Console.Write(text);
                var key = Console.ReadKey(true);
                prompt = false;

                if (key.Key == ConsoleKey.Enter)
                {
                    if (input.Length == 6)
                    {
                        break;
                    }
                    else
                    {
                        PrintMessage("\nPlease enter 6 digits.", false);
                        prompt = true;
                        input.Clear();
                    }
                }
                if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                    input.Remove(input.Length - 1, 1);
                else if (key.Key != ConsoleKey.Backspace)
                    input.Append(key.KeyChar);
            }
            return input.ToString();
        }
        /// <summary>
        /// Print dots and sleep the program for a while to give a feel of some animation
        /// </summary>
        /// <param name="timer"></param>
        public static void PrintDotAnimation(int timer = 10)
        {
            for (var x = 0; x < timer; x++)
            {
                Console.Write(".");
                Thread.Sleep(100);
            }
            Console.WriteLine();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ConsoleWriteLine"></param>
        public static void PrintConsoleWriteLine(string msg, bool ConsoleWriteLine = true)
        {
            if (ConsoleWriteLine)
                Console.WriteLine(msg);
            else
                Console.Write(msg);

            PressEnterMessage();
        }

        public static string FormatAmount(decimal amt)
        {
            return String.Format(culture, "{0:C2}", amt);
        }

        public static void PrintUserInputLabel(string msg, bool ConsoleWriteLine = false)
        {
            if (ConsoleWriteLine)
                Console.WriteLine(msg);
            else
                Console.Write(msg);

        }
    }
}
