using UnityEngine;
using TMPro;

namespace ProjectBS.UI
{
    public class CombatUI_InfoText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_infoText = null;

        public void SetText(string info)
        {
            m_infoText.text = info;
        }
    }
}
