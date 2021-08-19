using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text;
using BeanPoints;
using Microsoft.Xna.Framework;
using TShockAPI;
using UniversalEconomyFramework;
using LinqToDB;

namespace BeanPoints
{
	public enum ChangeType
	{
		收入,
		支出
	}
	public class BeanPlayer
	{
		private const string FRAMEWORK = "BeanPoints";

		public string Name { get; set; }

		public int Points { get; set; }

		public TSPlayer Player { get; set; }

		public BeanPlayer(string name, int points, TSPlayer plr)
		{
			Name = name;
			Points = points;
			Player = plr;
		}

		private void LoadFromDB()
		{
			using (var query = Economy.Query(FRAMEWORK, Name))
				Points = (int)(query.Single().bank / Economy.Multiplier(FRAMEWORK));
		}

		private void SaveToDB()
		{
			var val = Points * Economy.Multiplier(FRAMEWORK);
			using (var query = Economy.Query(FRAMEWORK, Name))
				query.Set(e => e.bank, _ => val).Update();
		}

		internal BeanPlayer(string name, TSPlayer plr) : this(name, 0, plr)
        {
			LoadFromDB();
        }
		public BeanPlayer(string name, int points)
		{
			Name = name;
			Points = points;
			Player = null;
		}

		public void AddPoints(int count)
		{
			Points += count;
			SaveToDB();
		}

		public void DecreasePoints(int count)
		{
			Points -= count;
			SaveToDB();
		}

		public void ResetPoints()
		{
			Points = 0;
			SaveToDB();
		}

		public bool ResetPoints(int count)
		{
			if (count < 0)
			{
				return false;
			}
			Points = count;
			SaveToDB();
			return true;
		}

		public static BeanPlayer GetBeanPlayer(string name)
		{
			return new BeanPlayer(name, TShock.Players.FirstOrDefault(p => p.Account?.Name == name));
		}

		public void ShowChangeInfo(ChangeType type, int count)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string value = new string('\n', 9);
			string value2 = new string('\n', 15);
			stringBuilder.Append(value);
			stringBuilder.Append("————————\n");
			stringBuilder.Append($"类型 : {type}\n");
			stringBuilder.Append($"数额 : {count}\n");
			stringBuilder.Append("————————\n");
			stringBuilder.Append(value2);
			stringBuilder.Append(value2);
			stringBuilder.Append(value2);
			if (Player != null)
			{
				Player.SendData(PacketTypes.Status, stringBuilder.ToString(), 0, 0f, 0f, 0f, 0);
			}
		}

		public void SendInfoMessage(string msg)
		{
			Player.SendMessage(msg, Color.DarkTurquoise);
		}

		public void SendMessage(string msg, Color color)
		{
			Player.SendMessage(msg, color);
		}

		public void SendSuccessMessage(string msg)
		{
			Player.SendMessage(msg, Color.MediumAquamarine);
		}

		public void SendErrorMessage(string msg)
		{
			Player.SendMessage(msg, Color.Crimson);
		}

		public void PriceIntoCoins(int aimcount)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (aimcount > Points || aimcount < 0)
			{
				SendMessage("请输入正确数字(0<x<你的余额)", Color.Tomato);
				return;
			}
			int copper, silver, gold, plat;
			Player.GiveItem(71, copper = aimcount - 100 * (aimcount /= 100), 0);
			Player.GiveItem(72, silver = aimcount - 100 * (aimcount /= 100), 0);
			Player.GiveItem(73, gold = aimcount - 100 * (aimcount /= 100), 0);
			Player.GiveItem(74, plat = aimcount, 0);
			DecreasePoints(aimcount);
			SendSuccessMessage($"成功兑换 [i:71]*{copper} [i:72]*{silver} [i:73]*{gold} [i:74]*{plat}");
		}
	}

}