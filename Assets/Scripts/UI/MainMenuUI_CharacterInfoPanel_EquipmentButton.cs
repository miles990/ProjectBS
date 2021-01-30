using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace ProjectBS.UI
{
    public class MainMenuUI_CharacterInfoPanel_EquipmentButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public event System.Action<MainMenuUI_CharacterInfoPanel_EquipmentButton> OnSelected = null; 

        [SerializeField] private GameObject m_noneContentHintRoot = null;
        [SerializeField] private GameObject m_contentRoot = null;
        [SerializeField] private TextMeshProUGUI m_nameText = null;
        [SerializeField] private TextMeshProUGUI m_hpValueText = null;
        [SerializeField] private TextMeshProUGUI m_attackValueText = null;
        [SerializeField] private TextMeshProUGUI m_defenseValueText = null;
        [SerializeField] private TextMeshProUGUI m_speedValueText = null;

        private Data.OwningEquipmentData m_referenceEquipment = null;
        private float m_showInfoTimer = 0f;

        public void SetUp(Data.OwningEquipmentData data)
        {
            m_referenceEquipment = data;


            RefreshInfo();
        }

        public void RefreshInfo()
        {
            if(m_referenceEquipment == null)
            {
                m_noneContentHintRoot.SetActive(true);
                m_contentRoot.SetActive(false);
                return;
            }
            m_noneContentHintRoot.SetActive(false);
            m_contentRoot.SetActive(true);
            m_nameText.text = ContextConverter.Instance.GetContext(m_referenceEquipment.GetSourceData().NameContextID);
            m_hpValueText.text = m_referenceEquipment.HP.ToString();
            m_attackValueText.text = m_referenceEquipment.Attack.ToString();
            m_defenseValueText.text = m_referenceEquipment.Defense.ToString();
            m_speedValueText.text = m_referenceEquipment.Speed.ToString();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_showInfoTimer = GameDataManager.GameProperties.PressDownShowInfoTime;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(m_showInfoTimer > 0f)
            {
                OnSelected?.Invoke(this);
            }
            m_showInfoTimer = 0f;
        }

        private void Update()
        {
            if (m_showInfoTimer > 0f)
            {
                m_showInfoTimer -= Time.deltaTime;
                if (m_showInfoTimer <= 0f)
                {
                    throw new System.NotImplementedException();
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_showInfoTimer = 0f;
        }
    }
}

