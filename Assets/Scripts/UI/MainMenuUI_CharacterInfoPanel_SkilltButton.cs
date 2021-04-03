using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectBS.UI
{
    public class MainMenuUI_CharacterInfoPanel_SkilltButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public event System.Action<MainMenuUI_CharacterInfoPanel_SkilltButton> OnSelected = null;

        [SerializeField] private TMPro.TextMeshProUGUI m_nameText = null;
        [SerializeField] private TMPro.TextMeshProUGUI m_spText = null;

        private Data.SkillData m_referenceSkill = null;
        private float m_showInfoTimer = 0f;

        public void SetUp(Data.SkillData data)
        {
            m_referenceSkill = data;
            m_nameText.text = ContextConverter.Instance.GetContext(data.NameContextID);
            if (data.SP > 0)
            {
                m_spText.text = "SP " + data.SP;
                m_spText.gameObject.SetActive(true);
            }
            else
            {
                m_spText.gameObject.SetActive(false);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_showInfoTimer = GameDataManager.GameProperties.PressDownShowInfoTime;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (m_showInfoTimer > 0f) OnSelected.Invoke(this);
            m_showInfoTimer = 0f;
        }

        private void Update()
        {
            if (m_showInfoTimer > 0f)
            {
                m_showInfoTimer -= Time.deltaTime;
                if (m_showInfoTimer <= 0f)
                {
                    m_showInfoTimer = 0f;
                    string _name = ContextConverter.Instance.GetContext(m_referenceSkill.NameContextID);

                    GameManager.Instance.MessageManager.ShowCommonMessage(
                        "SP: " + m_referenceSkill.SP + "\n\n" + m_referenceSkill.GetAllDescriptionContext(),
                        _name, null);
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_showInfoTimer = 0f;
        }
    }
}
