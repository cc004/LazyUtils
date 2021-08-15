using System;

namespace Wolfje.Plugins.SEconomy.Journal
{
	[Flags]
	public enum BankAccountTransferOptions
	{
		None = 0x0,
		AnnounceToReceiver = 0x1,
		AnnounceToSender = 0x2,
		AllowDeficitOnNormalAccount = 0x4,
		PvP = 0x8,
		MoneyTakenOnDeath = 0x10,
		IsPlayerToPlayerTransfer = 0x20,
		IsPayment = 0x40,
		SuppressDefaultAnnounceMessages = 0x80
	}
}
