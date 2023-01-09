using System;

namespace Wolfje.Plugins.SEconomy.Journal;

[Flags]
public enum BankAccountFlags
{
    Enabled = 0x1,
    SystemAccount = 0x2,
    LockedToWorld = 0x4,
    PluginAccount = 0x8
}