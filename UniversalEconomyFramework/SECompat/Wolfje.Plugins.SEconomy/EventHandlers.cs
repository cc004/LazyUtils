using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using Wolfje.Plugins.SEconomy.Journal;

namespace Wolfje.Plugins.SEconomy;

public class EventHandlers : IDisposable
{
    public SEconomy Parent
    {
        get;
        protected set;
    }

    protected Timer PayRunTimer
    {
        get;
        set;
    }

    public EventHandlers(SEconomy Parent)
    {
        this.Parent = Parent;
        if (Parent.Configuration.PayIntervalMinutes > 0)
        {
            PayRunTimer = new Timer(Parent.Configuration.PayIntervalMinutes * 60000);
            PayRunTimer.Elapsed += PayRunTimer_Elapsed;
            PayRunTimer.Start();
        }
        Parent.RunningJournal.BankTransferCompleted += BankAccount_BankTransferCompleted;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
    }

    protected void BankAccount_BankTransferCompleted(object s, BankTransferEventArgs e)
    {
        if (e.ReceiverAccount != null && (e.TransferOptions & BankAccountTransferOptions.SuppressDefaultAnnounceMessages) != BankAccountTransferOptions.SuppressDefaultAnnounceMessages)
        {
            TSPlayer tSPlayer = TShock.Players.FirstOrDefault((TSPlayer i) => i != null && i.Name == e.SenderAccount.UserAccountName);
            TSPlayer tSPlayer2 = TShock.Players.FirstOrDefault((TSPlayer i) => i != null && i.Name == e.ReceiverAccount.UserAccountName);
            if ((e.TransferOptions & BankAccountTransferOptions.AnnounceToReceiver) == BankAccountTransferOptions.AnnounceToReceiver && e.ReceiverAccount != null && tSPlayer2 != null)
            {
                bool flag = (long)e.Amount > 0 && (e.TransferOptions & BankAccountTransferOptions.IsPayment) == 0;
                string text = string.Format("{5}SEconomy\r\n{0}{1}\r\n因{2}\r\n存款: {3}{4}", flag ? "+" : "-", e.Amount.ToString(), e.TransactionMessage, e.ReceiverAccount.Balance.ToString(), RepeatLineBreaks(59), RepeatLineBreaks(11));
                tSPlayer2.SendData(PacketTypes.Status, text);
            }
            if ((e.TransferOptions & BankAccountTransferOptions.AnnounceToSender) == BankAccountTransferOptions.AnnounceToSender && tSPlayer != null)
            {
                bool flag2 = (long)e.Amount > 0 && (e.TransferOptions & BankAccountTransferOptions.IsPayment) == 0;
                string text2 = string.Format("{5}SEconomy\r\n{0}{1}\r\n因{2}\r\n存款: {3}{4}", flag2 ? "+" : "-", e.Amount.ToString(), e.TransactionMessage, e.SenderAccount.Balance.ToString(), RepeatLineBreaks(59), RepeatLineBreaks(11));
                tSPlayer.SendData(PacketTypes.Status, text2);
            }
        }
    }

    protected string RepeatLineBreaks(int number)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < number; i++)
        {
            stringBuilder.Append("\r\n");
        }
        return stringBuilder.ToString();
    }
		
    protected void PayRunTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        Money Money = 0L;
        if (Parent.Configuration.PayIntervalMinutes <= 0 || string.IsNullOrEmpty(Parent.Configuration.IntervalPayAmount) || !Money.TryParse(Parent.Configuration.IntervalPayAmount, out Money) || (long)Money <= 0)
        {
            return;
        }
        TSPlayer[] players = TShock.Players;
        foreach (TSPlayer tSPlayer in players)
        {
            if (tSPlayer != null && Parent != null)
            {
                TimeSpan? timeSpan;
                TimeSpan? timeSpan2 = (timeSpan = Parent.PlayerIdleSince(tSPlayer.TPlayer));
                IBankAccount bankAccount;
                if (timeSpan2.HasValue && !(timeSpan.Value.TotalMinutes > (double)Parent.Configuration.IdleThresholdMinutes) && (bankAccount = Parent.GetBankAccount(tSPlayer)) != null)
                {
                    Parent.WorldAccount.TransferTo(bankAccount, Money, BankAccountTransferOptions.AnnounceToReceiver, "棒棒哒", "棒棒哒");
                }
            }
        }
    }

    protected void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        if (!e.Observed)
        {
            TShock.Log.ConsoleError(SEconomyPlugin.Locale.StringOrDefault(27, "seconomy async: error occurred on a task thread: ") + e.Exception.Flatten().ToString());
            e.SetObserved();
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Parent.RunningJournal.BankTransferCompleted -= BankAccount_BankTransferCompleted;
            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
            PayRunTimer.Elapsed -= PayRunTimer_Elapsed;
            PayRunTimer.Dispose();
        }
    }
}