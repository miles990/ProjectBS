using UnityEngine;
using TMPro;

namespace ProjectBS.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UITextConverter : MonoBehaviour
    {
        [SerializeField] private int m_id = 0;

        private TextMeshProUGUI m_text = null;

        private void Awake()
        {
            m_text = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            m_text.text = ContextConverter.Instance.GetContext(m_id);
        }
    }
}
