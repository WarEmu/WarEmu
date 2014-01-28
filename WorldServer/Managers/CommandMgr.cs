
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class CommandHandler
    {
        public delegate void ComHandler(Player Plr, string Text);
        public CommandHandler(string Name, ComHandler Handler, CommandHandler[] Sub)
        {
            this.Name = Name;
            this.SubHandler = Sub;
            this.Handler = Handler;
        }

        public string Name;
        public ComHandler Handler;
        public CommandHandler[] SubHandler;
    }

    static public class CommandMgr
    {
        #region Handlers

        static public CommandHandler[] FriendHandler = new CommandHandler[]
        {
            new CommandHandler("add", AddFriend, null ),
            new CommandHandler("list", null, null ),
            new CommandHandler("remove", RemoveFriend, null ),
            new CommandHandler("toggle", null, null ),
        };

        static public CommandHandler[] AllianceHandler = new CommandHandler[]
        {
            new CommandHandler("form", null, null ),
            new CommandHandler("invite", null, null ),
            new CommandHandler("leave", null, null ),
            new CommandHandler("list", null, null ),
            new CommandHandler("say", null, null ),
        };

        static public CommandHandler[] SocialHandler = new CommandHandler[]
        {
            new CommandHandler("anon", SocialAnon, null ),
            new CommandHandler("hide", SocialHide, null ),
        };

        static public CommandHandler[] Handlers = new CommandHandler[]
        {
	        new CommandHandler("/afk", null, null ),
            new CommandHandler("/alliance", null, AllianceHandler ),
            new CommandHandler("/a", null, null ),
            new CommandHandler("/as", null, null ),
            new CommandHandler("/allianceofficersay", null, null ),
            new CommandHandler("/ao", null, null ),
            new CommandHandler("/aos", null, null ),
            new CommandHandler("/anonymous", null, null ),
            new CommandHandler("/appeal", null, null ),
            new CommandHandler("/appealview", null, null ),
            new CommandHandler("/appealcancel", null, null ),
            new CommandHandler("/assist", null, null ),
            new CommandHandler("/aid", null, null ),
            new CommandHandler("/bug", null, null ),
            new CommandHandler("/channel", null, null ),
            new CommandHandler("/chan", null, null ),
            new CommandHandler("/2", null, null ),
            new CommandHandler("/3", null, null ),
            new CommandHandler("/4", null, null ),
            new CommandHandler("/5", null, null ),
            new CommandHandler("/6", null, null ),
            new CommandHandler("/7", null, null ),
            new CommandHandler("/8", null, null ),
            new CommandHandler("/9", null, null ),
            new CommandHandler("/channelwho", null, null ),
            new CommandHandler("/cloaktoggle", null, null ),
            new CommandHandler("/cloak", null, null ),
            new CommandHandler("/count", null, null ),
            new CommandHandler("/debugwindow", null, null ),
            new CommandHandler("/duelchallenge", null, null ),
            new CommandHandler("/duel", null, null ),
            new CommandHandler("/duelaccept", null, null ),
            new CommandHandler("/dueldecline", null, null ),
            new CommandHandler("/duelcancel", null, null ),
            new CommandHandler("/duelsurrender", null, null ),
            new CommandHandler("/emote", null,null),
            new CommandHandler("::", null, null ),
            new CommandHandler("/emotelist", null, null ),
            new CommandHandler("/friend", null, FriendHandler ),
            new CommandHandler("/guild", null, null ),
            new CommandHandler("/g", null, null ),
            new CommandHandler("/gc", null, null ),
            new CommandHandler("/o", null, null ),
            new CommandHandler("/follow", null, null ),
            new CommandHandler("/helmtoggle", null, null ),
            new CommandHandler("/helm", null, null ),
            new CommandHandler("/help", null, null ),
            new CommandHandler("/hide", null, null ),
            new CommandHandler("/ignoreadd", null, null ),
            new CommandHandler("/ignorelist", null, null ),
            new CommandHandler("/ignoreremove", null, null ),
            new CommandHandler("/ignoretoggle", null, null ),
            new CommandHandler("/ignore", null, null ),
            new CommandHandler("/inspect", null, null ),
            new CommandHandler("/inspectable", null, null ),
            new CommandHandler("/inspectablebraggingrights", null, null ),
            new CommandHandler("/inspectabletradeskills", null, null ),
            new CommandHandler("/join", null, null ),
            new CommandHandler("/language", null, null ),
            new CommandHandler("/lfguild", null, null ),
            new CommandHandler("/lfm", null, null ),
            new CommandHandler("/lfp", null, null ),
            new CommandHandler("/lfg", null, null ),
            new CommandHandler("/location", null, null ),
            new CommandHandler("/loc", null, null ),
            new CommandHandler("/lockouts", null, null ),
            new CommandHandler("/logout", null, null ),
            new CommandHandler("/camp", null, null ),
            new CommandHandler("/openlist", null, null ),
            new CommandHandler("/openpartyinterest", null, null ),
            new CommandHandler("/openjoin", null, null ),
            new CommandHandler("/partyroll", null, null ),
            new CommandHandler("/partyrandom", null, null ),
            new CommandHandler("/partyresetinstance", null, null ),
            new CommandHandler("/partynote", null, null ),
            new CommandHandler("/openpartynote", null, null ),
            new CommandHandler("/lfpnote", null, null ),
            new CommandHandler("/partysay", null, null ),
            new CommandHandler("/p", null, null),
            new CommandHandler("/partyjoin", null, null ),
            new CommandHandler("/partyinvite", null, null ),
            new CommandHandler("/invite", null , null),
            new CommandHandler("/partyinviteopen", null, null ),
            new CommandHandler("/oinvite", null, null ),
            new CommandHandler("/partyremove", null, null ),
            new CommandHandler("/partykick", null, null ),
            new CommandHandler("/partyboot", null, null ),
            new CommandHandler("/kick", null, null ),
            new CommandHandler("/partyleader", null, null ),
            new CommandHandler("/makeleader", null, null ),
            new CommandHandler("/mainassist", null, null ),
            new CommandHandler("/makemainassist", null, null ),
            new CommandHandler("/partyleave", null, null ),
            new CommandHandler("/partyquit", null, null ),
            new CommandHandler("/leave", null, null),
            new CommandHandler("/disband", null, null ),
            new CommandHandler("/partylfwarband", null, null ),
            new CommandHandler("/partylfw", null, null ),
            new CommandHandler("/partyopen", null, null ),
            new CommandHandler("/partyprivate", null, null ),
            new CommandHandler("/partyclose", null, null ),
            new CommandHandler("/partypassword", null, null ),
            new CommandHandler("/partyguildonly", null, null ),
            new CommandHandler("/partyallianceonly", null, null ),
            new CommandHandler("/partylist", null, null ),
            new CommandHandler("/petname", null, null ),
            new CommandHandler("/played", null, null ),
            new CommandHandler("/random", null, null ),
            new CommandHandler("/roll", null, null ),
            new CommandHandler("/reply", null, null ),
            new CommandHandler("/r", null, null ),
            new CommandHandler("/transferguild", null, null ),
            new CommandHandler("/reportgoldseller", null, null ),
            new CommandHandler("/rgs", null, null ),
            new CommandHandler("/rg", null, null ),
            new CommandHandler("/reloadui", null, null ),
            new CommandHandler("/refund", null, null ),
            new CommandHandler("/respec", null, null ),
            new CommandHandler("/rvr", null, null ),
            new CommandHandler("/pvp", null, null ),
            new CommandHandler("/rvrmap", null, null ),
	        new CommandHandler("/quit", PlayerQuit, null),
            new CommandHandler("/exit", PlayerExit, null ),
            new CommandHandler("/q", null, null ),
            new CommandHandler("/say", PlayerSay, null ),
            new CommandHandler("/s", null, null ),
            new CommandHandler("'", null, null ),
            new CommandHandler("/scenariosay", null, null ),
            new CommandHandler("/sc", null, null ),
            new CommandHandler("/scenariotell", null, null ),
            new CommandHandler("/sp", null, null ),
            new CommandHandler("/sp1", null, null ),
            new CommandHandler("/sp2", null, null ),
            new CommandHandler("/sp3", null, null ),
            new CommandHandler("/sp4", null, null ),
            new CommandHandler("/sp5", null, null ),
            new CommandHandler("/sp6", null, null ),
            new CommandHandler("/sp7", null, null ),
            new CommandHandler("/sp8", null, null ),
            new CommandHandler("/sp9", null, null ),
            new CommandHandler("/sp10", null, null ),
            new CommandHandler("/script", null, null ),
            new CommandHandler("/shout", PlayerShout, null),
            new CommandHandler("/social", null, SocialHandler ),
            new CommandHandler("/stuck", null, null ),
            new CommandHandler("/target", null, null ),
	        new CommandHandler("/tell", PlayerWisp, null),
            new CommandHandler("/t", null, null ),
	        new CommandHandler("/whisper", null, null),
            new CommandHandler("/w", null, null ),
            new CommandHandler("/msg", null, null ),
            new CommandHandler("/send", null, null ),
            new CommandHandler("/time", null, null ),
            new CommandHandler("/togglecloakheraldry", null, null ),
            new CommandHandler("/togglealwaysformprivate", null, null ),
            new CommandHandler("/warband", null, null ),
            new CommandHandler("/war", null, null ),
            new CommandHandler("/wbc", null, null ),
            new CommandHandler("/ra", null, null ),
            new CommandHandler("/who", null, null ),
            new CommandHandler("/", null, null)
        };

        #endregion

        #region Commands

        static public void HandleText(Player Plr, string Text)
        {
            if (Text.Length <= 0)
                return;

            if (Text[0] != '&' && Text[0] != '/')
                return;

            Text = Text.Remove(0,1);
            Text = Text.Insert(0,"/");

            GetCommand(Plr, Text, Handlers);
        }
        static public void GetCommand(Player Plr,string Text,CommandHandler[] Handlers)
        {
            string Command = "";
            int Pos = Text.IndexOf(' ');
            if (Pos > 0)
            {
                Command = Text.Substring(0, Pos);
                Text = Text.Remove(0, Pos < Text.Length ? Pos + 1 : Pos);
            }
            else
            {
                Command = Text;
                Text = "";
            }

            Command = Command.Replace("^M", string.Empty);
            Text = Text.Replace("^M", string.Empty);

            for (int i = 0; i < Handlers.Length; ++i)
            {
                if (Handlers[i].Name == Command)
                {
                    if (Handlers[i].Handler != null)
                    {
                        if (GmCommand.HandleCommand(Plr, Text))
                            Handlers[i].Handler.Invoke(Plr, Text);

                        break;
                    }
                    else if (Handlers[i].SubHandler != null)
                    {
                        GetCommand(Plr, Text, Handlers[i].SubHandler);
                        break;
                    }
                }
            }

        }

        #endregion

        #region Functions

        static public void PlayerQuit(Player Plr,string Text) { if(!Plr.Leaving) Plr.Quit(); }
        static public void PlayerExit(Player Plr, string Text) { Plr.DisconnectTime = 0; Plr.Quit(); }

        #region Tchat

        static public void PlayerSay(Player Plr, string Text) { Plr.Say(Text, SystemData.ChatLogFilters.CHATLOGFILTERS_SAY); }
        static public void PlayerWisp(Player Plr, string Text)
        {
            int Pos = Text.IndexOf(' ');
            if(Pos < 0)
                return;

            string ReceiverName = Text.Substring(0, Pos);
            Text = Text.Remove(0, Pos + 1);

            Player Receiver = Player.GetPlayer(ReceiverName);
            if (Receiver == null || !Receiver.IsInWorld())
                Plr.SendLocalizeString(ReceiverName, GameData.Localized_text.TEXT_SN_LISTS_ERR_PLAYER_NOT_FOUND);
            else
            {
                Receiver.SendMessage(Plr.Oid, Plr.Name, Text, SystemData.ChatLogFilters.CHATLOGFILTERS_TELL_RECEIVE);
                Plr.SendMessage(Plr.Oid, ReceiverName, Text, SystemData.ChatLogFilters.CHATLOGFILTERS_TELL_SEND);
            }
        }
        static public void PlayerShout(Player Plr, string Text) { Plr.Say(Text, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT); }

        #endregion

        #region Friends

        static public void AddFriend(Player Plr, string Text) { Plr.SocInterface.AddFriend(Text); }
        static public void RemoveFriend(Player Plr, string Text) { Plr.SocInterface.RemoveFriend(Text); }

        #endregion

        #region Social

        static public void SocialAnon(Player Plr, string Text) { Plr.SocInterface.Anon = !Plr.SocInterface.Anon; }
        static public void SocialHide(Player Plr, string Text) { Plr.SocInterface.Hide = !Plr.SocInterface.Hide; }

        #endregion

        #endregion
    }
}
