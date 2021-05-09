using System;
using System.Collections.Generic;
using ATM.Repository.Entities;
using ATM.Utility;
using ATM.Repository.Enum;
using ATM.Repository.Interface;

namespace MyBankATMApp
{
    public class ATMApp:IATMApp, ITransaction,IBankAccountUser
    {
        private List<BankAccountUser> _userList;
        private List<Transaction> _transactions;
        private BankAccountUser _selectedUser;
        private readonly AtmScreen _screen;
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

            while (true)
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
            bool isLoginPassed = false;

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
                /*case (int)SecureMenu.ThirdPartyTransfer:

                    var vMThirdPartyTransfer = _screen.ThirdPartyTransferForm();
                    PerformThirdPartyTransfer(vMThirdPartyTransfer);
                    break;*/
                case (int)SecureMenu.ViewTransaction:
                    ViewTransaction();
                    break;

                case (int)SecureMenu.Logout:
                    AtmScreen.LogoutProgress();
                    Utility.PrintConsoleWriteLine("You have succesfully logout. Please collect your ATM card.");
                    ClearSession();
                    Execute();
                    break;
                default:
                    Utility.PrintMessage("Invalid Option Entered.", false);

                    break;
            }
        }

        private void ClearSession()
        {
            throw new NotImplementedException();
        }
        /*
        public void InsertTransaction(long _accountId, TransactionType transctiontype, decimal _tranAmount, string _desc)
        {
            throw new NotImplementedException();
        }

        public void ViewTransaction()
        {
            throw new NotImplementedException();
        }

        public void CheckBalance()
        {
            throw new NotImplementedException();
        }

        public void PlaceDeposit()
        {
            throw new NotImplementedException();
        }
        /*
        public void MakeWithdrawal()
        {
            throw new NotImplementedException();
        }*/
    }
}
