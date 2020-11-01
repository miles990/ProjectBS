using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_ReplaceSkill : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            int _orginSkillID = int.Parse(vars[0]);
            int _targetSKillID = int.Parse(vars[1]);

            for(int i = 0; i < GetSelf().skills.Length; i++)
            {
                if(GetSelf().skills[i] == _orginSkillID)
                {
                    GetSelf().skills[i] = _targetSKillID;
                    onCompleted?.Invoke();
                    return;
                }
            }

            onCompleted?.Invoke();
        }
    }
}
