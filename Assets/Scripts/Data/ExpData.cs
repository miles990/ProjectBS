using KahaGameCore.Interface;

namespace ProjectBS.Data
{
    public class ExpData : IGameData
    {
        public int ID { get; private set; }
        public int Require { get; private set; }
        public int Owning { get; private set; }
    }
}

