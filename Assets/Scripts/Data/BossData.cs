using KahaGameCore.Interface;

namespace ProjectBS.Data
{
    public class BossData : IGameData
    {
        public int ID { get; private set; }
        public int AppearanceDataID { get; private set; }
        public string RankString { get; private set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }
        public int Speed { get; private set; }
        public int HP { get; private set; }
        public int SP { get; private set; }
        public string SkillIDs { get; private set; }
        public string BuffIDs { get; private set; }
        public string AI { get; private set; }

        public string GetName()
        {
            return ContextConverter.Instance.GetContext(GameDataManager.GetGameData<AppearanceData>(AppearanceDataID).NameContextID);
        }

        public UnityEngine.Texture2D GetIcon()
        {
            return UnityEngine.Resources.Load<UnityEngine.Texture2D>(GameDataManager.GetGameData<AppearanceData>(AppearanceDataID).SpriteAssetPath);
        }
    }
}
