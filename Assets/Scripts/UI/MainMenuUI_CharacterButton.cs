using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectBS.UI
{
    public class MainMenuUI_CharacterButton : MonoBehaviour
    {
        [SerializeField] private Text m_nameAndLevelText = null;
        [SerializeField] private Text m_abilityText = null;
        [SerializeField] private Text m_equipmentText = null;
        [SerializeField] private Text m_skillText = null;
        [SerializeField] private GameObject m_partyHintRoot = null;
        [SerializeField] private Text m_partyHintText = null;
    }
}
