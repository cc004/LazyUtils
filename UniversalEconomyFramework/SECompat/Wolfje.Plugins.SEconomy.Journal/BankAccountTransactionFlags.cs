using System;

namespace Wolfje.Plugins.SEconomy.Journal;

[Flags]
public enum BankAccountTransactionFlags
{
    FundsAvailable = 0x1,
    Squashed = 0x2
}