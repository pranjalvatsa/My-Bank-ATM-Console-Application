using System;
using System.ComponentModel;

namespace ATM.Repository.Enum
{
    /// <summary>
    /// Enum to manage options of secure menu
    /// </summary>
    public enum SecureMenu
    {
        //Start enum value from 1 as default enum value starts from 0
        [Description("Check balance")]
        CheckBalance = 1,

        [Description("Place Deposit")]
        PlaceDeposit = 2,

        [Description("Make Withdrawal")]
        MakeWithdrawal = 3,

        [Description("Third Party Transfer")]
        ThirdPartyTransfer = 4,

        [Description("Transaction")]
        ViewTransaction = 5,

        [Description("Logout")]
        Logout = 6
    }
}
