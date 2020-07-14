using KahaGameCore.Interface;

namespace ProjectBS.Data
{
    public class AbilityData : IGameData
    {
        public int ID { get; private set; }
        public string Type { get; private set; }
        public string RankString { get; private set; }
        public int MinValue { get; private set; }
        public int MaxValue { get; private set; }
    }
}

