using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldServer.Managers
{
    public sealed class ChannelMgr
    {
        private static readonly ChannelMgr me = new ChannelMgr();
        public static Dictionary<string, Channel> Channels;

        public ChannelMgr()
        {
            Channels = new Dictionary<string, Channel>();
        }

        public static bool Exists(String ChannelName)
        {
            return Channels.ContainsKey(ChannelName);
        }

        public static bool IsPlayerInChannel(String channel, Player Plr)
        {
            bool inChannel = false;
            if (Channels.ContainsKey(channel))
            {
                foreach (Player member in Channels[channel].Members)
                {
                    if (member.CharacterId.Equals(Plr.CharacterId))
                    {
                        inChannel = true;
                        break;
                    }
                }
            }

            return inChannel;
        }

        public static void LeaveChannel(String channel, Player Plr)
        {
            if (Channels.ContainsKey(channel))
            {
                // Player leaving channel
                Channels[channel].Members.Remove(Plr);

                if (Plr.Channels.ContainsValue(channel))
                {
                    Plr.Channels.Remove(Plr.Channels.Where(ch => ch.Value.Equals(channel)).FirstOrDefault().Key);
                }
                Plr.SendLocalizeString("Left Channel '" + channel + "'", GameData.Localized_text.CHAT_TAG_DEFAULT);
            }
            else
            {
                Plr.SendLocalizeString("", GameData.Localized_text.TEXT_CHATCHANNEL_DOESNT_EXIST);
            }
        }

        public static void JoinChannel(String channel, Player Plr) {
            if (WorldServer.Managers.ChannelMgr.IsPlayerInChannel(channel, Plr))
            {
                // Player already in channel
                Plr.SendLocalizeString("", GameData.Localized_text.TEXT_CHATCHANNEL_ALREADY_MEMBER);
            }
            else if (Channels.ContainsKey(channel) && !Channels[channel].Faction.Equals(Plr.Faction))
            {
                // Channel belongs to opposite faction
                Plr.SendLocalizeString("The channel '" + channel + "' belongs to the opposing faction.", GameData.Localized_text.CHAT_TAG_DEFAULT);
            }
            else
            {
                if (Channels.ContainsKey(channel))
                {
                    // Player joined channel
                    Channels[channel].Members.Add(Plr);
                }
                else
                {
                    // Start new channel
                    Channels[channel] = new Channel()
                    {
                        Members = new List<Player>(){
                            Plr
                        },
                        Name = channel,
                        IsPublic = true,
                        Faction = Plr.Faction
                    };
                    Plr.SendLocalizeString(channel, GameData.Localized_text.TEXT_CHATCHANNEL_CREATE);
                }

                // Add channel alias number specific to this user
                int index = 3;
                while (true)
                {
                    if (Plr.Channels.ContainsKey(index))
                        index++;
                    else
                        break;
                }
                Plr.Channels.Add(index, channel);

                // Send Message informing the user that he joined the channel
                Plr.SendLocalizeString("Joined Channel '" + channel + "' under alias #" + index , GameData.Localized_text.CHAT_TAG_DEFAULT);
            }
        }
    }
}
