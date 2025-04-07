public class DBStatsEntity
{
    public int PlayerId { get; set; }      // FK a Player
    public int Level { get; set; }
    public int Xp { get; set; }
    public int Gold { get; set; }
    public int BankGold { get; set; }
    public int FreeSkillPoints { get; set; }
    public int Hp { get; set; }
    public int Mp { get; set; }
    public int Sp { get; set; }
    public int MaxHp { get; set; }
    public int MaxMp { get; set; }
    public int MaxSp { get; set; }
    public int Hunger { get; set; }
    public int Thirst { get; set; }
    public int KilledNpcs { get; set; }
    public int KilledUsers { get; set; }
    public int Deaths { get; set; }
}
