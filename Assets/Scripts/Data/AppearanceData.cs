using KahaGameCore.Interface;

namespace ProjectBS.Data
{
    public class AppearanceData : IGameData
    {
        public int ID { get; private set; }
        public string SpriteAssetPath { get; private set; }
        public int NameContextID { get; private set; }
        public string DefaultSkillSet { get; private set; }
        public int IsDrop { get; private set; }
    }
}

