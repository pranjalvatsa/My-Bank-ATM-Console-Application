using System;
using System.Collections.Generic;
using ATM.Repository.Entities;
using ATM.Utility;
using ATM.Repository.Enum;
using ATM.Repository.Interface;
using System.Linq;
using ConsoleTables;
using MyBankATMApp.ViewModels;

namespace MyBankATMApp
{
    public class ATMApp:IATMApp, ITransaction,IBankAccountUser
    {
        private List<BankAccountUser> _userList;
        private List<Transaction> _transactions;
        private BankAccountUser _selectedUser;
        private readonly AtmScreen _screen;
        bool isLoginPassed = false;
        private const decimal minimum_balance = 20;


        public ATMApp()
        {
            _screen = new AtmScreen();
        }

        /// <summary>
        /// Method to initialise the bank user object
        /// </summary>
        public void Initialise()
        {
            _userList = new List<BankAccountUser>
            {
                new BankAccountUser() { Id=1, FullName = "Ironman", AccountNumber=333111, CardNumber =123123, CardPin = 111111, AccountBalance = 2000.00m, IsLocked = false },
                new BankAccountUser() { Id=2, FullName = "Thor", AccountNumber=111222, CardNumber = 456456, CardPin = 222222, AccountBalance = 1500.30m, IsLocked = true },
                new BankAccountUser() { Id=3, FullName = "Superman", AccountNumber=888555, CardNumber = 789789, CardPin = 333333, AccountBalance = 2900.12m, IsLocked = false }
            };

            _transactions = new List<Transaction>();
        }

