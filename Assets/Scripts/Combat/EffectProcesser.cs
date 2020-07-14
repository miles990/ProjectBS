using System;
using ProjectBS.Combat.EffectCommand;
using System.Collections.Generic;
using KahaGameCore.Interface;
using KahaGameCore.Common;

namespace ProjectBS.Combat
{
    public class EffectProcesser
    {
        public enum TriggerTiming
        {
            OnBattleStarted,
            OnTurnStarted,
            OnActionStarted,
            OStartToAttack,
            OnStartToDealDamage,
            OnStartToTakeDamage,
            OnDamageTaken,
            OnAttacked,
            OnDied,
            OnStartToEndAction,
            OnBattleEnded,
            OnBuffGained,
            OnStartToLoseBuff,
            OnStatusValueChanged,
            OnActived
        }

        private string m_rawData = "";
        private CombatManager.CombatUnit m_caster = null;

        private class EffectData : IProcessable
        {
            public EffectCommandBase command;
            public string[] vars;

            public void Process(Action onCompleted)
            {
                command.Process(vars, onCompleted);
            }
        }

        public EffectProcesser(string rawData)
        {
            m_rawData = rawData;
        }

        public void Start(CombatManager.CombatUnit caster, TriggerTiming timing, Action onEnded)
        {
            m_caster = caster;
            string _commandRawData = DeserializeCommandRawDatas(timing);
            if(_commandRawData == null)
            {
                onEnded?.Invoke();
                return;
            }

            string[] _commandLines = _commandRawData.Split(';');
            List<EffectData> _effectDatas = new List<EffectData>();
            for(int i = 0; i < _commandLines.Length; i++)
            {
                EffectData _effectData = DeserializeCommands(_commandLines[i]);
                if (_effectData != null)
                {
                    _effectDatas.Add(_effectData);
                }
            }

            new Processer<EffectData>(_effectDatas.ToArray()).Start(onEnded);
        }

        private string DeserializeCommandRawDatas(TriggerTiming timing)
        {
            string _deserializeBuffer = "";
            bool _startRecordCommands = false;

            for (int i = 0; i < m_rawData.Length; i++)
            {
                if (_startRecordCommands)
                {
                    if (m_rawData[i] == '}')
                    {
                        return _deserializeBuffer;
                    }
                    _deserializeBuffer += m_rawData[i];
                    continue;
                }

                if (m_rawData[i] == '{'
                    && _deserializeBuffer == timing.ToString())
                {
                    _startRecordCommands = true;
                    _deserializeBuffer = "";
                    continue;
                }

                if (m_rawData[i] == '}')
                {
                    _deserializeBuffer = "";
                }
                else
                {
                    _deserializeBuffer += m_rawData[i];
                }
            }

            return null;
        }

        private EffectData DeserializeCommands(string commandData)
        {
            string _deserializeBuffer = "";
            int _leftCounter = 0;
            EffectData _newData = new EffectData();

            for(int i = 0; i < commandData.Length; i++)
            {
                if(_leftCounter > 0)
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
                    if(commandData[i] == '(')
                    {
                        _leftCounter++;
                    }
                    continue;
                }

                if(commandData[i] == '(')
                {
                    _newData.command = GetEffectCommand(_deserializeBuffer);
                    if(_newData.command != null)
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
            switch(command)
            {
                case "SetStatus":
                    {
                        return new EffectCommand_SetStatus(m_caster);
                    }
                case "AddStatus":
                    {
                        break;
                    }
                case "AddDamage":
                    {
                        break;
                    }
                case "ForceEnd":
                    {
                        break;
                    }
                case "GainBuff":
                    {
                        break;
                    }
                case "RemoveBuff":
                    {
                        break;
                    }
                case "AddBuffTime":
                    {
                        break;
                    }
                case "BeginIf":
                    {
                        break;
                    }
                case "EndIf":
                    {
                        break;
                    }
                case "StoreDamage":
                    {
                        break;
                    }
                case "Chain":
                    {
                        break;
                    }
                default:
                    {
                        return null;
                    }
            }
        }
    }
}
