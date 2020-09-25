using System.Collections.Generic;
using KahaGameCore.Static;

namespace ProjectBS.Combat
{
    public static class EffectProcessManager
    {
        private static Dictionary<int, EffectProcesser> m_effectIDToProcesser = new Dictionary<int, EffectProcesser>();
        private static Dictionary<int, EffectProcesser> m_skillIDToProcesser = new Dictionary<int, EffectProcesser>();

        public static EffectProcesser GetSkillEffectProcesser(int id)
        {
            if(!m_effectIDToProcesser.ContainsKey(id))
            {
                m_effectIDToProcesser.Add(id, new EffectProcesser(GameDataManager.GetGameData<Data.SkillEffectData>(id).Command));
            }

            return m_effectIDToProcesser[id];
        }

        public static EffectProcesser GetSkillProcesser(int id)
        {
            if (!m_skillIDToProcesser.ContainsKey(id))
            {
                m_skillIDToProcesser.Add(id, new EffectProcesser(GameDataManager.GetGameData<Data.SkillData>(id).Command));
            }

            return m_skillIDToProcesser[id];
        }
    }
}
