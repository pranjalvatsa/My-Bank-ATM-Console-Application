using System;
namespace ATM.Repository.Interface
{
    public interface IBankAccountUser
    {
        void CheckBalance();
        void PlaceDeposit();
        void MakeWithdrawal();
    }
}
