using KahaGameCore.Interface;

namespace ProjectBS.Data
{
    public class BossStageData : IGameData
    {
        public int ID { get; private set; } 
        public int Index { get; private set; } 
        public int MainBossID { get; private set; }
        public int Stamina { get; private set; } 
        public string BossIDs { get; private set; } 
        public int DescriptionContextID { get; private set; }
        public string DropData { get; private set; }
        public int MinExp { get; private set; }
        public int MaxExp { get; private set; }
    }
}
