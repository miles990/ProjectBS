using KahaGameCore.Interface;

namespace ProjectBS.Data
{
    public class SkillData : IGameData
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Command { get; private set; }
        public int SkillScrollItemID { get; private set; }
        public int SkillScrollItemRequireAmount { get; private set; }
    }
}