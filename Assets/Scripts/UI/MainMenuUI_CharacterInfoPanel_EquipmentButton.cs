using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProjectBS.UI
{
    public class MainMenuUI_CharacterInfoPanel_EquipmentButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        private const string ABILITY_FORMAT = "HP {0}\nAttack {1}\nDefense {2}\nSpeed {3}";

        [SerializeField] private Text m_nameText = null;

        private Data.OwningEquipmentData m_referenceEquipment = null;
        private float m_showInfoTimer = 0f;

        public void SetUp(Data.OwningEquipmentData data)
        {
            m_referenceEquipment = data;
            if (m_referenceEquipment == null)
                m_nameText.text = ContextConverter.Instance.GetContext(0);
            else
                m_nameText.text = ContextConverter.Instance.GetContext(m_referenceEquipment.GetSourceData().NameContextID);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_showInfoTimer = GameDataLoader.GameProperties.PressDownShowInfoTime;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
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

