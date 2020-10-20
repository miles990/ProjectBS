using KahaGameCore.Static;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectBS.UI
{
    public class MainMenuUI_EquipmentButton : MonoBehaviour
    {
        public event System.Action OnEdited = null;

        private const string ABILITY_FORMAT = "HP {0}\nAttack {1}\nDefense {2}\nSpeed {3}";

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
            Data.RawEquipmentData _source = GameDataManager.GetGameData<Data.RawEquipmentData>(equipmentData.EquipmentSourceID);

            m_nameAndLevelText.text = ContextConverter.Instance.GetContext(_source.NameContextID) 
                + "\nLevel " + equipmentData.Level 
                + "\nExp " + equipmentData.Exp + "/" + GameDataManager.GetGameData<Data.ExpData>(equipmentData.Level).Require;
            m_abilityText.text = string.Format(ABILITY_FORMAT,
                                               equipmentData.HP >= 0 ? "+" + equipmentData.HP : equipmentData.HP.ToString(),
                                               equipmentData.Attack >= 0 ? "+" + equipmentData.Attack : equipmentData.Attack.ToString(),
                                               equipmentData.Defense >= 0 ? "+" + equipmentData.Defense : equipmentData.Defense.ToString(),
                                               equipmentData.Speed >= 0 ? "+" + equipmentData.Speed : equipmentData.Speed.ToString());

            m_lockToggle.isOn = PlayerManager.Instance.Player.LockedEquipmentUDIDs.Contains(m_refEquipment.UDID);
            m_departButton.interactable = !m_lockToggle.isOn;

            m_equipedHint.SetActive(PlayerManager.Instance.GetEquipedCharacter(equipmentData.UDID) != null);

            m_initing = false;
        }

        public void Button_Depart()
        {
            EquipmentUtility.Depart(m_refEquipment.UDID);
            PlayerManager.Instance.SavePlayer();

            OnEdited?.Invoke();
        }

        public void Button_LevelUp()
        {
            EquipmentUtility.TryAddOneLevel(m_refEquipment);
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
