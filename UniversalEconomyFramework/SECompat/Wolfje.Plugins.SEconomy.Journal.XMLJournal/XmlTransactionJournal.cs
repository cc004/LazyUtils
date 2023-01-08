using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using LazyUtils;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace Wolfje.Plugins.SEconomy.Journal.XMLJournal
{
	public class XmlTransactionJournal : ITransactionJournal
	{
		
		public SEconomy SEconomyInstance
		{
			get;
			set;
		}
		
		public bool JournalSaving
		{
			get;
			set;
		}

		public bool BackupsEnabled
		{
			get;
			set;
		}

        public List<IBankAccount> BankAccounts =>
            TShock.UserAccounts.GetUserAccounts().Select(i => AddBankAccount(i.Name, 0, BankAccountFlags.Enabled, "")).ToList();

        public IEnumerable<ITransaction> Transactions => new ITransaction[0];

		public event EventHandler<PendingTransactionEventArgs> BankTransactionPending;

		public event EventHandler<BankTransferEventArgs> BankTransferCompleted;

		public event EventHandler<JournalLoadingPercentChangedEventArgs> JournalLoadingPercentChanged;

		public XmlTransactionJournal(SEconomy Parent, string JournalSavePath)
		{
		}

		protected async void JournalBackupTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (BackupsEnabled && !JournalSaving)
			{
				await SaveJournalAsync();
			}
		}

		public IBankAccount GetWorldAccount()
		{
			IBankAccount bankAccount = null;
            bankAccount = AddBankAccount("SYSTEM", Main.worldID, BankAccountFlags.Enabled | BankAccountFlags.SystemAccount | BankAccountFlags.LockedToWorld, "World account for world " + Main.worldName);
			return bankAccount;
		}
		
		public IBankAccount AddBankAccount(string UserAccountName, long WorldID, BankAccountFlags Flags, string Description)
		{
			return AddBankAccount(new XmlBankAccount(this)
			{
				UserAccountName = UserAccountName,
				WorldID = WorldID,
				Flags = Flags,
				Description = Description
			});
		}

		public IBankAccount AddBankAccount(IBankAccount Account)
        {
            Account.BankAccountK = Account.UserAccountName.GetHashCode();
			return Account;
		}

		public IBankAccount GetBankAccountByName(string UserAccountName)
        {
            return new XmlBankAccount(this)
            {
                UserAccountName = UserAccountName,
                WorldID = 0,
                Flags = BankAccountFlags.Enabled,
                Description = ""
            };
        }

		public IBankAccount GetBankAccount(long BankAccountK)
        {
            return BankAccounts.Single(i => BankAccountK == i.UserAccountName.GetHashCode());
        }

		public IEnumerable<IBankAccount> GetBankAccountList(long BankAccountK)
		{
            return BankAccounts.Where(i => BankAccountK == i.UserAccountName.GetHashCode());
		}

		public void DumpSummary()
		{
		}

		public async Task DeleteBankAccountAsync(long BankAccountK)
		{
			await Task.Run(delegate
			{
				DeleteBankAccount(BankAccountK);
			});
		}

		public void DeleteBankAccount(long BankAccountK)
		{
		}

		public void SaveJournal()
		{
		}

		public async Task SaveJournalAsync()
		{
		}

		public bool LoadJournal()
		{
			return true;
		}

		public Task<bool> LoadJournalAsync()
		{
			return Task.Factory.StartNew(() => LoadJournal());
		}

		public void BackupJournal()
		{
			SaveJournal();
		}

		public async Task BackupJournalAsync()
		{
			await Task.Factory.StartNew(delegate
			{
				BackupJournal();
			});
		}

		public async Task SquashJournalAsync()
        {
        }

		private bool TransferMaySucceed(IBankAccount FromAccount, IBankAccount ToAccount, Money MoneyNeeded, BankAccountTransferOptions Options)
		{
			if (FromAccount == null || ToAccount == null)
			{
				return false;
			}
			if (!FromAccount.IsSystemAccount && !FromAccount.IsPluginAccount && (Options & BankAccountTransferOptions.AllowDeficitOnNormalAccount) != BankAccountTransferOptions.AllowDeficitOnNormalAccount)
			{
				if ((long)FromAccount.Balance >= (long)MoneyNeeded)
				{
					return (long)MoneyNeeded > 0;
				}
				return false;
			}
			return true;
		}

		private ITransaction BeginSourceTransaction(long BankAccountK, Money Amount, string Message)
		{
			IBankAccount bankAccount = GetBankAccount(BankAccountK);
			ITransaction transaction = new XmlTransaction(bankAccount)
			{
				Flags = BankAccountTransactionFlags.FundsAvailable,
				TransactionDateUtc = DateTime.UtcNow,
				Amount = (long)Amount * -1
			};
			if (!string.IsNullOrEmpty(Message))
			{
				transaction.Message = Message;
			}
			return bankAccount.AddTransaction(transaction);
		}

		private ITransaction FinishEndTransaction(long SourceBankTransactionKey, IBankAccount ToAccount, Money Amount, string Message)
		{
			ITransaction transaction = new XmlTransaction(ToAccount);
			transaction.BankAccountFK = ToAccount.BankAccountK;
			transaction.Flags = BankAccountTransactionFlags.FundsAvailable;
			transaction.TransactionDateUtc = DateTime.UtcNow;
			transaction.Amount = Amount;
			transaction.BankAccountTransactionFK = SourceBankTransactionKey;
			if (!string.IsNullOrEmpty(Message))
			{
				transaction.Message = Message;
			}
			return ToAccount.AddTransaction(transaction);
		}

		private void BindTransactions(ref ITransaction SourceTransaction, ref ITransaction DestTransaction)
		{
			SourceTransaction.BankAccountTransactionFK = DestTransaction.BankAccountTransactionK;
			DestTransaction.BankAccountTransactionFK = SourceTransaction.BankAccountTransactionK;
		}

		public Task<BankTransferEventArgs> TransferBetweenAsync(IBankAccount FromAccount, IBankAccount ToAccount, Money Amount, BankAccountTransferOptions Options, string TransactionMessage, string JournalMessage)
		{
			return Task.Factory.StartNew(() => TransferBetween(FromAccount, ToAccount, Amount, Options, TransactionMessage, JournalMessage));
		}

		public BankTransferEventArgs TransferBetween(IBankAccount FromAccount, IBankAccount ToAccount, Money Amount, BankAccountTransferOptions Options, string TransactionMessage, string JournalMessage)
		{
			BankTransferEventArgs bankTransferEventArgs = new BankTransferEventArgs();
			_ = Guid.Empty;
			try
			{
				if (ToAccount != null && TransferMaySucceed(FromAccount, ToAccount, Amount, Options))
				{
					PendingTransactionEventArgs pendingTransactionEventArgs = new PendingTransactionEventArgs(FromAccount, ToAccount, Amount, Options, TransactionMessage, JournalMessage);
					if (this.BankTransactionPending != null)
					{
						this.BankTransactionPending(this, pendingTransactionEventArgs);
					}
					if (pendingTransactionEventArgs == null)
					{
						return bankTransferEventArgs;
					}
					bankTransferEventArgs.Amount = pendingTransactionEventArgs.Amount;
					bankTransferEventArgs.SenderAccount = pendingTransactionEventArgs.FromAccount;
					bankTransferEventArgs.ReceiverAccount = pendingTransactionEventArgs.ToAccount;
					bankTransferEventArgs.TransferOptions = Options;
					bankTransferEventArgs.TransferSucceeded = false;
					bankTransferEventArgs.TransactionMessage = pendingTransactionEventArgs.TransactionMessage;
					if (pendingTransactionEventArgs.IsCancelled)
					{
						return bankTransferEventArgs;
					}
					ITransaction SourceTransaction = BeginSourceTransaction(FromAccount.BankAccountK, pendingTransactionEventArgs.Amount, pendingTransactionEventArgs.JournalLogMessage);
					if (SourceTransaction != null)
					{
						ITransaction DestTransaction = FinishEndTransaction(SourceTransaction.BankAccountTransactionK, ToAccount, pendingTransactionEventArgs.Amount, pendingTransactionEventArgs.JournalLogMessage);
						if (DestTransaction != null)
						{
							BindTransactions(ref SourceTransaction, ref DestTransaction);
							bankTransferEventArgs.TransactionID = SourceTransaction.BankAccountTransactionK;
							IBankAccount bankAccount = FromAccount;
							bankAccount.Balance = (long)bankAccount.Balance + (long)Amount * -1;
							ToAccount.Balance = (long)ToAccount.Balance + (long)Amount;
							bankTransferEventArgs.TransferSucceeded = true;
						}
					}
				}
				else
				{
					bankTransferEventArgs.TransferSucceeded = false;
					TSPlayer tSPlayer;
					if ((tSPlayer = TShock.Players.FirstOrDefault((TSPlayer i) => i.Name == FromAccount.UserAccountName)) == null)
					{
						return bankTransferEventArgs;
					}
					if (!ToAccount.IsSystemAccount && !ToAccount.IsPluginAccount)
					{
						if ((long)Amount < 0)
						{
							tSPlayer.SendErrorMessage(SEconomyPlugin.Locale.StringOrDefault(83, "Invalid amount."));
						}
						else
						{
							tSPlayer.SendErrorMessage(SEconomyPlugin.Locale.StringOrDefault(84, "You need {0} more to make this payment."), ((Money)((long)FromAccount.Balance - (long)Amount)).ToLongString());
						}
					}
				}
			}
			catch (Exception ex)
			{
				Exception ex3 = (bankTransferEventArgs.Exception = ex);
				bankTransferEventArgs.TransferSucceeded = false;
			}
			if (this.BankTransferCompleted != null)
			{
				this.BankTransferCompleted(this, bankTransferEventArgs);
			}
			return bankTransferEventArgs;
		}

		public void CleanJournal(PurgeOptions options)
		{
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}
	}
}
