using KahaGameCore.Interface;

namespace ProjectBS.Data
{
    public class EquipmentUpgradeData : IGameData
    {
        public int ID { get; private set; }
        public int RequireItemID { get; private set; }
        public int RequireItemAmount { get; private set; }
    }
}