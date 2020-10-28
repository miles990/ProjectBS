using KahaGameCore.Static;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProjectBS.UI
{
    public class MainMenuUI_ChangeWeaponPanel_EquipmentButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        private const string ABILITY_FORMAT = "HP {0}\nAttack {1}\nDefense {2}\nSpeed {3}";

        public event Action<Data.OwningEquipmentData> OnSelected = null;

        [SerializeField] private Text m_infoText = null;
        [SerializeField] private Image m_buttonImage = null;
        [SerializeField] private Color m_enableColor = Color.white;
        [SerializeField] private Color m_disableColor = Color.gray;

        private Data.OwningEquipmentData m_referenceEquipment = null;
        private float m_showInfoTimer = 0f;
        private bool m_isEnable = false;

        public void SetUp(Data.OwningEquipmentData data)
        {
            m_referenceEquipment = data;
            bool _isEquiped = PlayerManager.Instance.GetEquipedCharacter(m_referenceEquipment.UDID) != null;
            m_infoText.text = string.Format("{0}\nHP{1}\nAttack{2}\nDefense{3}\nSpeed{4}",
                                         ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.RawEquipmentData>(m_referenceEquipment.EquipmentSourceID).NameContextID) + (_isEquiped ? " (E)" : ""),
                                         m_referenceEquipment.HP >= 0 ? "+" + m_referenceEquipment.HP : m_referenceEquipment.HP.ToString(),
                                         m_referenceEquipment.Attack >= 0 ? "+" + m_referenceEquipment.Attack : m_referenceEquipment.Attack.ToString(),
                                         m_referenceEquipment.Defense >= 0 ? "+" + m_referenceEquipment.Defense : m_referenceEquipment.Defense.ToString(),
                                         m_referenceEquipment.Speed >= 0 ? "+" + m_referenceEquipment.Speed : m_referenceEquipment.Speed.ToString());

        }

        public void EnableButton(bool enable)
        {
            m_buttonImage.color = enable ? m_enableColor : m_disableColor;
            m_isEnable = enable;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_showInfoTimer = GameDataManager.GameProperties.PressDownShowInfoTime;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (m_showInfoTimer > 0f)
            {
                if (m_isEnable) OnSelected?.Invoke(m_referenceEquipment);
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
                    string _name = ContextConverter.Instance.GetContext(m_referenceEquipment.GetSourceData().NameContextID)
                    + " Level " + m_referenceEquipment.Level
                    + " <size=30>Exp " + m_referenceEquipment.Exp + "/" + m_referenceEquipment.GetRequireExp() + "</size>";
                    string _des = string.Format(ABILITY_FORMAT,
                                               m_referenceEquipment.HP >= 0 ? "+" + m_referenceEquipment.HP : m_referenceEquipment.HP.ToString(),
                                               m_referenceEquipment.Attack >= 0 ? "+" + m_referenceEquipment.Attack : m_referenceEquipment.Attack.ToString(),
                                               m_referenceEquipment.Defense >= 0 ? "+" + m_referenceEquipment.Defense : m_referenceEquipment.Defense.ToString(),
                                               m_referenceEquipment.Speed >= 0 ? "+" + m_referenceEquipment.Speed : m_referenceEquipment.Speed.ToString());

                    GameManager.Instance.MessageManager.ShowCommonMessage(
                        _des + "\n\n" + ContextConverter.Instance.GetContext(m_referenceEquipment.GetSourceData().DescriptionContextID),
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
