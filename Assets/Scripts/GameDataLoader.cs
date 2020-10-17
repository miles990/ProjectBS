using KahaGameCore.Static;
using ProjectBS.Data;

namespace ProjectBS
{
    public static class GameDataLoader
    {
        public static void StartLoad() 
        {
            GameDataManager.LoadGameData<ContextData>("ContextData");
            GameDataManager.LoadGameData<SkillData>("SkillData");
            GameDataManager.LoadGameData<AbilityData>("AbilityData");
            GameDataManager.LoadGameData<SkillEffectData>("SkillEffectData");
            GameDataManager.LoadGameData<BossData>("BossData");
            GameDataManager.LoadGameData<ExpData>("ExpData");
            GameDataManager.LoadGameData<CharacterNamePoolData>("CharacterNamePoolData");
            GameDataManager.LoadGameData<RandomSkillData>("RandomSkillData");
            GameDataManager.LoadGameData<RawEquipmentData>("RawEquipmentData");
            GameDataManager.LoadGameData<BossStageData>("BossStageData");
        }
    }
}
