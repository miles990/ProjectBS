﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectBS.UI
{
    public class MainMenuUI_EquipmentButton : MonoBehaviour
    {
        public event System.Action OnEdited = null;

        [SerializeField] private TextMeshProUGUI m_levelText = null;
        [SerializeField] private TextMeshProUGUI m_nameText = null;
        [SerializeField] private TextMeshProUGUI m_hpText = null;
        [SerializeField] private TextMeshProUGUI m_attackText = null;
        [SerializeField] private TextMeshProUGUI m_defenseText = null;
        [SerializeField] private TextMeshProUGUI m_speedText = null;
        [SerializeField] private TextMeshProUGUI m_effectText = null;
        [SerializeField] private Toggle m_lockToggle = null;
        [SerializeField] private Button m_departButton = null;
        [SerializeField] private RawImage m_iconImage = null;
        [SerializeField] private Texture m_headIconSprite = null;
        [SerializeField] private Texture m_handIconSprite = null;
        [SerializeField] private Texture m_bodyIconSprite = null;
        [SerializeField] private Texture m_footIconSprite = null;
        [SerializeField] private GameObject m_lockUIRoot = null;
        [SerializeField] private GameObject m_unlockUIRoot = null;

        private Data.OwningEquipmentData m_refEquipment = null;
        private bool m_initing = false;

        public void SetUp(Data.OwningEquipmentData equipmentData)
        {
            m_initing = true;

            m_refEquipment = equipmentData;
            Data.RawEquipmentData _source = equipmentData.GetSourceData();

            m_levelText.text = "Lv. " + m_refEquipment.Level;
            m_nameText.text = ContextConverter.Instance.GetContext(_source.NameContextID);
            m_hpText.text = m_refEquipment.HP.ToString();
            m_attackText.text = m_refEquipment.Attack.ToString();
            m_defenseText.text = m_refEquipment.Defense.ToString();
            m_speedText.text = m_refEquipment.Speed.ToString();
            m_effectText.text = ContextConverter.Instance.GetContext(_source.DescriptionContextID);

            switch(_source.EquipmentType)
            {
                case Keyword.Body:
                    m_iconImage.texture = m_bodyIconSprite;
                    break;
                case Keyword.Foot:
                    m_iconImage.texture = m_footIconSprite;
                    break;
                case Keyword.Hand:
                    m_iconImage.texture = m_handIconSprite;
                    break;
                case Keyword.Head:
                    m_iconImage.texture = m_headIconSprite;
                    break;
            }

            UpdateToogleUI();

            m_initing = false;
        }

        public void Button_Depart()
        {
            EquipmentUtility.Depart(m_refEquipment.UDID);
            PlayerManager.Instance.SavePlayer();

            OnEdited?.Invoke();
        }

        public void Button_OnToggleValueChanged()
        {
            if (m_initing)
                return;

            if(m_refEquipment != null)
            {
                EquipmentUtility.Lock(m_refEquipment.UDID, m_lockToggle.isOn);
                PlayerManager.Instance.SavePlayer();
                UpdateToogleUI();
            }
        }

        private void UpdateToogleUI()
        {
            m_lockToggle.isOn = PlayerManager.Instance.Player.LockedEquipmentUDIDs.Contains(m_refEquipment.UDID);
            m_departButton.gameObject.SetActive(!m_lockToggle.isOn);
            m_lockUIRoot.gameObject.SetActive(m_lockToggle.isOn);
            m_unlockUIRoot.gameObject.SetActive(!m_lockToggle.isOn);
        }
    }
}
