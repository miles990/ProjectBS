using KahaGameCore.Interface;

namespace ProjectBS.Data
{
    public class BossData : IGameData
    {
        public int ID { get; private set; }
        public int NameContextID { get; private set; }
        public int CharacterSpriteID { get; private set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }
        public int Speed { get; private set; }
        public int HP { get; private set; }
        public int SP { get; private set; }
        public string EffectIDs { get; private set; }
        public string AI { get; private set; }
        public string DropData { get; private set; }
        public int MinExp { get; private set; }
        public int MaxExp { get; private set; }
    }
}
