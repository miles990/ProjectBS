using System;
using UnityEngine;
using TMPro;

namespace ProjectBS.UI
{
    public class CombatUI_SelectSkillButton : CombatUI_ButtonBase
    {
        public event Action<int> OnShownDetailCommanded = null;

        [SerializeField] private TextMeshProUGUI m_skillText = null;
        [SerializeField] private TextMeshProUGUI m_spText = null;

        public void SetUp(string skillName, int sp)
        {
            m_skillText.text = skillName;
            m_spText.text = "SP: " + sp;
        }

        protected override void OnNeedToShowDetail()
        {
            OnShownDetailCommanded?.Invoke(Index);
        }
    }
}
