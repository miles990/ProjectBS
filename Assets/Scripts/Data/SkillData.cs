using KahaGameCore.Interface;

namespace ProjectBS.Data
{
    public class SkillData : IGameData
    {
        public int ID { get; private set; }
        public int NameContextID { get; private set; }
        public int DescriptionContextID { get; private set; }
        public string Command { get; private set; }
        public int SkillScrollItemID { get; private set; }
        public int SkillScrollItemRequireAmount { get; private set; }
    }
}