        /// <summary>
        /// Method to show the login UI to user. Check the
        /// user login details, allow user to see the secure menu if
        /// login details match the record
        /// </summary>
        public void Execute()
        {
            AtmScreen.WelcomeATM();

            ValidateATMCardNoPincode();

            AtmScreen.WelcomeCustomer(_selectedUser.FullName);

            while (isLoginPassed)
            {
                AtmScreen.DisplaySecureMenu();
                ProcessMenuOption();
            }

        }
        /// <summary>
        /// Check the user input card number and password in the
        /// user list
        /// </summary>
        public void ValidateATMCardNoPincode()
        {

            while (isLoginPassed == false)
            {
                var inputAccount = _screen.LoginForm();
                //show login progress animation
                AtmScreen.LoginProgress();

                //check for bank account in the list of records, set islogin to true
                foreach(BankAccountUser user in _userList)
                {
                    _selectedUser = user;
                    if (inputAccount.CardNumber.Equals(user.CardNumber))
                    {
                        _selectedUser.NumberOfLogins++;

                        if (inputAccount.CardPin.Equals(user.CardPin))
                        {
                            _selectedUser = user;
                            if (_selectedUser.IsLocked)
                            {
                                AtmScreen.PrintLockAccount();
                            }
                            else
                            {
                                _selectedUser.NumberOfLogins = 0;
                                isLoginPassed = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (isLoginPassed == false)
            {
                Utility.PrintMessage("Invalid card number or PIN", false);
                //Lock the account if user fails to login in 3 attempts
                _selectedUser.IsLocked = _selectedUser.NumberOfLogins == 3;
                if (_selectedUser.IsLocked)
                    AtmScreen.PrintLockAccount();
            }
            Console.Clear();
        }
        /// <summary>
        /// Implements the secure menu option choices
        /// </summary>
        private void ProcessMenuOption()
        {
            switch (Validator.Convert<int>("your option"))
            {
                case (int)SecureMenu.CheckBalance:
                    CheckBalance();
                    break;
                case (int)SecureMenu.PlaceDeposit:
                    PlaceDeposit();
                    break;
                case (int)SecureMenu.MakeWithdrawal:
                    MakeWithdrawal();
                    break;
                case (int)SecureMenu.ThirdPartyTransfer:
                    var vMThirdPartyTransfer = _screen.ThirdPartyTransferForm();
                    PerformThirdPartyTransfer(vMThirdPartyTransfer);
                    break;
                case (int)SecureMenu.ViewTransaction:
                    ViewTransaction();
                    break;

                case (int)SecureMenu.Logout:
                    AtmScreen.LogoutProgress();
                    Utility.PrintConsoleWriteLine("You have succesfully logout. Please collect your ATM card.");
                    Execute();
                    break;
                default:
                    Utility.PrintMessage("Invalid Option Entered.", false);
                    break;
            }
        }

        /// <summary>
        /// Insert any deposit or withdrawal transaction into transactions list
        /// </summary>
        /// <param name="_accountId"></param>
        /// <param name="_tranType"></param>
        /// <param name="_tranAmount"></param>
        /// <param name="_desc"></param>
        public void InsertTransaction(long _accountId, TransactionType _tranType, decimal _tranAmount, string _desc)
        {
            var transaction = new Transaction()
            {
                Id = Utility.GetTransactionId(),
                UserBankAccountId = _accountId,
                TransactionDate = DateTime.Now,
                TransactionType = _tranType,
                TransactionAmount = _tranAmount,
                Description = _desc
            };

            _transactions.Add(transaction);
        }
        /// <summary>
        /// View all transactions
        /// </summary>
        public void ViewTransaction()
        {
            // Filter transaction list
            var filteredTransactionList = _transactions.Where(t => t.UserBankAccountId == _selectedUser.AccountNumber).ToList();

            if (filteredTransactionList.Count <= 0)
                Utility.PrintMessage($"There is no transaction yet.", true);
            else
            {
                var table = new ConsoleTable("Id", "Transaction Date", "Type", "Descriptions", "Amount " + AtmScreen.currency);

                foreach (var tran in filteredTransactionList)
                {
                    table.AddRow(tran.Id, tran.TransactionDate, tran.TransactionType, tran.Description, tran.TransactionAmount);
                }
                table.Options.EnableCount = false;
                table.Write();
                Utility.PrintMessage($"You have {filteredTransactionList.Count} transaction(s).", true);
            }
        }


        public void CheckBalance()
        {
            AtmScreen.PrintCheckBalanceScreen();
            Utility.PrintConsoleWriteLine(Utility.FormatAmount(_selectedUser.AccountBalance), false);
        }
        /// <summary>
        /// Ask the user to enter an amount and deposit it into his account
        /// </summary>
        public void PlaceDeposit()
        {
            //To get amount input from user
            var transaction_amt = Validator.Convert<int>($"amount {AtmScreen.currency}");

            //To show checking and counting message
            Utility.PrintUserInputLabel("\nChecking and counting bank notes.");
            Utility.PrintDotAnimation();
            Console.SetCursorPosition(0, Console.CursorTop - 3);
            Console.WriteLine("");

            
            //Validation
            if (transaction_amt <= 0)
            {
                Utility.PrintMessage("Amount needs to be more than zero. Try again.", false);
                return;
            }

            if (transaction_amt % 10 != 0)
            {
                Utility.PrintMessage($"Key in the deposit amount only with multiply of 10. Try again.", false);
                return;
            }
            //To preview number of each of each type
            if (PreviewBankNotesCount(transaction_amt) == false)
            {
                Utility.PrintMessage($"You have cancelled your action.", false);
                return;
            }

            //insert transaction
            InsertTransaction(_selectedUser.AccountNumber, TransactionType.Deposit, transaction_amt, "deposit");

            //update account balance
            _selectedUser.AccountBalance = _selectedUser.AccountBalance + transaction_amt;

            //print message
            Utility.PrintMessage($"You have successfully deposited {Utility.FormatAmount(transaction_amt)}. " +
                "Please collect the bank slip. ", true);

        }

        /// <summary>
        /// Withdraw amount from the bank
        /// </summary>
        public void MakeWithdrawal()
        {
            //ask the user to enter an amount
            var transaction_amt = Validator.Convert<int>($"amount {AtmScreen.currency}");

            // Input data validation - Start
            if (transaction_amt <= 0)
            {
                Utility.PrintMessage("Amount needs to be more than zero. Try again.", false);
                return;
            }

            if (transaction_amt % 10 != 0)
            {
                Utility.PrintMessage($"Key in the deposit amount only with multiply of 10. Try again.", false);
                return;
            }
            // Input data validation - End


            // Business rules validation - Start
            if (transaction_amt > _selectedUser.AccountBalance)
            {
                Utility.PrintMessage($"Withdrawal failed. You do not have enough fund to withdraw {Utility.FormatAmount(transaction_amt)}", false);
                return;
            }

            if ((_selectedUser.AccountBalance - transaction_amt) < minimum_balance)
            {
                Utility.PrintMessage($"Withdrawal failed. Your account needs to have minimum {Utility.FormatAmount(minimum_balance)}", false);
                return;
            }
            // Business rules validation - End


            // Bind transaction_amt to Transaction object
            // Add transaction record - Start
            InsertTransaction(_selectedUser.Id, TransactionType.Withdrawal, +
                -transaction_amt, "");
            // Add transaction record - End

            // Another method to update account balance.
            _selectedUser.AccountBalance = _selectedUser.AccountBalance - transaction_amt;

            Utility.PrintMessage("Please collect your money. You have successfully withdraw " +
                $"{Utility.FormatAmount(transaction_amt)}. Please collect your bank slip.", true);

        }


        private bool PreviewBankNotesCount(decimal amount)
        {
            int hundredNotesCount = (int)amount / 100;
            int fiftyNotesCount = ((int)amount % 100) / 50;
            int tenNotesCount = ((int)amount % 50) / 10;

            Utility.PrintUserInputLabel("\nSummary                                                  ", true);
            Utility.PrintUserInputLabel("-------", true);
            Utility.PrintUserInputLabel($"{AtmScreen.currency} 100 x {hundredNotesCount} = {100 * hundredNotesCount}", true);
            Utility.PrintUserInputLabel($"{AtmScreen.currency} 50 x {fiftyNotesCount} = {50 * fiftyNotesCount}", true);
            Utility.PrintUserInputLabel($"{AtmScreen.currency} 10 x {tenNotesCount} = {10 * tenNotesCount}", true);
            Utility.PrintUserInputLabel($"Total amount: {Utility.FormatAmount(amount)}\n\n", true);

            char opt = Validator.Convert<char>("1 to confirm");
            return opt.Equals('1');
        }
        /// <summary>
        /// Takes the thord party transfer object as input and performs the transfer
        /// </summary>
        /// <param name="vMThirdPartyTransfer"></param>
        public void PerformThirdPartyTransfer(VMThirdPartyTransfer vMThirdPartyTransfer)
        {
            if (vMThirdPartyTransfer.TransferAmount <= 0)
            {
                Utility.PrintMessage("Amount needs to be more than zero. Try again.", false);
                return;
            }

            // Check giver's account balance - Start
            if (vMThirdPartyTransfer.TransferAmount > _selectedUser.AccountBalance)
            {
                Utility.PrintMessage("Withdrawal failed. You do not have enough " +
                    $"fund to withdraw {Utility.FormatAmount(vMThirdPartyTransfer.TransferAmount)}", false);
                return;
            }

            if (_selectedUser.AccountBalance - vMThirdPartyTransfer.TransferAmount < minimum_balance)
            {
                Utility.PrintMessage($"Withdrawal ailed. Your account needs to have " +
                    $"minimum {Utility.FormatAmount(minimum_balance)}", false);
                return;
            }
            // Check giver's account balance - End

            // Check if receiver's bank account number is valid.
            var selectedBankAccountReceiver = (from b in _userList
                                               where b.AccountNumber == vMThirdPartyTransfer.RecipientBankAccountNumber
                                               select b).FirstOrDefault();

            if (selectedBankAccountReceiver == null)
            {
                Utility.PrintMessage($"Third party transfer failed. Receiver bank account number is invalid.", false);
                return;
            }

            if (selectedBankAccountReceiver.FullName != vMThirdPartyTransfer.RecipientBankAccountName)
            {
                Utility.PrintMessage($"Third party transfer failed. Recipient's account name does not match.", false);
                return;
            }

            // Bind transaction_amt to Transaction object
            // Add transaction record (Giver) - Start            
            InsertTransaction(_selectedUser.AccountNumber, TransactionType.ThirdPartyTransfer, +
                -vMThirdPartyTransfer.TransferAmount, "Transfered " +
                $" to {selectedBankAccountReceiver.AccountNumber} ({selectedBankAccountReceiver.FullName})");
            // Add transaction record (Giver) - End

            // Update balance amount (Giver)
            _selectedUser.AccountBalance = _selectedUser.AccountBalance - vMThirdPartyTransfer.TransferAmount;

            // Add transaction record (Receiver) - Start
            InsertTransaction(selectedBankAccountReceiver.AccountNumber, TransactionType.ThirdPartyTransfer, +
                vMThirdPartyTransfer.TransferAmount, "Transfered " +
                $" from {_selectedUser.AccountNumber} ({_selectedUser.FullName})");
            // Add transaction record (Receiver) - End

            // Update balance amount (Receiver)
            selectedBankAccountReceiver.AccountBalance = selectedBankAccountReceiver.AccountBalance + vMThirdPartyTransfer.TransferAmount;

            Utility.PrintMessage("You have successfully transferred out " +
                $" {Utility.FormatAmount(vMThirdPartyTransfer.TransferAmount)} to {vMThirdPartyTransfer.RecipientBankAccountName}", true);
        }

    }
}
