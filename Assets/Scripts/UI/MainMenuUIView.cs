using System;
using KahaGameCore.Interface;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectBS.UI
{
    public class MainMenuUIView : UIView
    {
        public override bool IsShowing { get { return m_root.activeSelf; } }

        [Serializable]
        private class PanelData
        {
            public Animator downButtonAnimator = null;
            public MainMenuUI_PanelBase panel = null;
        }

        [SerializeField] private GameObject m_root = null;
        [SerializeField] private GameObject m_preloadAnimationObject = null;
        [Header("Top")]
        [SerializeField] private TextMeshProUGUI m_staminaText = null;
        [SerializeField] private Image m_staminaImage = null;
        [SerializeField] private TextMeshProUGUI m_expAmountText = null;
        [Header("Down")]
        [SerializeField] private GameObject[] m_disableIfFirstPlay = null;
        [Header("Panels")]
        [SerializeField] private int m_defaultIndex = 0;
        [SerializeField] private PanelData[] m_panelDatas = null;

        private Animator m_playingDownButtonAnimator = null;

        private void Update()
        {
            if (m_root.activeSelf && PlayerManager.Instance.IsInited)
            {
                m_staminaText.text = PlayerManager.Instance.Player.Stamina + " / " + GameDataManager.GameProperties.MaxStamina;
                m_staminaImage.fillAmount = (float)PlayerManager.Instance.Player.Stamina / (float)GameDataManager.GameProperties.MaxStamina;
                m_expAmountText.text = PlayerManager.Instance.Player.OwnExp.ToString();
            }
        }

        public override void ForceShow(Manager manager, bool show)
        {
            throw new NotImplementedException();
        }

        public override void Show(Manager manager, bool show, Action onCompleted)
        {
            m_root.SetActive(show);
            if (show)
            {
                Button_SwitchTo(m_panelDatas[m_defaultIndex].downButtonAnimator);
                m_preloadAnimationObject.SetActive(false);

                for(int i = 0; i < m_disableIfFirstPlay.Length; i++)
                {
                    m_disableIfFirstPlay[i].SetActive(PlayerManager.Instance.Player.ClearedBossStage.Count > 0);
                }
            }
            onCompleted?.Invoke();
        }

        public void Button_SwitchTo(Animator button)
        {
            DisableAllPanel();
            for(int i = 0; i < m_panelDatas.Length; i++)
            {
                if(m_panelDatas[i].downButtonAnimator == button)
                {
                    if (m_playingDownButtonAnimator == button)
                        return;

                    m_playingDownButtonAnimator = button;
                    m_playingDownButtonAnimator.Play("Disable", 0, 0f);

                    m_panelDatas[i].panel.Show();
                }
                else
                {
                    if(m_panelDatas[i].downButtonAnimator.gameObject.activeSelf)
                        m_panelDatas[i].downButtonAnimator.Play("Enable");
                }
            }
        }

        public void Button_Ad_AddStamina()
        {
            GameManager.Instance.MessageManager.ShowCommonMessage(
                string.Format(ContextConverter.Instance.GetContext(1000032), GameDataManager.GameProperties.AddStaminaByAd),
                "Event",
                StartShowAd,
                delegate { });
        }

        private void StartShowAd()
        {
            PlayerManager.Instance.Player.Stamina += GameDataManager.GameProperties.AddStaminaByAd;
            PlayerManager.Instance.SavePlayer();

            KahaGameCore.Static.TimerManager.Schedule(0.1f, delegate
            {
                GameManager.Instance.MessageManager.ShowCommonMessage(
                    "測試訊息：你吃了一個廣告",
                    "Event",
                    null);
            });
        }

        private void DisableAllPanel()
        {
            for (int i = 0; i < m_panelDatas.Length; i++)
            {
                m_panelDatas[i].panel.Hide();
            }
        }
    }
}
