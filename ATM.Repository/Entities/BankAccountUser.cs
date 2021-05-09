using System;
namespace ATM.Repository.Entities
{
    public class BankAccountUser
    {
        public long Id { get; set; }
        public long CardNumber { get; set; }
        public long CardPin { get; set; }
        public string FullName { get; set; }
        public long AccountNumber { get; set; }
        public decimal AccountBalance { get; set; }
        public bool IsLocked { get; set; }
        public int NumberOfLogins { get; set; }
    }

}
