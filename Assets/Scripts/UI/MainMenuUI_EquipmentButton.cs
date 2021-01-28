using KahaGameCore.Static;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectBS.UI
{
    public class MainMenuUI_EquipmentButton : MonoBehaviour
    {
        public event System.Action OnEdited = null;

        [SerializeField] private Text m_nameAndLevelText = null;
        [SerializeField] private Text m_abilityText = null;
        [SerializeField] private GameObject m_equipedHint = null;
        [SerializeField] private Toggle m_lockToggle = null;
        [SerializeField] private Button m_departButton = null;

        private Data.OwningEquipmentData m_refEquipment = null;
        private bool m_initing = false;

        public void SetUp(Data.OwningEquipmentData equipmentData)
        {
            m_initing = true;

            m_refEquipment = equipmentData;
            Data.RawEquipmentData _source = equipmentData.GetSourceData();

            m_initing = false;
        }

        public void Button_Depart()
        {
            EquipmentUtility.Depart(m_refEquipment.UDID);
            PlayerManager.Instance.SavePlayer();

            OnEdited?.Invoke();
        }

        public void Button_LevelUp(int addLevel)
        {
            for(int i = 0; i < addLevel; i++)
            {
                EquipmentUtility.TryAddOneLevel(m_refEquipment);
            }
            PlayerManager.Instance.SavePlayer();

            OnEdited?.Invoke();
        }

        public void Button_OnToggleValueChanged()
        {
            if (m_initing)
                return;

            m_departButton.interactable = !m_lockToggle.isOn;
            if(m_refEquipment != null)
            {
                EquipmentUtility.Lock(m_refEquipment.UDID, m_lockToggle.isOn);
                PlayerManager.Instance.SavePlayer();
            }
        }
    }
}
