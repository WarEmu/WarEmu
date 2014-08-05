/*
 * Copyright (C) 2011 APS
 *	http://AllPrivateServer.com
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

using Common;
using FrameWork;

namespace AccountCacher
{
    [ConsoleHandler("create", 2, "New Account <Username,Password,GMLevel(0-3)>")]
    public class CreateAccount : IConsoleHandler
    {
        public bool HandleCommand(string command, List<string> args)
        {
            string Username = args[0];
            string Password = args[1];
            int GmLevel = int.Parse(args[2]);

            Account Acct = Program.AcctMgr.GetAccount(Username);
            if (Acct != null)
            {
                Log.Error("CreateAccount", "This username is already used");
                return false;
            }

            Acct = new Account();
            Acct.Username = Username.ToLower();
            Acct.Password = Password.ToLower();
            Acct.Ip = "127.0.0.1";
            Acct.Token = "";
            Acct.GmLevel = (sbyte)GmLevel;
            AccountMgr.Database.AddObject(Acct);

            return true;
        }
    }
}
