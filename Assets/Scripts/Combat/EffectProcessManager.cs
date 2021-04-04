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
            typeof(EffectCommand_BeginIf_Buff),
            typeof(EffectCommand_BeginIf_Skill),
            typeof(EffectCommand_BeginIf_HasBuffTag),
            typeof(EffectCommand_BeginIf_LastSkillTag)
        };

        private static Dictionary<int, EffectProcesser> m_buffIDToProcesser = new Dictionary<int, EffectProcesser>();
        private static Dictionary<int, EffectProcesser> m_skillIDToProcesser = new Dictionary<int, EffectProcesser>();

        public static EffectProcesser GetBuffProcesser(int id)
        {
            if(!m_buffIDToProcesser.ContainsKey(id))
            {
                m_buffIDToProcesser.Add(id, new EffectProcesser(GameDataManager.GetGameData<Data.BuffData>(id).Command));
            }

            return m_buffIDToProcesser[id];
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
                case "AddBuffAmount":
                    {
                        return new EffectCommand_AddBuffAmount();
                    }
                case "AddBuffAmountByTag":
                    {
                        return new EffectCommand_AddBuffAmountByTag();
                    }
                case "AddBuffTime":
                    {
                        return new EffectCommand_AddBuffTime();
                    }
                case "AddBuffTimeByTag":
                    {
                        return new EffectCommand_AddBuffTimeByTag();
                    }
                case "BeginIf":
                    {
                        return new EffectCommand_BeginIf();
                    }
                case "BeginIf_Buff":
                    {
                        return new EffectCommand_BeginIf_Buff();
                    }
                case "BeginIf_Skill":
                    {
                        return new EffectCommand_BeginIf_Skill();
                    }
                case "BeginIf_LastSkillTag":
                    {
                        return new EffectCommand_BeginIf_LastSkillTag();
                    }
                case "EffectCommand_BeginIf_HasBuffTag":
                    {
                        return new EffectCommand_BeginIf_HasBuffTag();
                    }
                case "EndIf":
                    {
                        return new EffectCommand_EndIf();
                    }
                case "AddShield":
                    {
                        return new EffectCommand_AddShield();
                    }
                case "Chain":
                    {
                        return new EffectCommand_Chain();
                    }
                case "ReplaceSkill":
                    {
                        return new EffectCommand_ReplaceSkill();
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
                case "SetSkipCheckSP":
                    {
                        return new EffectCommand_SetSkipCheckSP();
                    }
                case "AddActionIndex":
                    {
                        return new EffectCommand_AddActionIndex();
                    }
                case "AddExtraAction":
                    {
                        return new EffectCommand_AddExtraAction();
                    }
                case "TriggerBuff":
                    {
                        return new EffectCommand_TriggerBuff();
                    }
                default:
                    {
                        throw new System.Exception("[EffectProcesser][GetEffectCommand] Invaild command=" + command);
                    }
            }
        }
    }
}
