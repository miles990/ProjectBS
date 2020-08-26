using KahaGameCore.Interface;

namespace ProjectBS.Data
{
    public class RandomSkillData : IGameData
    {
        public int ID { get; private set; }
        public int SkillID1 { get; private set; }
        public int Weight1 { get; private set; }
        public int SkillID2 { get; private set; }
        public int Weight2 { get; private set; }
        public int SkillID3 { get; private set; }
        public int Weight3 { get; private set; }
        public int SkillID4 { get; private set; }
        public int Weight4 { get; private set; }
        public int SkillID5 { get; private set; }
        public int Weight5 { get; private set; }
    }
}
