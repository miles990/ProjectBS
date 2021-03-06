using UnityEngine;
using System;
using TMPro;

namespace ProjectBS.UI
{
    public class MainMenuUI_CharacterButton : MainMenuUI_CustomButtonBase
    {
        public event Action OnEdited = null;
        public event Action<Data.OwningCharacterData> OnButtonPressed = null;

        [SerializeField] private GameObject m_partyHintRoot = null;
        [SerializeField] private UnityEngine.UI.RawImage m_iconImage = null;
        [SerializeField] private TextMeshProUGUI m_nameText = null;
        [SerializeField] private TextMeshProUGUI m_levelText = null;
        [Header("Status")]
        [SerializeField] private TextMeshProUGUI m_hpValueText = null;
        [SerializeField] private TextMeshProUGUI m_attackValueText = null;
        [SerializeField] private TextMeshProUGUI m_defenseValueText = null;
        [SerializeField] private TextMeshProUGUI m_speedValueText = null;

        private Data.OwningCharacterData m_refCharacter = null;

        public void SetUp(Data.OwningCharacterData characterData)
        {
            m_refCharacter = characterData;
            RefreshInfo();
        }

        public void RefreshInfo()
        {
            m_nameText.text = m_refCharacter.Name;
            m_iconImage.texture = m_refCharacter.GetIcon();
            m_levelText.text = "Lv." + m_refCharacter.Level;

            string _hpRankString = m_refCharacter.GetAbilityRankString(Keyword.HP);
            string _attackRankString = m_refCharacter.GetAbilityRankString(Keyword.Attack);
            string _defenseRankString = m_refCharacter.GetAbilityRankString(Keyword.Defense);
            string _speedRankString = m_refCharacter.GetAbilityRankString(Keyword.Speed);
            Color _hpRankColor = UITextColorStorer.GetRankStringColor(_hpRankString);
            Color _attackRankColor = UITextColorStorer.GetRankStringColor(_attackRankString);
            Color _defenseRankColor = UITextColorStorer.GetRankStringColor(_defenseRankString);
            Color _speedRankColor = UITextColorStorer.GetRankStringColor(_speedRankString);

            m_hpValueText.text = "<#" + ColorUtility.ToHtmlStringRGB(_hpRankColor) + ">" + _hpRankString + "</color> " + m_refCharacter.GetTotal(Keyword.HP).ToString();
            m_attackValueText.text = "<#" + ColorUtility.ToHtmlStringRGB(_attackRankColor) + ">" + _attackRankString + "</color> " + m_refCharacter.GetTotal(Keyword.Attack).ToString();
            m_defenseValueText.text = "<#" + ColorUtility.ToHtmlStringRGB(_defenseRankColor) + ">" + _defenseRankString + "</color> " + m_refCharacter.GetTotal(Keyword.Defense).ToString();
            m_speedValueText.text = "<#" + ColorUtility.ToHtmlStringRGB(_speedRankColor) + ">" + _speedRankString + "</color> " + m_refCharacter.GetTotal(Keyword.Speed).ToString();

            int _partyIndex = PlayerManager.Instance.GetPartyIndex(m_refCharacter);
            if (_partyIndex != -1)
            {
                m_partyHintRoot.SetActive(true);
            }
            else
            {
                m_partyHintRoot.SetActive(false);
            }
        }

        public void Button_Depart()
        {
            if(CharacterUtility.Depart(m_refCharacter.UDID))
            {
                PlayerManager.Instance.SavePlayer();
                OnEdited?.Invoke();
            }
        }

        public void Button_LevelUp(int add)
        {
            for(int i = 0; i < add; i++)
            {
                CharacterUtility.TryAddOneLevel(m_refCharacter);
            }
            PlayerManager.Instance.SavePlayer();
            OnEdited?.Invoke();
        }

        protected override void OnPressed()
        {
            OnButtonPressed?.Invoke(m_refCharacter);
        }

        protected override void OnLongPressed()
        {
            OnButtonPressed?.Invoke(m_refCharacter);
        }
    }
}
