using KahaGameCore.Interface;
using KahaGameCore.Common;
using System;
using System.Collections.Generic;
using ProjectBS.Combat.EffectCommand;
using UnityEngine;

namespace ProjectBS.Combat
{
    public class EffectProcesser
    {
        public enum TriggerTiming
        {
            OnBattleStarted,
            OnTurnStarted,
            OnActionStarted,
            OnSelfActionStarted,
            OStartToAttack,
            OnStartToDealDamage,
            OnStartToTakeDamage,
            OnDamageTaken,
            OnAttacked,
            OnDied,
            OnStartToEndAction,
            OnStartToEndSelfAction,
            OnStartToEndTurn,
            OnBattleEnded,
            OnBuffGained,
            OnStartToLoseBuff,
            OnStatusValueChanged,
            OnActived
        }

        public class ProcessData
        {
            public TriggerTiming timing = TriggerTiming.OnActived;
            public CombatUnit caster = null;
            public CombatUnit target = null;
            public Action onEnded = null;
        }

        private class EffectData : IProcessable
        {
            public EffectCommandBase command;
            public string[] vars;

            public void Process(Action onCompleted)
            {
                command.Process(vars, onCompleted);
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
                    _effects[i].command.caster = processData.caster;
                    _effects[i].command.target = processData.target;
                }

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
                            _newData.vars = _deserializeBuffer.Split(',');
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
                        return null;
                    }
                case "AddStatus":
                    {
                        return null;
                    }
                case "DealDamage":
                    {
                        return new EffectCommand_DealDamage();
                    }
                case "AddDamage":
                    {
                        return null;
                    }
                case "ForceEnd":
                    {
                        return null;
                    }
                case "GainBuff":
                    {
                        return null;
                    }
                case "RemoveBuff":
                    {
                        return null;
                    }
                case "AddBuffTime":
                    {
                        return null;
                    }
                case "BeginIf":
                    {
                        return null;
                    }
                case "ElseIf":
                    {
                        return null;
                    }
                case "Else":
                    {
                        return null;
                    }
                case "EndIf":
                    {
                        return null;
                    }
                case "StoreDamage":
                    {
                        return null;
                    }
                case "Chain":
                    {
                        return null;
                    }
                default:
                    {
                        return null;
                    }
            }
        }
    }
}
