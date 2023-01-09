using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using UniversalEconomyFramework;

namespace Wolfje.Plugins.SEconomy.Journal.XMLJournal;

public class XmlBankAccount : IBankAccount
{
    private ITransactionJournal owningJournal;

    private List<ITransaction> transactions;

    public long BankAccountK
    {
        get;
        set;
    }

    public string OldBankAccountK
    {
        get;
        set;
    }

    public string UserAccountName
    {
        get;
        set;
    }

    public long WorldID
    {
        get;
        set;
    }

    public BankAccountFlags Flags
    {
        get;
        set;
    }

    public string Description
    {
        get;
        set;
    }

    private const string FRAMEWORK = "SEconomy";

    public Money Balance
    {
        get
        {
            using (var query = Economy.Query(FRAMEWORK, UserAccountName))
                return (int)(query.Single().bank / Economy.Multiplier(FRAMEWORK));
        }
        set
        {
            using (var query = Economy.Query(FRAMEWORK, UserAccountName))
                query.Set(e => e.bank, value * Economy.Multiplier(FRAMEWORK)).Update();
        }
    }

    public bool IsAccountEnabled => (Flags & BankAccountFlags.Enabled) == BankAccountFlags.Enabled;

    public bool IsSystemAccount => (Flags & BankAccountFlags.SystemAccount) == BankAccountFlags.SystemAccount;

    public bool IsLockedToWorld => (Flags & BankAccountFlags.LockedToWorld) == BankAccountFlags.LockedToWorld;

    public bool IsPluginAccount => (Flags & BankAccountFlags.PluginAccount) == BankAccountFlags.PluginAccount;

    public List<ITransaction> Transactions => transactions;

    public ITransactionJournal OwningJournal => owningJournal;

    public XmlBankAccount(ITransactionJournal OwningJournal)
    {
        owningJournal = OwningJournal;
    }

    public void SyncBalance()
    {
    }

    public Task SyncBalanceAsync()
    {
        return Task.Factory.StartNew(delegate
        {
            SyncBalance();
        });
    }

    public BankTransferEventArgs TransferTo(IBankAccount Account, Money Amount, BankAccountTransferOptions Options, string TransactionMessage, string JournalMessage)
    {
        return owningJournal.TransferBetween(this, Account, Amount, Options, TransactionMessage, JournalMessage);
    }

    public async Task<BankTransferEventArgs> TransferToAsync(int Index, Money Amount, BankAccountTransferOptions Options, string TransactionMessage, string JournalMessage)
    {
        IBankAccount account = SEconomyPlugin.Instance.GetBankAccount(Index);
        return await Task.Factory.StartNew(() => TransferTo(account, Amount, Options, TransactionMessage, JournalMessage));
    }

    public async Task<BankTransferEventArgs> TransferToAsync(IBankAccount ToAccount, Money Amount, BankAccountTransferOptions Options, string TransactionMessage, string JournalMessage)
    {
        return await Task.Factory.StartNew(() => TransferTo(ToAccount, Amount, Options, TransactionMessage, JournalMessage));
    }

    public ITransaction AddTransaction(ITransaction Transaction)
    {
        Balance += Transaction.Amount;
        return Transaction;
    }

    public void ResetAccountTransactions(long BankAccountK)
    {
        Balance = 0;
    }

    public Task ResetAccountTransactionsAsync(long BankAccountK)
    {
        return Task.Factory.StartNew(delegate
        {
            ResetAccountTransactions(BankAccountK);
        });
    }
}