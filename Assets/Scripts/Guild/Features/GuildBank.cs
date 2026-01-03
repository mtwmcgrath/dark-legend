using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DarkLegend.Guild
{
    /// <summary>
    /// Guild bank system with permissions and transaction logs
    /// Hệ thống kho guild với quyền hạn và nhật ký giao dịch
    /// </summary>
    public class GuildBank : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GuildManager guildManager;
        
        // Guild banks storage / Lưu trữ kho guild
        private Dictionary<string, GuildBankData> guildBanks = new Dictionary<string, GuildBankData>();
        
        /// <summary>
        /// Guild bank data
        /// Dữ liệu kho guild
        /// </summary>
        [Serializable]
        public class GuildBankData
        {
            public string GuildId;
            public List<BankItem> Items;
            public int Zen;
            public List<BankTransaction> TransactionLog;
            
            public GuildBankData(string guildId)
            {
                GuildId = guildId;
                Items = new List<BankItem>();
                Zen = 0;
                TransactionLog = new List<BankTransaction>();
            }
        }
        
        /// <summary>
        /// Item in guild bank
        /// Vật phẩm trong kho guild
        /// </summary>
        [Serializable]
        public class BankItem
        {
            public string ItemId;
            public string ItemName;
            public ItemRarity Rarity;
            public int Quantity;
            public string DepositedBy;
            public DateTime DepositDate;
        }
        
        /// <summary>
        /// Item rarity levels
        /// Cấp độ hiếm của vật phẩm
        /// </summary>
        public enum ItemRarity
        {
            Common,
            Uncommon,
            Rare,
            Epic,
            Legendary,
            Mythic
        }
        
        /// <summary>
        /// Bank transaction record
        /// Bản ghi giao dịch kho
        /// </summary>
        [Serializable]
        public class BankTransaction
        {
            public string TransactionId;
            public DateTime Timestamp;
            public TransactionType Type;
            public string PlayerId;
            public string PlayerName;
            public string ItemName;
            public int Quantity;
            public int ZenAmount;
        }
        
        public enum TransactionType
        {
            DepositItem,
            WithdrawItem,
            DepositZen,
            WithdrawZen
        }
        
        private void Awake()
        {
            if (guildManager == null)
            {
                guildManager = GuildManager.Instance;
            }
        }
        
        /// <summary>
        /// Get or create guild bank
        /// Lấy hoặc tạo kho guild
        /// </summary>
        private GuildBankData GetOrCreateBank(string guildId)
        {
            if (!guildBanks.ContainsKey(guildId))
            {
                guildBanks[guildId] = new GuildBankData(guildId);
            }
            return guildBanks[guildId];
        }
        
        #region Item Operations / Thao tác vật phẩm
        
        /// <summary>
        /// Deposit item to guild bank
        /// Gửi vật phẩm vào kho guild
        /// </summary>
        public bool DepositItem(string guildId, string playerId, BankItem item)
        {
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null) return false;
            
            GuildMember member = guild.GetMember(playerId);
            if (member == null) return false;
            
            GuildRankPermissions permissions = member.GetPermissions();
            if (!permissions.CanDepositBank)
            {
                Debug.LogError("You don't have permission to deposit items.");
                return false;
            }
            
            GuildBankData bank = GetOrCreateBank(guildId);
            GuildData data = guildManager.GetGuildData();
            
            // Check bank capacity
            int maxSlots = data.GetMaxBankSlots(guild.Level);
            if (bank.Items.Count >= maxSlots)
            {
                Debug.LogError("Guild bank is full.");
                return false;
            }
            
            // Add item
            item.DepositedBy = playerId;
            item.DepositDate = DateTime.Now;
            bank.Items.Add(item);
            
            // Log transaction
            LogTransaction(bank, new BankTransaction
            {
                TransactionId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now,
                Type = TransactionType.DepositItem,
                PlayerId = playerId,
                PlayerName = member.PlayerName,
                ItemName = item.ItemName,
                Quantity = item.Quantity
            });
            
            Debug.Log($"{member.PlayerName} deposited {item.ItemName} x{item.Quantity}");
            return true;
        }
        
        /// <summary>
        /// Withdraw item from guild bank
        /// Rút vật phẩm từ kho guild
        /// </summary>
        public bool WithdrawItem(string guildId, string playerId, string itemId)
        {
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null) return false;
            
            GuildMember member = guild.GetMember(playerId);
            if (member == null) return false;
            
            GuildRankPermissions permissions = member.GetPermissions();
            if (!permissions.CanWithdrawBank)
            {
                Debug.LogError("You don't have permission to withdraw items.");
                return false;
            }
            
            GuildBankData bank = GetOrCreateBank(guildId);
            BankItem item = bank.Items.FirstOrDefault(i => i.ItemId == itemId);
            
            if (item == null)
            {
                Debug.LogError("Item not found in guild bank.");
                return false;
            }
            
            // Check daily limit (implement daily tracking here)
            // For now, just check permission limit
            
            // Remove item
            bank.Items.Remove(item);
            
            // Log transaction
            LogTransaction(bank, new BankTransaction
            {
                TransactionId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now,
                Type = TransactionType.WithdrawItem,
                PlayerId = playerId,
                PlayerName = member.PlayerName,
                ItemName = item.ItemName,
                Quantity = item.Quantity
            });
            
            Debug.Log($"{member.PlayerName} withdrew {item.ItemName} x{item.Quantity}");
            return true;
        }
        
        #endregion
        
        #region Zen Operations / Thao tác tiền
        
        /// <summary>
        /// Deposit Zen to guild bank
        /// Gửi Zen vào kho guild
        /// </summary>
        public bool DepositZen(string guildId, string playerId, int amount)
        {
            if (amount <= 0) return false;
            
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null) return false;
            
            GuildMember member = guild.GetMember(playerId);
            if (member == null) return false;
            
            GuildRankPermissions permissions = member.GetPermissions();
            if (!permissions.CanDepositBank)
            {
                Debug.LogError("You don't have permission to deposit Zen.");
                return false;
            }
            
            GuildBankData bank = GetOrCreateBank(guildId);
            bank.Zen += amount;
            
            // Add contribution
            member.AddContribution(amount / 1000); // 1 contribution per 1000 zen
            
            // Log transaction
            LogTransaction(bank, new BankTransaction
            {
                TransactionId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now,
                Type = TransactionType.DepositZen,
                PlayerId = playerId,
                PlayerName = member.PlayerName,
                ZenAmount = amount
            });
            
            Debug.Log($"{member.PlayerName} deposited {amount:N0} Zen");
            return true;
        }
        
        /// <summary>
        /// Withdraw Zen from guild bank
        /// Rút Zen từ kho guild
        /// </summary>
        public bool WithdrawZen(string guildId, string playerId, int amount)
        {
            if (amount <= 0) return false;
            
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null) return false;
            
            GuildMember member = guild.GetMember(playerId);
            if (member == null) return false;
            
            GuildRankPermissions permissions = member.GetPermissions();
            if (!permissions.CanWithdrawBank)
            {
                Debug.LogError("You don't have permission to withdraw Zen.");
                return false;
            }
            
            GuildBankData bank = GetOrCreateBank(guildId);
            
            if (bank.Zen < amount)
            {
                Debug.LogError("Insufficient Zen in guild bank.");
                return false;
            }
            
            // Check daily limit
            if (permissions.DailyWithdrawLimit >= 0 && amount > permissions.DailyWithdrawLimit)
            {
                Debug.LogError($"Daily withdraw limit is {permissions.DailyWithdrawLimit:N0} Zen.");
                return false;
            }
            
            bank.Zen -= amount;
            
            // Log transaction
            LogTransaction(bank, new BankTransaction
            {
                TransactionId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now,
                Type = TransactionType.WithdrawZen,
                PlayerId = playerId,
                PlayerName = member.PlayerName,
                ZenAmount = amount
            });
            
            Debug.Log($"{member.PlayerName} withdrew {amount:N0} Zen");
            return true;
        }
        
        #endregion
        
        #region Query / Truy vấn
        
        /// <summary>
        /// Get guild bank items
        /// Lấy vật phẩm trong kho guild
        /// </summary>
        public List<BankItem> GetBankItems(string guildId)
        {
            GuildBankData bank = GetOrCreateBank(guildId);
            return new List<BankItem>(bank.Items);
        }
        
        /// <summary>
        /// Get guild bank Zen
        /// Lấy số Zen trong kho guild
        /// </summary>
        public int GetBankZen(string guildId)
        {
            GuildBankData bank = GetOrCreateBank(guildId);
            return bank.Zen;
        }
        
        /// <summary>
        /// Get transaction logs
        /// Lấy nhật ký giao dịch
        /// </summary>
        public List<BankTransaction> GetTransactionLog(string guildId, int limit = 50)
        {
            GuildBankData bank = GetOrCreateBank(guildId);
            return bank.TransactionLog
                .OrderByDescending(t => t.Timestamp)
                .Take(limit)
                .ToList();
        }
        
        #endregion
        
        #region Utility / Tiện ích
        
        /// <summary>
        /// Log bank transaction
        /// Ghi lại giao dịch kho
        /// </summary>
        private void LogTransaction(GuildBankData bank, BankTransaction transaction)
        {
            bank.TransactionLog.Add(transaction);
            
            // Keep only last 1000 transactions
            if (bank.TransactionLog.Count > 1000)
            {
                bank.TransactionLog.RemoveAt(0);
            }
        }
        
        #endregion
    }
}
