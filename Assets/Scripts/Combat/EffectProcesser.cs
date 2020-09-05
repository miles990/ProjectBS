using KahaGameCore.Interface;
using KahaGameCore.Common;
using System;
using System.Collections.Generic;
using ProjectBS.Combat.EffectCommand;
using KahaGameCore.Static;

namespace ProjectBS.Combat
{
    public class EffectProcesser
    {
        public enum TriggerTiming
        {
            OnBattleStarted,
            OnTurnStarted,
            OnActionStarted_Any,
            OnActionStarted_Self,
            OnStartToAttack_Any,
            OnStartToAttack_Self,
            OnDamageCalculated_Any,
            OnDamageCalculated_Self,
            OnStartToTakeDamage_Any,
            OnStartToTakeDamage_Self,
            OnDamageTaken_Any,
            OnDamageTaken_Self,
            OnAttacked_Any,
            OnAttacked_Self,
            OnDied_Any,
            OnDied_Self,
            OnStartToEndAction_Any,
            OnStartToEndAction_Self,
            OnStartToEndTurn,
            OnBattleEnded,
            OnActived,
            OnDeactived,
            AI
        }

        public class ProcessData
        {
            public TriggerTiming timing = TriggerTiming.OnActived;
            public CombatUnit caster = null;
            public CombatUnit target = null;
            public AllCombatUnitAllEffectProcesser allEffectProcesser = null;
            public CombatUnit.Buff referenceBuff = null;
            public Data.SkillData refenceSkill = null;
            public int skipIfCount = 0;
            public Action onEnded = null;
        }

        private class EffectData : IProcessable
        {
            public EffectCommandBase command;
            public string[] vars;

            public void Process(Action onCompleted)
            {
                UnityEngine.Debug.LogWarning("EffectData command=" + command + ", command.processData.skipIfCount=" + command.processData.skipIfCount);
                if (command.processData.skipIfCount > 0
                    && !(command is EffectCommand_EndIf)
                    && !(command is EffectCommand_BeginIf)
                    && !(command is EffectCommand_BeginIf_Effect)
                    && !(command is EffectCommand_BeginIf_Skill))
                {
                    onCompleted?.Invoke();
                }
                else
                {
                    command.Process(vars, onCompleted);
                }
            }
        }

        private readonly Dictionary<string, List<EffectData>> m_timingToEffectProcesser = new Dictionary<string, List<EffectData>>();

        public EffectProcesser(string rawCommandString)
        {
            if (string.IsNullOrEmpty(rawCommandString))
                return;

            rawCommandString = rawCommandString.RemoveBlankCharacters();

            string[] _timings = Enum.GetNames(typeof(TriggerTiming));
            for (int _timingStringIndex = 0; _timingStringIndex < _timings.Length; _timingStringIndex++)
            {
                string _rawCommandString = DeserializeCommandRawDatas(_timings[_timingStringIndex], rawCommandString);

                if (_rawCommandString == null)
                    continue;
                
                string[] _commandStrings = _rawCommandString.Split(';');
                List<EffectData> _effects = new List<EffectData>();
                for(int _commandStringIndex = 0; _commandStringIndex < _commandStrings.Length; _commandStringIndex++)
                {
                    EffectData _effectData = DeserializeCommands(_commandStrings[_commandStringIndex]);
                    if(_effectData == null)
                    {
                        continue;
                    }

                    _effects.Add(_effectData);
                }
                m_timingToEffectProcesser.Add(_timings[_timingStringIndex], _effects);
            }
        }

        public void Start(ProcessData processData)
        {
            if(processData == null)
            {
                return;
            }
            if(!m_timingToEffectProcesser.ContainsKey(processData.timing.ToString()))
            {
                processData.onEnded?.Invoke();
            }
            else
            {
                List<EffectData> _effects = m_timingToEffectProcesser[processData.timing.ToString()];
                for (int i = 0; i < _effects.Count; i++)
                {
                    _effects[i].command.processData = processData;
                }

                processData.skipIfCount = 0;
                new Processer<EffectData>(_effects.ToArray()).Start(processData.onEnded);
            }
        }

        private string DeserializeCommandRawDatas(string timing, string rawData)
        {
            string _deserializeBuffer = "";
            bool _startRecordCommands = false;

            for (int i = 0; i < rawData.Length; i++)
            {
                if (_startRecordCommands)
                {
                    if (rawData[i] == '}')
                    {
                        return _deserializeBuffer;
                    }
                    _deserializeBuffer += rawData[i];
                    continue;
                }

                if (rawData[i] == '{'
                    && _deserializeBuffer == timing)
                {
                    _startRecordCommands = true;
                    _deserializeBuffer = "";
                    continue;
                }

                if (rawData[i] == '}')
                {
                    _deserializeBuffer = "";
                }
                else
                {
                    _deserializeBuffer += rawData[i];
                }
            }

            return null;
        }

        private EffectData DeserializeCommands(string commandData)
        {
            string _deserializeBuffer = "";
            int _leftCounter = 0;
            EffectData _newData = new EffectData();
            for (int i = 0; i < commandData.Length; i++)
            {
                if (_leftCounter > 0)
                {
                    if (commandData[i] == ')')
                    {
                        _leftCounter--;
                        if (_leftCounter <= 0)
                        {
                            int _varLeftCounter = 0;
                            string _varBuffer = "";
                            List<string> _varsTempList = new List<string>();
                            for(int j = 0; j < _deserializeBuffer.Length; j++)
                            {
                                if (_deserializeBuffer[j] == ',' && _varLeftCounter == 0)
                                {
                                    _varsTempList.Add(_varBuffer);
                                    _varBuffer = "";
                                    continue;
                                }
                                _varBuffer += _deserializeBuffer[j];
                                if(_deserializeBuffer[j] == ')')
                                {
                                    _varLeftCounter--;
                                }
                                if (_deserializeBuffer[j] == '(')
                                {
                                    _varLeftCounter++;
                                }
                                if(j == _deserializeBuffer.Length - 1)
                                {
                                    _varsTempList.Add(_varBuffer);
                                }
                            }

                            _newData.vars = _varsTempList.ToArray();
                            return _newData;
                        }
                    }

                    _deserializeBuffer += commandData[i];
                    if (commandData[i] == '(')
                    {
                        _leftCounter++;
                    }
                    continue;
                }

                if (commandData[i] == '(')
                {
                    _newData.command = GetEffectCommand(_deserializeBuffer);
                    if (_newData.command != null)
                    {
                        _leftCounter++;
                        _deserializeBuffer = "";
                        continue;
                    }
                    else
                    {
                        return null;
                    }
                }
                _deserializeBuffer += commandData[i];
            }

            return null;
        }

        private EffectCommandBase GetEffectCommand(string command)
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
                case "ForceEndAction":
                    {
                        return new EffectCommand_ForceEndAction();
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
                default:
                    {
                        throw new Exception("[EffectProcesser][GetEffectCommand] Invaild command=" + command);
                    }
            }
        }
    }
}
