using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KahaGameCore.Interface;
using System;

namespace ProjectBS.UI
{
    public class EditPartyUIView : UIView
    {
        public override bool IsShowing => throw new NotImplementedException();

        [SerializeField] private GameObject m_root = null;
        [Header("Character")]
        [SerializeField] private EditPartyUI_CharacterButton[] m_characterButtons = null;

        private int m_currentCharacterListPage = 0;

        public override void ForceShow(Manager manager, bool show)
        {
            throw new NotImplementedException();
        }

        public override void Show(Manager manager, bool show, Action onCompleted)
        {
            m_currentCharacterListPage = 0;
            SetCharacterButton();

            m_root.SetActive(show);
            onCompleted?.Invoke();
        }

        private void SetCharacterButton()
        {
            for (int i = 0; i < m_characterButtons.Length; i++)
            {
                m_characterButtons[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < m_characterButtons.Length; i++)
            {
                if(i + m_currentCharacterListPage >= PlayerManager.Instance.Player.Characters.Count)
                {
                    return;
                }
                m_characterButtons[i].SetUp(PlayerManager.Instance.Player.Characters[i + m_currentCharacterListPage]);
                m_characterButtons[i].gameObject.SetActive(true);
            }
        }
    }
}

