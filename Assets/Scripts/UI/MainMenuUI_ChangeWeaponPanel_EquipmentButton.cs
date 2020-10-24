using KahaGameCore.Static;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProjectBS.UI
{
    public class MainMenuUI_ChangeWeaponPanel_EquipmentButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public event Action<Data.OwningEquipmentData> OnSelected = null;
        public event Action<Data.OwningEquipmentData> OnShownDetailCommanded = null;

        [SerializeField] private Text m_infoText = null;
        [SerializeField] private Image m_buttonImage = null;
        [SerializeField] private Color m_enableColor = Color.white;
        [SerializeField] private Color m_disableColor = Color.gray;

        private Data.OwningEquipmentData m_refEquipment = null;
        private float m_showInfoTimer = 0f;
        private bool m_isEnable = false;

        public void SetUp(Data.OwningEquipmentData data)
        {
            m_refEquipment = data;
            bool _isEquiped = PlayerManager.Instance.GetEquipedCharacter(m_refEquipment.UDID) != null;
            m_infoText.text = string.Format("{0}\nHP{1}\nAttack{2}\nDefense{3}\nSpeed{4}",
                                         ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.RawEquipmentData>(m_refEquipment.EquipmentSourceID).NameContextID) + (_isEquiped ? " (E)" : ""),
                                         m_refEquipment.HP >= 0 ? "+" + m_refEquipment.HP : m_refEquipment.HP.ToString(),
                                         m_refEquipment.Attack >= 0 ? "+" + m_refEquipment.Attack : m_refEquipment.Attack.ToString(),
                                         m_refEquipment.Defense >= 0 ? "+" + m_refEquipment.Defense : m_refEquipment.Defense.ToString(),
                                         m_refEquipment.Speed >= 0 ? "+" + m_refEquipment.Speed : m_refEquipment.Speed.ToString());

        }

        public void EnableButton(bool enable)
        {
            m_buttonImage.color = enable ? m_enableColor : m_disableColor;
            m_isEnable = enable;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_showInfoTimer = GameDataLoader.GameProperties.PressDownShowInfoTime;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (m_showInfoTimer > 0f)
            {
                if (m_isEnable) OnSelected?.Invoke(m_refEquipment);
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
                    m_showInfoTimer = 0f;
                    OnShownDetailCommanded?.Invoke(m_refEquipment);
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_showInfoTimer = 0f;
        }

    }
}
