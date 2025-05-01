namespace VentusServer.Models
{
    public class PlayerUpdateDTO
    {
        public required string Name;
        public int? Level;
        public CharacterClass Class;
        public Gender Gender;
        public Race Race;
    }
}
