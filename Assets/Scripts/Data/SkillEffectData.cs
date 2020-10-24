using KahaGameCore.Interface;

namespace ProjectBS.Data
{
    public class SkillEffectData : IGameData
    {
        public int ID { get; private set; }
        public int NameContextID { get; private set; }
        public int DescriptionContextID { get; private set; }
        public string Command { get; private set; }
        public int Tag { get; private set; }
        public int MaxStackCount { get; private set; }
    }
}