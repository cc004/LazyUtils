﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TShockAPI;

namespace LazyUtils.Commands;

internal partial class Command
{
    private class HelpCommand : CommandBase
    {
        private readonly Command parent;
        public HelpCommand(Command parent, string infoPrefix)
        {
            this.parent = parent;
            this.permissions = Array.Empty<string>();
            this.info = infoPrefix + "help";
        }

        public override ParseResult TryParse(CommandArgs args, int current)
        {
            if (current != args.Parameters.Count - 1)
            {
                return this.GetResult(Math.Abs(args.Parameters.Count - 1 - current));
            }

            if (args.Parameters[current] != "help")
            {
                return this.GetResult(1);
            }

            args.Player.SendInfoMessage("available usage:");
            foreach (var sub in this.parent._dict.Values.SelectMany(subs => subs).Concat(this.parent._main).Distinct()
                         .Where(sub => sub.CanExec(args.Player)))
            {
                args.Player.SendInfoMessage(sub.ToString());
            }

            return this.GetResult(0);
        }
    }
}