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

        private Data.OwningEquipmentData m_refEquipment = null;

        public void SetUp(Data.OwningEquipmentData equipmentData)
        {
            m_refEquipment = equipmentData;
            Data.RawEquipmentData _source = GameDataManager.GetGameData<Data.RawEquipmentData>(equipmentData.EquipmentSourceID);

            m_nameAndLevelText.text = ContextConverter.Instance.GetContext(_source.NameContextID) + "\nLevel " + equipmentData.Level;
            m_abilityText.text = string.Format(ABILITY_FORMAT,
                                               equipmentData.HP >= 0 ? "+" + equipmentData.HP : equipmentData.HP.ToString(),
                                               equipmentData.Attack >= 0 ? "+" + equipmentData.Attack : equipmentData.Attack.ToString(),
                                               equipmentData.Defense >= 0 ? "+" + equipmentData.Defense : equipmentData.Defense.ToString(),
                                               equipmentData.Speed >= 0 ? "+" + equipmentData.Speed : equipmentData.Speed.ToString());

            m_equipedHint.SetActive(PlayerManager.Instance.GetEquipedCharacter(equipmentData.UDID) != null);
        }

        public void Button_Depart()
        {
            PlayerManager.Instance.Player.Equipments.Remove(m_refEquipment);
            PlayerManager.Instance.Player.OwnExp += GameDataManager.GetGameData<Data.ExpData>(m_refEquipment.Level).Owning / 2;
            PlayerManager.Instance.SavePlayer();

            OnEdited?.Invoke();
        }

        public void Button_LevelUp()
        {
            EquipmentUtility.TryAddOneLevel(m_refEquipment);
            PlayerManager.Instance.SavePlayer();

            OnEdited?.Invoke();
        }
    }
}
