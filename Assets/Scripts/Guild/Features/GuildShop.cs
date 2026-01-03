using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkLegend.Guild
{
    /// <summary>
    /// Guild shop system for exclusive items
    /// Hệ thống cửa hàng guild cho vật phẩm độc quyền
    /// </summary>
    public class GuildShop : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GuildManager guildManager;
        
        [Header("Shop Configuration")]
        [SerializeField] private List<GuildShopItem> shopItems;
        
        /// <summary>
        /// Guild shop item configuration
        /// Cấu hình vật phẩm cửa hàng guild
        /// </summary>
        [Serializable]
        public class GuildShopItem
        {
            public string ItemId;
            public string ItemName;
            public string Description;
            public GuildShopCurrency Currency;
            public int Price;
            public int MinGuildLevel;
            public int MinMemberRank;  // Minimum GuildRank value
            public int Stock;          // -1 for unlimited
            public bool IsLimited;
            public string IconPath;
        }
        
        /// <summary>
        /// Currency types for guild shop
        /// Loại tiền tệ cho cửa hàng guild
        /// </summary>
        public enum GuildShopCurrency
        {
            GuildPoints,        // Điểm guild
            ContributionPoints, // Điểm đóng góp cá nhân
            Zen                 // Zen từ kho guild
        }
        
        /// <summary>
        /// Purchase record
        /// Bản ghi mua hàng
        /// </summary>
        [Serializable]
        public class PurchaseRecord
        {
            public string PurchaseId;
            public string GuildId;
            public string PlayerId;
            public string PlayerName;
            public string ItemId;
            public string ItemName;
            public GuildShopCurrency Currency;
            public int Price;
            public DateTime PurchaseTime;
        }
        
        // Purchase history / Lịch sử mua hàng
        private List<PurchaseRecord> purchaseHistory = new List<PurchaseRecord>();
        
        // Stock tracking for limited items / Theo dõi tồn kho cho vật phẩm giới hạn
        private Dictionary<string, int> currentStock = new Dictionary<string, int>();
        
        private void Awake()
        {
            if (guildManager == null)
            {
                guildManager = GuildManager.Instance;
            }
            
            InitializeShopItems();
            InitializeStock();
        }
        
        /// <summary>
        /// Initialize default shop items
        /// Khởi tạo vật phẩm cửa hàng mặc định
        /// </summary>
        private void InitializeShopItems()
        {
            if (shopItems == null || shopItems.Count == 0)
            {
                shopItems = new List<GuildShopItem>
                {
                    new GuildShopItem
                    {
                        ItemId = "guild_potion_hp",
                        ItemName = "Guild HP Potion",
                        Description = "Restores 50% HP instantly",
                        Currency = GuildShopCurrency.ContributionPoints,
                        Price = 10,
                        MinGuildLevel = 1,
                        MinMemberRank = (int)GuildRank.Member,
                        Stock = -1,
                        IsLimited = false
                    },
                    new GuildShopItem
                    {
                        ItemId = "guild_potion_mp",
                        ItemName = "Guild MP Potion",
                        Description = "Restores 50% MP instantly",
                        Currency = GuildShopCurrency.ContributionPoints,
                        Price = 10,
                        MinGuildLevel = 1,
                        MinMemberRank = (int)GuildRank.Member,
                        Stock = -1,
                        IsLimited = false
                    },
                    new GuildShopItem
                    {
                        ItemId = "guild_scroll_teleport",
                        ItemName = "Guild Teleport Scroll",
                        Description = "Teleports to guild house",
                        Currency = GuildShopCurrency.ContributionPoints,
                        Price = 50,
                        MinGuildLevel = 5,
                        MinMemberRank = (int)GuildRank.Member,
                        Stock = -1,
                        IsLimited = false
                    },
                    new GuildShopItem
                    {
                        ItemId = "guild_emblem",
                        ItemName = "Guild Emblem",
                        Description = "Displays guild affiliation",
                        Currency = GuildShopCurrency.GuildPoints,
                        Price = 100,
                        MinGuildLevel = 3,
                        MinMemberRank = (int)GuildRank.Member,
                        Stock = -1,
                        IsLimited = false
                    },
                    new GuildShopItem
                    {
                        ItemId = "guild_armor_epic",
                        ItemName = "Guild Master Armor",
                        Description = "Epic armor exclusive to guild",
                        Currency = GuildShopCurrency.GuildPoints,
                        Price = 5000,
                        MinGuildLevel = 20,
                        MinMemberRank = (int)GuildRank.SeniorMember,
                        Stock = 10,
                        IsLimited = true
                    },
                    new GuildShopItem
                    {
                        ItemId = "guild_weapon_legendary",
                        ItemName = "Guild Legendary Weapon",
                        Description = "Legendary weapon for top guilds",
                        Currency = GuildShopCurrency.GuildPoints,
                        Price = 10000,
                        MinGuildLevel = 30,
                        MinMemberRank = (int)GuildRank.BattleMaster,
                        Stock = 5,
                        IsLimited = true
                    }
                };
            }
        }
        
        /// <summary>
        /// Initialize stock for limited items
        /// Khởi tạo tồn kho cho vật phẩm giới hạn
        /// </summary>
        private void InitializeStock()
        {
            foreach (var item in shopItems)
            {
                if (item.IsLimited && item.Stock > 0)
                {
                    currentStock[item.ItemId] = item.Stock;
                }
            }
        }
        
        /// <summary>
        /// Purchase item from guild shop
        /// Mua vật phẩm từ cửa hàng guild
        /// </summary>
        public bool PurchaseItem(string guildId, string playerId, string itemId)
        {
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null)
            {
                Debug.LogError("Guild not found.");
                return false;
            }
            
            GuildMember member = guild.GetMember(playerId);
            if (member == null)
            {
                Debug.LogError("Member not found.");
                return false;
            }
            
            GuildRankPermissions permissions = member.GetPermissions();
            if (!permissions.CanAccessGuildShop)
            {
                Debug.LogError("You don't have permission to access the guild shop.");
                return false;
            }
            
            GuildShopItem item = shopItems.Find(i => i.ItemId == itemId);
            if (item == null)
            {
                Debug.LogError("Item not found.");
                return false;
            }
            
            // Check guild level requirement
            if (guild.Level < item.MinGuildLevel)
            {
                Debug.LogError($"Guild level {item.MinGuildLevel} required.");
                return false;
            }
            
            // Check member rank requirement
            if ((int)member.Rank < item.MinMemberRank)
            {
                Debug.LogError($"Rank {(GuildRank)item.MinMemberRank} or higher required.");
                return false;
            }
            
            // Check stock
            if (item.IsLimited)
            {
                if (!currentStock.ContainsKey(itemId) || currentStock[itemId] <= 0)
                {
                    Debug.LogError("Item is out of stock.");
                    return false;
                }
            }
            
            // Check and deduct currency
            bool canAfford = false;
            switch (item.Currency)
            {
                case GuildShopCurrency.GuildPoints:
                    if (guild.Points >= item.Price)
                    {
                        guild.Points -= item.Price;
                        canAfford = true;
                    }
                    break;
                    
                case GuildShopCurrency.ContributionPoints:
                    if (member.TotalContribution >= item.Price)
                    {
                        member.TotalContribution -= item.Price;
                        canAfford = true;
                    }
                    break;
                    
                case GuildShopCurrency.Zen:
                    // This would require access to GuildBank
                    // For now, just check if guild has enough points
                    if (guild.Points >= item.Price)
                    {
                        guild.Points -= item.Price;
                        canAfford = true;
                    }
                    break;
            }
            
            if (!canAfford)
            {
                Debug.LogError($"Insufficient {item.Currency}. Price: {item.Price}");
                return false;
            }
            
            // Deduct stock if limited
            if (item.IsLimited)
            {
                currentStock[itemId]--;
            }
            
            // Record purchase
            PurchaseRecord record = new PurchaseRecord
            {
                PurchaseId = Guid.NewGuid().ToString(),
                GuildId = guildId,
                PlayerId = playerId,
                PlayerName = member.PlayerName,
                ItemId = itemId,
                ItemName = item.ItemName,
                Currency = item.Currency,
                Price = item.Price,
                PurchaseTime = DateTime.Now
            };
            
            purchaseHistory.Add(record);
            
            Debug.Log($"{member.PlayerName} purchased {item.ItemName} for {item.Price} {item.Currency}");
            OnItemPurchased(guild, member, item);
            
            // Give item to player (implement in actual item system)
            
            return true;
        }
        
        /// <summary>
        /// Get available items for member
        /// Lấy vật phẩm có sẵn cho thành viên
        /// </summary>
        public List<GuildShopItem> GetAvailableItems(string guildId, string playerId)
        {
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null)
            {
                return new List<GuildShopItem>();
            }
            
            GuildMember member = guild.GetMember(playerId);
            if (member == null)
            {
                return new List<GuildShopItem>();
            }
            
            List<GuildShopItem> availableItems = new List<GuildShopItem>();
            
            foreach (var item in shopItems)
            {
                // Check guild level
                if (guild.Level < item.MinGuildLevel)
                    continue;
                
                // Check member rank
                if ((int)member.Rank < item.MinMemberRank)
                    continue;
                
                // Check stock
                if (item.IsLimited && currentStock.ContainsKey(item.ItemId) && currentStock[item.ItemId] <= 0)
                    continue;
                
                availableItems.Add(item);
            }
            
            return availableItems;
        }
        
        /// <summary>
        /// Get purchase history
        /// Lấy lịch sử mua hàng
        /// </summary>
        public List<PurchaseRecord> GetPurchaseHistory(string guildId, int limit = 50)
        {
            return purchaseHistory
                .FindAll(p => p.GuildId == guildId)
                .OrderByDescending(p => p.PurchaseTime)
                .Take(limit)
                .ToList();
        }
        
        /// <summary>
        /// Get remaining stock for item
        /// Lấy số lượng còn lại cho vật phẩm
        /// </summary>
        public int GetRemainingStock(string itemId)
        {
            if (currentStock.ContainsKey(itemId))
            {
                return currentStock[itemId];
            }
            
            GuildShopItem item = shopItems.Find(i => i.ItemId == itemId);
            if (item != null && !item.IsLimited)
            {
                return -1; // Unlimited
            }
            
            return 0;
        }
        
        /// <summary>
        /// Restock limited items (admin function)
        /// Bổ sung hàng cho vật phẩm giới hạn (chức năng admin)
        /// </summary>
        public void RestockItem(string itemId, int amount)
        {
            GuildShopItem item = shopItems.Find(i => i.ItemId == itemId);
            if (item == null || !item.IsLimited)
            {
                return;
            }
            
            if (!currentStock.ContainsKey(itemId))
            {
                currentStock[itemId] = 0;
            }
            
            currentStock[itemId] += amount;
            Debug.Log($"Restocked {item.ItemName}: +{amount} (Total: {currentStock[itemId]})");
        }
        
        /// <summary>
        /// Called when item is purchased
        /// Được gọi khi vật phẩm được mua
        /// </summary>
        private void OnItemPurchased(Guild guild, GuildMember member, GuildShopItem item)
        {
            // Notify guild
            // Update UI
            // Log to analytics
        }
    }
}
