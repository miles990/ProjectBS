using System;
using KahaGameCore.Interface;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace ProjectBS.UI
{
    public class EndUIView : UIView
    {
        [SerializeField] private GameObject m_gameWinInfoPanelRoot = null;
        [SerializeField] private GameObject m_gameLoseInfoPanelRoot = null;
        [Header("Game Win Info")]
        [SerializeField] private TMPro.TextMeshProUGUI m_expText = null;
        [SerializeField] private Transform m_skillInfoObjectRoot = null;
        [SerializeField] private MainMenuUI_SkillButton m_skillInfoPrefab = null;
        [SerializeField] private GameObject m_noContentInfoObjRoot = null;

        private Action onConfirm;

        public override bool IsShowing => throw new NotImplementedException();

        public override void ForceShow(Manager manager, bool show)
        {
            throw new NotImplementedException();
        }

        public override void Show(Manager manager, bool show, Action onCompleted)
        {
            throw new NotImplementedException();
        }

        public class GameWinInfo
        {
            public int exp;
            public List<int> skills;
        }

        public void Button_OkButton()
        {
            m_gameWinInfoPanelRoot.SetActive(false);
            m_gameLoseInfoPanelRoot.SetActive(false);
            onConfirm?.Invoke();
        }

        public void ShowGameWin(GameWinInfo gameWinInfo, Action onConfirm)
        {
            m_gameWinInfoPanelRoot.SetActive(true);
            StartCoroutine(StartShowExp(gameWinInfo.exp));
            StartCoroutine(StartShowSkills(gameWinInfo.skills));
            this.onConfirm = onConfirm;
        }

        public void ShowGameLose(Action onConfirm)
        {
            m_gameLoseInfoPanelRoot.SetActive(true);
            this.onConfirm = onConfirm;
        }

        private IEnumerator StartShowExp(int targetValue)
        {
            m_expText.text = "";
            yield return new WaitForSeconds(1.5f);
            float _current = 0f;
            float _per = targetValue / (0.5f / Time.fixedDeltaTime);
            for(; _current < targetValue; _current += _per)
            {
                m_expText.text = Convert.ToInt32(_current).ToString();
                yield return new WaitForFixedUpdate();
            }
            m_expText.text = Convert.ToInt32(targetValue).ToString();
        }

        private IEnumerator StartShowSkills(List<int> skills)
        {
            foreach(Transform t in m_skillInfoObjectRoot)
            {
                Destroy(t.gameObject);
            }
            yield return new WaitForSeconds(3f);

            if(skills.Count <= 0)
            {
                m_noContentInfoObjRoot.SetActive(true);
            }
            else
            {
                m_noContentInfoObjRoot.SetActive(false);
                for (int i = 0; i < skills.Count; i++)
                {
                    yield return new WaitForSeconds(0.1f);
                    MainMenuUI_SkillButton _cloneText = Instantiate(m_skillInfoPrefab);
                    _cloneText.transform.SetParent(m_skillInfoObjectRoot);
                    _cloneText.transform.localScale = Vector3.one;
                    Data.SkillData _skill = GameDataManager.GetGameData<Data.SkillData>(skills[i]);
                    string _name = ContextConverter.Instance.GetContext(_skill.NameContextID);
                    _cloneText.OnButtonPressed += OnSkillButtonPressed;
                    _cloneText.SetUp(_skill);
                }
            }
        }

        private void OnSkillButtonPressed(Data.OwningSkillData owningSkillData)
        {
            Data.SkillData _skill = GameDataManager.GetGameData<Data.SkillData>(owningSkillData.SkillSourceID);
            string _name = ContextConverter.Instance.GetContext(_skill.NameContextID);

            GameManager.Instance.MessageManager.ShowCommonMessage(
                _skill.GetAllDescriptionContext(),
                _name, null);
        }
    }
}
