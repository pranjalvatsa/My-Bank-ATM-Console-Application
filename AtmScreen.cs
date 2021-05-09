using System;
using ATM.Repository.Entities;
using ATM.Utility;

namespace MyBankATMApp
{
    /// <summary>
    /// Class responsible to print the user interface
    /// </summary>
    internal class AtmScreen
    {
        public AtmScreen()
        {
        }

        /// <summary>
        /// Method to print the welcome screen
        /// </summary>
        internal static void WelcomeATM()
        {
            Console.Clear();
            Console.Title = "My Bank ATM System";
            Console.WriteLine("Welcome to My Bank ATM. \n");
            Console.WriteLine("Please insert your ATM card.");
            //Show press enter key message
            Utility.PressEnterMessage();
        }
        /// <summary>
        /// Prints user name on console
        /// </summary>
        /// <param name="fullName"></param>
        internal static void WelcomeCustomer(string fullName)
        {
            Utility.PrintConsoleWriteLine("Welcome back, " + fullName);
        }

        /// <summary>
        /// Print the secure menu
        /// </summary>
        internal static void DisplaySecureMenu()
        {
            Console.Clear();
            Console.WriteLine("------------------------------");
            Console.WriteLine("| My Bank ATM Secure Menu    |");
            Console.WriteLine("|                            |");
            Console.WriteLine("| 1. Balance Enquiry         |");
            Console.WriteLine("| 2. Cash Deposit            |");
            Console.WriteLine("| 3. Withdrawal              |");
            Console.WriteLine("| 4. Third Party Transfer    |");
            Console.WriteLine("| 5. Transactions            |");
            Console.WriteLine("| 6. Logout                  |");
            Console.WriteLine("|                            |");
            Console.WriteLine("------------------------------");
        }

        /// <summary>
        /// Show login form
        /// </summary>
        ///<returns>Bank Account User</returns>
        internal BankAccountUser LoginForm()
        {
            var _bankAccountUser = new BankAccountUser();

            _bankAccountUser.CardNumber = Validator.Convert<long>("card number");
            //get pin code without showing the characters on the console
            _bankAccountUser.CardPin = Convert.ToInt32(Utility.GetHiddenConsoleInput("Enter card pin:"));
            return _bankAccountUser;
        }
        /// <summary>
        /// Show login progress
        /// </summary>
        internal static void LoginProgress()
        {
            Console.Write("\nChecking card number and card pin.");
            Utility.PrintDotAnimation();
            Console.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        internal static void PrintLockAccount()
        {
            Console.Clear();
            Utility.PrintMessage("Your account is locked. Please go to " +
                "the nearest branch to unlocked your account. Thank you.", true);

            Utility.PressEnterMessage();
            Environment.Exit(1);
        }

        internal static void LogoutProgress()
        {
            throw new NotImplementedException();
        }
    }
}
