using KahaGameCore.Interface;

namespace ProjectBS.Data
{
    public class ItemData : IGameData
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int Exp { get; private set; }
    }
}