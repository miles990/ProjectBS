using KahaGameCore.Interface;

namespace ProjectBS.Data
{
    public class RawEquipmentData : IGameData
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string AbilityIDs { get; private set; }
        public string SkillEffectIDs { get; private set; }
        public int MinSkillEffectCount { get; private set; }
        public int MaxSkillEffectCount { get; private set; }
        public string EquipmentType { get; private set; }
        public int DescriptionContextID { get; private set; }
        public string EquipmentUpgradeItemIDs { get; private set; }
    }
}