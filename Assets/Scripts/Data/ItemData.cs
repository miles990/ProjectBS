using KahaGameCore.Interface;

namespace ProjectBS.Data
{
    public class ItemData : IGameData
    {
        public int ID { get; private set; }
        public int NameContextID { get; private set; }
        public int DescriptionContextID { get; private set; }
        public int Exp { get; private set; }
    }
}