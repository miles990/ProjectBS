using KahaGameCore.Interface;

namespace ProjectBS.Data
{
    public class RawEquipmentData : IGameData
    {
        public int ID { get; private set; }
        public int NameContextID { get; private set; }
        public string AbilityPool { get; private set; }
        public string BuffIDs { get; private set; }
        public string EquipmentType { get; private set; }
        public int DescriptionContextID { get; private set; }
        public int StoryContextID { get; private set; }
    }
}