using UnityEngine;

namespace ProjectBS.UI
{
    public class CombatUI_InfoText : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Text m_infoText = null;

        public void SetText(string info)
        {
            m_infoText.text = info;
        }
    }
}
