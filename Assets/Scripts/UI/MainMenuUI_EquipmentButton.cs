using TMPro;
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
