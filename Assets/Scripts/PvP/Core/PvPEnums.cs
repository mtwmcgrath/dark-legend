namespace DarkLegend.PvP
{
    /// <summary>
    /// Duel types - Các loại đấu tay đôi
    /// </summary>
    public enum DuelType
    {
        Normal,      // Không ảnh hưởng rank
        Ranked,      // Ảnh hưởng ELO
        Bet,         // Cược Zen/Items
        Tournament   // Trong tournament
    }

    /// <summary>
    /// Arena modes - Các chế độ đấu trường
    /// </summary>
    public enum ArenaMode
    {
        Solo1v1,         // 1 vs 1
        Solo2v2,         // 2 vs 2
        Solo3v3,         // 3 vs 3
        Team5v5,         // 5 vs 5
        FreeForAll       // Battle Royale style
    }

    /// <summary>
    /// PK Status - Trạng thái giết người
    /// </summary>
    public enum PKStatus
    {
        Normal,          // Không giết người, name WHITE
        SelfDefense,     // Bị tấn công trước, name YELLOW (5 min)
        Murderer,        // Giết người, name ORANGE (1-10 kills)
        Outlaw,          // Giết nhiều người, name RED (10+ kills)
        Hero             // Giết Outlaws, name BLUE
    }

    /// <summary>
    /// Rank tiers - Hạng PvP
    /// </summary>
    public enum RankTier
    {
        Bronze_III,      // 0-1099
        Bronze_II,       // 1100-1199
        Bronze_I,        // 1200-1299
        Silver_III,      // 1300-1399
        Silver_II,       // 1400-1499
        Silver_I,        // 1500-1599
        Gold_III,        // 1600-1699
        Gold_II,         // 1700-1799
        Gold_I,          // 1800-1899
        Platinum_III,    // 1900-1999
        Platinum_II,     // 2000-2099
        Platinum_I,      // 2100-2199
        Diamond_III,     // 2200-2299
        Diamond_II,      // 2300-2399
        Diamond_I,       // 2400-2499
        Master,          // 2500-2699
        GrandMaster,     // 2700-2899
        Challenger       // 2900+ (Top 100)
    }

    /// <summary>
    /// Tournament types - Loại giải đấu
    /// </summary>
    public enum TournamentType
    {
        Weekly1v1,           // Every Sunday
        Monthly2v2,          // First Saturday of month
        Seasonal5v5,         // End of season
        GuildTournament,     // Guild vs Guild brackets
        WorldChampionship    // Annual event
    }

    /// <summary>
    /// Bracket types - Loại bảng đấu
    /// </summary>
    public enum BracketType
    {
        SingleElimination,   // Loại trực tiếp
        DoubleElimination    // Thua 2 lần mới bị loại
    }

    /// <summary>
    /// Match state - Trạng thái trận đấu
    /// </summary>
    public enum MatchState
    {
        Waiting,
        Starting,
        InProgress,
        Paused,
        Ended
    }
}
