using KahaGameCore.Interface;

namespace ProjectBS.Data
{
    public class BossStageData : IGameData
    {
        public int ID { get; private set; } = 0;
        public int Index { get; private set; } = 0;
        public int MainBossID { get; private set; } = 0;
        public string BossIDs { get; private set; } = "";
        public int DescriptionContextID { get; private set; } = 0;
    }
}
