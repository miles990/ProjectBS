using TMPro;
using UnityEngine;

namespace ProjectBS
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class Localizer : MonoBehaviour
    {
        [SerializeField] private int m_id = 0;

        private TextMeshProUGUI m_text = null;

        private void Awake()
        {
            m_text = GetComponent<TextMeshProUGUI>();
            m_text.text = ContextConverter.Instance.GetContext(m_id);
        }
    }
}

