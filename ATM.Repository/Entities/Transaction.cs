using System;
using ATM.Repository.Enum;

namespace ATM.Repository.Entities
{
    public class Transaction
    {
        public long Id { get; set; }
        public long UserBankAcocunt { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Description { get; set; }
        public decimal TransactionAMount { get; set; }
    }
}
