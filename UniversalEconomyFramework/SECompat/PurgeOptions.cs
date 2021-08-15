using System;

[Flags]
public enum PurgeOptions
{
	RemoveOrphanedAccounts = 0x1,
	RemoveZeroBalanceAccounts = 0x2
}
