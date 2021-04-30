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
            OnActionStarted_Other,
            OnActionStarted_Self,
            OnStartToAttack_Other,
            OnStartToAttack_Self,
            OnDamageCalculated_Other,
            OnDamageCalculated_Self,
            OnStartToTakeDamage_Other,
            OnStartToTakeDamage_Self,
            OnDamageTaken_Other,
            OnDamageTaken_Self,
            OnDamageDealed_Other,
            OnDamageDealed_Self,
            OnBeAttackedEnded_Self,
            OnBeAttackedEnded_Other,
            OnAttackEnded_Other,
            OnAttackEnded_Self,
            OnDied_Other,
            OnDied_Self,
            OnStartToEndAction_Other,
            OnStartToEndAction_Self,
            OnStartToEndTurn,
            OnBattleEnded,
            OnActived,
            OnDeactived,
            AI
        }

        public class ProcessData
        {
            public class ReferenceSkillInfo
            {
                public CombatUnit owner;
                public Data.SkillData skill;
            }
            public TriggerTiming timing = TriggerTiming.OnActived;
            public CombatUnit caster = null;
            public CombatUnit target = null;
            public AllCombatUnitAllEffectProcesser allEffectProcesser = null;
            public CombatUnit.Buff referenceBuff = null;
            public ReferenceSkillInfo refenceSkill = null;
            public int skipIfCount = 0;
            public Action onEnded = null;
        }

        private class EffectData : IProcessable
        {
            public EffectCommandBase command;
            public string[] vars;

            public void Process(Action onCompleted)
            {
                if (command.processData.skipIfCount > 0 
                    && !EffectProcessManager.IsIfCommand(command.GetType()))
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
            if (processData == null)
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
                    && _deserializeBuffer.Trim() == timing.Trim())
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
                    _newData.command = EffectProcessManager.GetEffectCommand(_deserializeBuffer);
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
    }
}
