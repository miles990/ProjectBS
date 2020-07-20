using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBS.Combat
{
    public class EffectCommand_SetStatus : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            CombatUnit _target = vars[0] == "Self" ? caster : target;

            List<string> _buffer = new List<string> { "" };
            int _finalResult = 0;
            for (int i = 0; i < vars[1].Length; i++)
            {
                if(vars[1][i] == '(')
                {
                    _buffer.Add("");
                    continue;
                }

                if(vars[1][i] == ')' || i == vars[1].Length - 1)
                {
                    int _blockResult = 0;

                    // math here......

                    if(_buffer.Count > 1)
                    {
                        _buffer[_buffer.Count - 2] += _blockResult.ToString();
                        _buffer.RemoveAt(_buffer.Count - 1);
                    }
                    else
                    {
                        _finalResult = _blockResult;
                        break;
                    }
                    continue;
                }

                _buffer[_buffer.Count - 1] += vars[1][i];
            }
        }
    }
}
