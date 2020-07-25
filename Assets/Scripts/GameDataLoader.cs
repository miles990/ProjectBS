using KahaGameCore.Static;
using ProjectBS.Data;
using UnityEngine;

namespace ProjectBS
{
    public class GameDataLoader
    {
        public static GameDataLoader Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new GameDataLoader();
                }
                return m_instance;
            }
        }
        private static GameDataLoader m_instance = null;

        private GameDataLoader() 
        {
            GameDataManager.LoadGameData<SkillData>("SkillData");
        }

        public string GetCharacterName(int ID)
        {
            return GameDataManager.GetGameData<CharacterNamePoolData>(ID).NameContextID.ToString();
        }

        public Sprite GetSprite(int ID)
        {
            return Resources.Load<Sprite>(GameDataManager.GetGameData<AppearanceData>(ID).SpriteAssetPath);
        }

        public SkillData GetSkill(string ID)
        {
            return GameDataManager.GetGameData<SkillData>(int.Parse(ID));
        }

        public SkillEffectData GetSkillEffect(string ID)
        {
            return GameDataManager.GetGameData<SkillEffectData>(int.Parse(ID));
        }
    }
}
