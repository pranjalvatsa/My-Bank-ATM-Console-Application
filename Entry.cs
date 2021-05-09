using System;

namespace MyBankATMApp
{
    class Entry
    {
        static void Main(string[] args)
        {
            var app = new ATMApp();
            app.Initialise();
            app.Execute();
        }
    }
}
