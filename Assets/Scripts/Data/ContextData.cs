using KahaGameCore.Interface;

namespace ProjectBS.Data
{
    public class ContextData : IGameData
    {
        public int ID { get; private set; }
        public string zh_tw { get; private set; }
        public string en_us { get; private set; }
        public string ja_jp { get; private set; }
    }
}
