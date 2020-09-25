using UnityEngine;
using KahaGameCore.Interface;
using System;

namespace ProjectBS.UI
{
    public class EditPartyUIView : UIView
    {
        public override bool IsShowing => throw new NotImplementedException();

        [SerializeField] private GameObject m_root = null;
        [Header("UI Part")]
        [SerializeField] private UnityEngine.UI.Button m_nextPageButton = null;
        [SerializeField] private UnityEngine.UI.Button m_previousPageButton = null;
        [Header("Character")]
        [SerializeField] private EditPartyUI_CharacterButton[] m_characterButtons = null;
        [SerializeField] private EditPartyUI_EditCharacterUI m_characterInfoPanel = null;

        private int m_currentCharacterListPage = 0;

        private Data.OwningCharacterData m_editPartyTarget = null;

        public override void ForceShow(Manager manager, bool show)
        {
            throw new NotImplementedException();
        }

        public override void Show(Manager manager, bool show, Action onCompleted)
        {
            m_currentCharacterListPage = 0;
            m_previousPageButton.interactable = false;
            SetCharacterButton();

            m_root.SetActive(show);
            onCompleted?.Invoke();
        }

        public void Button_SwitchPage(bool next)
        {
            if (next)
                m_currentCharacterListPage++;
            else
                m_currentCharacterListPage--;

            if (m_currentCharacterListPage < 0)
                m_currentCharacterListPage = 0;

            m_previousPageButton.interactable = m_currentCharacterListPage != 0;
            SetCharacterButton();
        }

        public void Button_OnPartyButtonPressed(int index)
        {
            if(m_editPartyTarget != null)
            {
                int _partyIndex = PlayerManager.Instance.GetPartyIndex(m_editPartyTarget);
                if(_partyIndex == -1)
                {
                    PlayerManager.Instance.SetToParty(index, m_editPartyTarget);
                }
                else
                {
                    Data.OwningCharacterData _selectedChar = PlayerManager.Instance.GetCharacterByPartyIndex(index);
                    PlayerManager.Instance.SetToParty(index, m_editPartyTarget);
                    PlayerManager.Instance.SetToParty(_partyIndex, _selectedChar);
                }

                m_editPartyTarget = null;
                SetCharacterButton();
            }
            else
            {
                OnCharacterButtonPressed(PlayerManager.Instance.GetCharacterByPartyIndex(index));
            }
        }

        public void Button_StartCombat()
        {
            GameManager.StartCombat();
        }

        private void OnEnable()
        {
            for (int i = 0; i < m_characterButtons.Length; i++)
            {
                m_characterButtons[i].OnPressed += OnCharacterButtonPressed;
            }

            m_characterInfoPanel.OnEditEnded += CharacterInfoPanel_OnEditEnded;
            m_characterInfoPanel.OnStartEditParty += CharacterInfoPanel_OnStartEditParty;
        }

        private void CharacterInfoPanel_OnStartEditParty(Data.OwningCharacterData obj)
        {
            m_editPartyTarget = obj;
            for (int i = 0; i < m_characterButtons.Length; i++)
            {
                m_characterButtons[i].gameObject.SetActive(false);
            }
        }

        private void CharacterInfoPanel_OnEditEnded()
        {
            SetCharacterButton();
        }

        private void OnDisable()
        {
            for (int i = 0; i < m_characterButtons.Length; i++)
            {
                m_characterButtons[i].OnPressed -= OnCharacterButtonPressed;
            }

            m_characterInfoPanel.OnEditEnded -= CharacterInfoPanel_OnEditEnded;
            m_characterInfoPanel.OnStartEditParty -= CharacterInfoPanel_OnStartEditParty;
        }

        private void SetCharacterButton()
        {
            for (int i = 0; i < m_characterButtons.Length; i++)
            {
                m_characterButtons[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < m_characterButtons.Length; i++)
            {
                if(i + m_currentCharacterListPage * m_characterButtons.Length >= PlayerManager.Instance.Player.Characters.Count)
                {
                    break;
                }
                m_characterButtons[i].SetUp(PlayerManager.Instance.Player.Characters[i + m_currentCharacterListPage * m_characterButtons.Length]);
                m_characterButtons[i].gameObject.SetActive(true);
            }

            m_nextPageButton.interactable = PlayerManager.Instance.Player.Characters.Count - ((m_currentCharacterListPage + 1) * m_characterButtons.Length) > 0;
        }

        private void OnCharacterButtonPressed(Data.OwningCharacterData characterData)
        {
            m_characterInfoPanel.SetUp(characterData);
            m_characterInfoPanel.gameObject.SetActive(true);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.A))
            {
                Data.OwningCharacterData _newChar = CharacterUtility.CreateNewCharacter();
                CharacterUtility.LevelUp(_newChar, 100);
                PlayerManager.Instance.Player.Characters.Add(_newChar);
                PlayerManager.Instance.SavePlayer();
                SetCharacterButton();
            }
        }
    }
}

