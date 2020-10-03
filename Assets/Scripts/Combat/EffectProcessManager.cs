using System.Collections.Generic;
using KahaGameCore.Static;
using ProjectBS.Combat.EffectCommand;

namespace ProjectBS.Combat
{
    public static class EffectProcessManager
    {
        private static readonly List<System.Type> m_ifCommands = new List<System.Type>
        {
            typeof(EffectCommand_EndIf),
            typeof(EffectCommand_BeginIf),
            typeof(EffectCommand_BeginIf_Effect),
            typeof(EffectCommand_BeginIf_Skill)
        };

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

        public static bool IsIfCommand(System.Type type)
        {
            return m_ifCommands.Contains(type);
        }

        public static EffectCommandBase GetEffectCommand(string command)
        {
            switch (command.Trim())
            {
                case "SetStatus":
                    {
                        return new EffectCommand_SetStatus();
                    }
                case "AddStatus":
                    {
                        return new EffectCommand_AddStatus();
                    }
                case "DealDamage":
                    {
                        return new EffectCommand_DealDamage();
                    }
                case "AddDamage":
                    {
                        return new EffectCommand_AddDamage();
                    }
                case "SetDamage":
                    {
                        return new EffectCommand_SetDamage();
                    }
                case "SetForceEndAction":
                    {
                        return new EffectCommand_SetForceEndAction();
                    }
                case "GainBuff":
                    {
                        return new EffectCommand_GainBuff();
                    }
                case "RemoveBuff":
                    {
                        return new EffectCommand_RemoveBuff();
                    }
                case "AddBuffTime":
                    {
                        return null;
                    }
                case "BeginIf":
                    {
                        return new EffectCommand_BeginIf();
                    }
                case "BeginIf_Effect":
                    {
                        return new EffectCommand_BeginIf_Effect();
                    }
                case "BeginIf_Skill":
                    {
                        return new EffectCommand_BeginIf_Skill();
                    }
                case "EndIf":
                    {
                        return new EffectCommand_EndIf();
                    }
                case "StoreDamage":
                    {
                        return null;
                    }
                case "Chain":
                    {
                        return null;
                    }
                case "ReplaceSkill":
                    {
                        return null;
                    }
                case "CastSkill":
                    {
                        return new EffectCommand_CastSkill();
                    }
                case "RandomCastSkill":
                    {
                        return new EffectCommand_RandomCastSkill();
                    }
                case "Quit":
                    {
                        return new EffectCommand_Quit();
                    }
                case "LockAddStatus":
                    {
                        return new EffectCommand_LockAddStatus();
                    }
                case "ForceDie":
                    {
                        return new EffectCommand_ForceDie();
                    }
                case "Destroy":
                    {
                        return new EffectCommand_Destroy();
                    }
                default:
                    {
                        throw new System.Exception("[EffectProcesser][GetEffectCommand] Invaild command=" + command);
                    }
            }
        }
    }
}
