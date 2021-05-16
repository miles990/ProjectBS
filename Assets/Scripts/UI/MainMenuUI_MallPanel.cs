using ProjectBS.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBS.UI
{
    public class MainMenuUI_MallPanel : MainMenuUI_PanelBase
    {
        [SerializeField] private MainMenuUI_CharacterButton m_characterButtonPrefab = null;
        [SerializeField] private MainMenuUI_SkillButton m_skillButtonPrefab = null;
        [SerializeField] private GameObject m_luckyDrawCharacterResultRoot = null;
        [SerializeField] private GameObject m_luckyDrawSkillResultRoot = null;
        [SerializeField] private RectTransform m_characterButtonContainer = null;
        [SerializeField] private RectTransform m_skillButtonContainer = null;
        [SerializeField] private MainMenuUI_CharacterInfoPanel m_characterInfoPanel = null;

        private List<OwningCharacterData> m_addCharacters = null;
        private List<MainMenuUI_CharacterButton> m_allClonedCharacterButtons = new List<MainMenuUI_CharacterButton>();

        private List<SkillData> m_addSkills = null;
        private List<MainMenuUI_SkillButton> m_allClonedSkillButtons = new List<MainMenuUI_SkillButton>();


        private void Start()
        {
        }

        protected override void OnHidden()
        {
        }

        protected override void OnShown()
        {
            Button_CloseDrawResult();
        }

        public void Purchase(string id)
        {
            IAP.ProductManager.Instance.BuyProductID(id);
        }

        public void LuckyDraw_Character()
        {
            if(PlayerManager.Instance.Player.OwnExp < GameDataManager.GameProperties.DrawCharacterCost)
            {
                GameManager.Instance.MessageManager.ShowCommonMessage(
                    string.Format(ContextConverter.Instance.GetContext(1000031), GameDataManager.GameProperties.DrawCharacterCost),
                    "Warning", 
                    null);
                return;
            }

            PlayerManager.Instance.Player.OwnExp -= GameDataManager.GameProperties.DrawCharacterCost;
            m_addCharacters = new List<OwningCharacterData>();
            for(int i = 0; i < GameDataManager.GameProperties.AddCharacterCountPerDraw; i++)
            {
                m_addCharacters.Add(CharacterUtility.CreateNewCharacter());
                PlayerManager.Instance.Player.Characters.Add(m_addCharacters[m_addCharacters.Count - 1]);
            }

            ShowLuckyDrawResult_Character();

            PlayerManager.Instance.SavePlayer();
        }

        public void LuckyDraw_Skill()
        {
            if (PlayerManager.Instance.Player.OwnExp < GameDataManager.GameProperties.DrawSkillCost)
            {
                GameManager.Instance.MessageManager.ShowCommonMessage(
                    string.Format(ContextConverter.Instance.GetContext(1000031), GameDataManager.GameProperties.DrawSkillCost),
                    "Warning",
                    null);
                return;
            }

            PlayerManager.Instance.Player.OwnExp -= GameDataManager.GameProperties.DrawSkillCost;
            m_addSkills = new List<SkillData>();
            for (int i = 0; i < GameDataManager.GameProperties.AddSkillCountPerDraw; i++)
            {
                m_addSkills.Add(DropUtility.GetRandomSkill());
                PlayerManager.Instance.AddSkill(m_addSkills[m_addSkills.Count - 1].ID);
            }

            ShowLuckyDrawResult_Skill();

            PlayerManager.Instance.SavePlayer();
        }

        public void Button_DepartAllCharacter()
        {
            for(int i = 0; i < m_addCharacters.Count; i++)
            {
                CharacterUtility.Depart(m_addCharacters[i].UDID);
            }
            Button_CloseDrawResult();
        }

        private void ShowLuckyDrawResult_Character()
        {
            Debug.Log("ShowLuckyDrawResult_Character");
            for (int i = 0; i < m_allClonedCharacterButtons.Count; i++)
            {
                m_allClonedCharacterButtons[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < m_addCharacters.Count; i++)
            {
                if (i < m_allClonedCharacterButtons.Count)
                {
                    m_allClonedCharacterButtons[i].SetUp(m_addCharacters[i]);
                    m_allClonedCharacterButtons[i].gameObject.SetActive(true);
                }
                else
                {
                    MainMenuUI_CharacterButton _cloneButton = Instantiate(m_characterButtonPrefab);
                    _cloneButton.transform.SetParent(m_characterButtonContainer);
                    _cloneButton.transform.localScale = Vector3.one;
                    _cloneButton.SetUp(m_addCharacters[i]);
                    _cloneButton.OnButtonPressed += OnCharacterButtonPressed;
                    _cloneButton.OnEdited += RefreshCharacterPageButtonState;
                    m_allClonedCharacterButtons.Add(_cloneButton);
                }
            }
            m_luckyDrawCharacterResultRoot.SetActive(true);
        }

        private void ShowLuckyDrawResult_Skill()
        {
            for (int i = 0; i < m_allClonedSkillButtons.Count; i++)
            {
                m_allClonedSkillButtons[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < m_addSkills.Count; i++)
            {
                if (i < m_allClonedSkillButtons.Count)
                {
                    m_allClonedSkillButtons[i].SetUp(m_addSkills[i]);
                    m_allClonedSkillButtons[i].gameObject.SetActive(true);
                }
                else
                {
                    MainMenuUI_SkillButton _cloneButton = Instantiate(m_skillButtonPrefab);
                    _cloneButton.transform.SetParent(m_skillButtonContainer);
                    _cloneButton.transform.localScale = Vector3.one;
                    _cloneButton.SetUp(m_addSkills[i]);
                    _cloneButton.OnButtonPressed += OnSkillButtonPressed;
                    m_allClonedSkillButtons.Add(_cloneButton);
                }
            }
            m_luckyDrawSkillResultRoot.SetActive(true);
        }

        private void OnSkillButtonPressed(Data.OwningSkillData owningSkillData)
        {
            Data.SkillData _skill = GameDataManager.GetGameData<Data.SkillData>(owningSkillData.SkillSourceID);
            string _name = ContextConverter.Instance.GetContext(_skill.NameContextID);

            GameManager.Instance.MessageManager.ShowCommonMessage(
                _skill.GetAllDescriptionContext(),
                _name, null);
        }

        public void Button_CloseDrawResult()
        {
            m_characterInfoPanel.OnEditEnded -= RefreshCharacterPageButtonState;
            m_luckyDrawCharacterResultRoot.SetActive(false);
        }

        private void OnCharacterButtonPressed(OwningCharacterData obj)
        {
            m_characterInfoPanel.Enable(obj);
        }

        private void RefreshCharacterPageButtonState()
        {
            List<OwningCharacterData> _allCharacter = PlayerManager.Instance.Player.Characters;
            for(int i = 0; i < m_addCharacters.Count; i++)
            {
                if(!_allCharacter.Contains(m_addCharacters[i]))
                {
                    m_addCharacters.RemoveAt(i);
                    i--;
                }
            }
            ShowLuckyDrawResult_Character();
        }
    }
}
