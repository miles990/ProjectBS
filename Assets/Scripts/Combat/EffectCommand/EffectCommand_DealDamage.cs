using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBS.Combat.EffectCommand
{
    public class EffectCommand_DealDamage : EffectCommandBase
    {
        public override void Process(string[] vars, Action onCompleted)
        {
            string[] _targetSelection = vars[0].Replace(")", "").Split('(');

            Debug.Log(_targetSelection[0]);

            // UI.EnableSelectSkillUI(Action<SkillData> OnSelected)
            // UI.EnableSelectCharacter(int count, Action<List<Character>> OnSelected, bool random = false)

            switch (_targetSelection[0])
            {
                case "Self":
                    break;
                case "SelectSameSide":
                    break;
                case "SelectOpponent":
                    break;
                case "SelectAllSide":
                    break;
                case "AllSameSide":
                    break;
                case "AllOpponent":
                    break;
                case "AllBattleField":
                    break;
                case "RandomAll":
                    break;
                case "RandomSameSide":
                    break;
                case "RandomOpponent":
                    break;
            }
        }
    }
}

