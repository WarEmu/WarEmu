/*
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

using FrameWork;
using Google.ProtocolBuffers;
namespace Common
{
    public enum AuthResult
    {
        AUTH_SUCCESS = 0x00,
        AUTH_ACCT_EXPIRED = 0x07,
        AUTH_ACCT_BAD_USERNAME_PASSWORD = 0x09,
        AUTH_ACCT_TERMINATED = 0x0D,
        AUTH_ACCT_SUSPENDED = 0x0E
    };

    [Rpc(true, System.Runtime.Remoting.WellKnownObjectMode.Singleton,1)]
    public class AccountMgr : RpcObject
    {
        // Account Database
        static public MySQLObjectDatabase Database = null;

        #region Account

        // Account : Username,Account
        public Dictionary<string, Account> _Accounts = new Dictionary<string, Account>();

        public bool LoadAccount(string Username)
        {
            Username = Username.ToLower();
            Log.Info("LoadAccount", Username);
            try
            {
                lock (_Accounts)
                    if (_Accounts.ContainsKey(Username))
                        return true;

                Account Acct = Database.SelectObject<Account>("Username='" + Database.Escape(Username) + "'");

                if (Acct == null)
                {
                    Log.Error("LoadAccount", "Compte Introuvable : " + Username);
                    return false;
                }

                lock (_Accounts)
                    _Accounts.Add(Username, Acct);
            }
            catch (Exception e)
            {
                Log.Error("LoadAccount", e.ToString());
                return false;
            }

            return true;
        }

        public Account GetAccount(string Username)
        {
            Username = Username.ToLower();

            Log.Info("GetAccount", Username);

            if (!LoadAccount(Username))
            {
                Log.Error("GetAccount", "Compte Introuvable : " + Username);
                return null;
            }

            lock (_Accounts)
                return _Accounts[Username];
        }
        public bool CheckAccount(string Username, byte[] Password)
        {
            Username = Username.ToLower();

            Log.Info("CheckAccount", Username + " : " + Password);

            try
            {
                Account Acct = GetAccount(Username);

                if (Acct == null)
                {
                    Log.Error("CheckAccount", "Compte introuvable : " + Username);
                    return false;
                }

                SHA256Managed Sha = new SHA256Managed();
                string CP = Acct.Username + ":" + Acct.Password;
                Log.Error("Cp", "."+CP+".");

                byte[] Result = Sha.ComputeHash(UTF8Encoding.UTF8.GetBytes(CP));
                Log.Dump("Result", Result, 0, Result.Length);
                Log.Dump("Pass", Password, 0, Password.Length);

                if (Result.Length != Password.Length)
                {
                    Log.Error("CheckAccount", "Taille du pass invalide : " + Password.Length);
                    return false;
                }

                for (int i = 0; i < Result.Length; ++i)
                    if (Result[i] != Password[i])
                        return false;

            }
            catch (Exception e)
            {
                Log.Error("CheckAccount", e.ToString());
                return false;
            }

            return true;
        }
        public string GenerateToken(string Username)
        {
            Username = Username.ToLower();
            Log.Info("GenerateToken", Username);
            Account Acct = GetAccount(Username);

            if (Acct == null)
            {
                Log.Error("GenerateToken", "Compte introuvable : " + Username);
                return "ERREUR";
            }

            /*string Token = Guid.NewGuid().ToString();
            if (Token.Length <= 34)
            {
                for (int i = Token.Length; i < 34; ++i)
                    Token += "X";
            }
            else Token = Token.Substring(0, 34);
            
            Acct.Token = Token;
            */

            var md5bytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(md5bytes);
            }

            string Token = BitConverter.ToString(md5bytes).Replace("-", "");
            Acct.Token = Token;

            Log.Debug("GenerateToken",Acct.Token);
            
            Database.SaveObject(Acct);

            return Acct.Token;
        }

        public AuthResult CheckToken(string Username, string Token)
        {
            Username = Username.ToLower();

            Account Acct = GetAccount(Username);
            if (Acct == null)
                return AuthResult.AUTH_ACCT_BAD_USERNAME_PASSWORD;

            if (Acct.Token != Token)
                return AuthResult.AUTH_ACCT_BAD_USERNAME_PASSWORD;
            
            return AuthResult.AUTH_SUCCESS;
        }

        public ResultCode CheckToken(string Token)
        {
            //
            return ResultCode.RES_SUCCESS;
        }

        #endregion

        #region Realm

        public Dictionary<byte, Realm> _Realms = new Dictionary<byte, Realm>();
        
        public void LoadRealms()
        {
            Realm[] Rms = Database.SelectAllObjects<Realm>().ToArray();
            foreach (Realm Rm in Rms)
                AddRealm(Rm);
        }
        public bool AddRealm(Realm Rm)
        {
            lock (_Realms)
            {
                if (_Realms.ContainsKey(Rm.RealmId))
                    return false;

                Log.Info("AddRealm", "New Realm : " + Rm.Name);

                _Realms.Add(Rm.RealmId, Rm);
            }

            return true;
        }
        public Realm GetRealm(byte RealmId)
        {
            Log.Info("GetRealm", "RealmId = " + RealmId);
            lock (_Realms)
                if (_Realms.ContainsKey(RealmId))
                    return _Realms[RealmId];

            return null;
        }
        public List<Realm> GetRealms()
        {
            List<Realm> Rms = new List<Realm>();
            Rms.AddRange(_Realms.Values);
            return Rms;
        }
        public Realm GetRealmByRpc(int RpcId)
        {
            lock (_Realms)
                return _Realms.Values.ToList().Find(info => info.Info != null && info.Info.RpcID == RpcId);
        }
        public bool UpdateRealm(RpcClientInfo Info, byte RealmId)
        {
            Realm Rm = GetRealm(RealmId);

            if (Rm != null)
            {
                Log.Success("Realm", "Realm (" + Rm.Name + ") online at " + Info.Ip+":"+Info.Port);
                Rm.Info = Info;
            }
            else
            {
                Log.Error("UpdateRealm", "Realm (" + RealmId + ") missing : Please complete the table 'realm'");
                return false;
            }

            return true;
        }
        public void UpdateRealm(byte RealmId, uint OnlinePlayers, uint OrderCount, uint DestructionCount)
        {
            Realm Rm = GetRealm(RealmId);

            if (Rm == null)
                return;

            Rm.OnlinePlayers = OnlinePlayers;
            Rm.OrderCount = OrderCount;
            Rm.DestructionCount = DestructionCount;
        }

        public void UpdateRealmCharacters(byte RealmId, uint OrderCharacters, uint DestruCharacters)
        {
            Realm Rm = GetRealm(RealmId);

            if (Rm == null)
                return;

            Rm.OrderCharacters = OrderCharacters;
            Rm.DestruCharacters = DestruCharacters;
            Rm.Dirty = true;
            Database.ExecuteNonQuery("UPDATE realms SET OrderCharacters =" + OrderCharacters + ", DestruCharacters=" + DestruCharacters + " WHERE RealmId = " + RealmId);
        }

        private ClusterProp setProp(string name, string value)
        {
            return ClusterProp.CreateBuilder().SetPropName(name)
                                              .SetPropValue(value)
                                              .Build();
        }
        public byte[] BuildClusterList()
        {
       
            GetClusterListReply.Builder ClusterListReplay = GetClusterListReply.CreateBuilder();

            lock (_Realms)
            {
                Log.Info("BuildRealm", "Sending " + _Realms.Count + " realm(s)");

                ClusterInfo.Builder cluster = ClusterInfo.CreateBuilder();
                foreach (Realm Rm in _Realms.Values)
                {
                    cluster.SetClusterId(Rm.RealmId)
                           .SetClusterName(Rm.Name)
                           .SetLobbyHost(Rm.Adresse)
                           .SetLobbyPort((uint)Rm.Port)
                           .SetLanguageId(0)
                           .SetMaxClusterPop(500)
                           .SetClusterPopStatus(ClusterPopStatus.POP_UNKNOWN)
                           .SetClusterStatus(ClusterStatus.STATUS_ONLINE);

                    cluster.AddServerList(
                        ServerInfo.CreateBuilder().SetServerId(Rm.RealmId)
                                                  .SetServerName(Rm.Name)
                                                  .Build());

                    cluster.AddPropertyList(setProp("setting.allow_trials", Rm.AllowTrials));
                    cluster.AddPropertyList(setProp("setting.charxferavailable", Rm.CharfxerAvailable));
                    cluster.AddPropertyList(setProp("setting.language", Rm.Language));
                    cluster.AddPropertyList(setProp("setting.legacy", Rm.Legacy));
                    cluster.AddPropertyList(setProp("setting.manualbonus.realm.destruction", Rm.BonusDestruction));
                    cluster.AddPropertyList(setProp("setting.manualbonus.realm.order", Rm.BonusOrder));
                    cluster.AddPropertyList(setProp("setting.min_cross_realm_account_level", "0"));
                    cluster.AddPropertyList(setProp("setting.name", Rm.Name));
                    cluster.AddPropertyList(setProp("setting.net.address", Rm.Adresse.ToString()));
                    cluster.AddPropertyList(setProp("setting.net.port", Rm.Port.ToString()));
                    cluster.AddPropertyList(setProp("setting.redirect", Rm.Redirect));
                    cluster.AddPropertyList(setProp("setting.region", Rm.Region));
                    cluster.AddPropertyList(setProp("setting.retired", Rm.Retired));
                    cluster.AddPropertyList(setProp("status.queue.Destruction.waiting", Rm.WaitingDestruction));
                    cluster.AddPropertyList(setProp("status.queue.Order.waiting", Rm.WaitingOrder));
                    cluster.AddPropertyList(setProp("status.realm.destruction.density", Rm.DensityDestruction));
                    cluster.AddPropertyList(setProp("status.realm.order.density", Rm.DensityOrder));
                    cluster.AddPropertyList(setProp("status.servertype.openrvr", Rm.OpenRvr));
                    cluster.AddPropertyList(setProp("status.servertype.rp", Rm.Rp));
                    cluster.AddPropertyList(setProp("status.status", Rm.Status));
                    cluster.Build();
                    ClusterListReplay.AddClusterList(cluster);
                }
            }
            ClusterListReplay.ResultCode = ResultCode.RES_SUCCESS;
            return ClusterListReplay.Build().ToByteArray();

        }


        public override void  OnClientDisconnected(RpcClientInfo Info)
        {
            Realm Rm = GetRealmByRpc(Info.RpcID);
            if (Rm != null && Rm.Info.RpcID == Info.RpcID)
            {
                Log.Error("Realm", "Realm offline : " + Rm.Name);
                Rm.Info = null;
            }
        }

        #endregion
    }
}
