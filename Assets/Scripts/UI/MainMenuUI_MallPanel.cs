using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBS.UI
{
    public class MainMenuUI_MallPanel : MainMenuUI_PanelBase
    {
        protected override void OnHidden()
        {
        }

        protected override void OnShown()
        {
        }

        public void AddNewCharacter(int count)
        {
            for(int i = 0; i < count; i++)
            {
                PlayerManager.Instance.Player.Characters.Add(CharacterUtility.CreateNewCharacter());
            }
        }
    }
}
