﻿using UnityEngine;
using System;
using TMPro;

namespace ProjectBS.UI
{
    public class MainMenuUI_CharacterButton : MainMenuUI_CustomButtonBase
    {
        public event Action OnEdited = null;
        public event Action<Data.OwningCharacterData> OnButtonPressed = null;

        [SerializeField] private GameObject m_partyHintRoot = null;
        [SerializeField] private TextMeshProUGUI m_nameText = null;
        [Header("Status")]
        [SerializeField] private TextMeshProUGUI m_hpValueText = null;
        [SerializeField] private TextMeshProUGUI m_attackValueText = null;
        [SerializeField] private TextMeshProUGUI m_defenseValueText = null;
        [SerializeField] private TextMeshProUGUI m_speedValueText = null;
        [Header("Skills")]
        [SerializeField] private TextMeshProUGUI m_skill0Text = null;
        [SerializeField] private TextMeshProUGUI m_skill1Text = null;
        [SerializeField] private TextMeshProUGUI m_skill2Text = null;
        [SerializeField] private TextMeshProUGUI m_skill3Text = null;

        private Data.OwningCharacterData m_refCharacter = null;

        public void SetUp(Data.OwningCharacterData characterData)
        {
            m_refCharacter = characterData;
            RefreshInfo();
        }

        public void RefreshInfo()
        {
            m_nameText.text = ContextConverter.Instance.GetContext(m_refCharacter.CharacterNameID);

            m_hpValueText.text = m_refCharacter.GetTotal(Keyword.HP).ToString();
            m_attackValueText.text = m_refCharacter.GetTotal(Keyword.Attack).ToString();
            m_defenseValueText.text = m_refCharacter.GetTotal(Keyword.Defense).ToString();
            m_speedValueText.text = m_refCharacter.GetTotal(Keyword.Speed).ToString();

            m_skill0Text.text = ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(m_refCharacter.SkillSlot_0).NameContextID);
            m_skill1Text.text = ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(m_refCharacter.SkillSlot_1).NameContextID);
            m_skill2Text.text = ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(m_refCharacter.SkillSlot_2).NameContextID);
            m_skill3Text.text = ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(m_refCharacter.SkillSlot_3).NameContextID);

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

        protected override void OnPressed()
        {
            OnButtonPressed?.Invoke(m_refCharacter);
        }
    }
}
