using System;
using ATM.Repository.Enum;

namespace ATM.Repository.Interface
{
    public interface ITransaction
    {
        void InsertTransaction(long _accountId, TransactionType transctiontype, decimal _tranAmount, string _desc);
        void ViewTransaction();
    }
}
