using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KahaGameCore.Static;
using UnityEngine.EventSystems;

namespace ProjectBS.UI
{
    public class MainMenuUI_BossButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private Text m_bossNameText = null;
        [SerializeField] private Text m_bossDescriptionText = null;

        private Data.BossStageData m_refBossStageData = null;
        private float m_showInfoTimer = 0f;

        public void SetUp(Data.BossStageData bossStageData)
        {
            m_refBossStageData = bossStageData;
            Data.BossData _mainBoss = GameDataManager.GetGameData<Data.BossData>(m_refBossStageData.MainBossID);
            m_bossNameText.text = ContextConverter.Instance.GetContext(_mainBoss.NameContextID);
            if(PlayerManager.Instance.Player.ClearedBossStage.Contains(bossStageData.ID))
            {
                m_bossNameText.text += " (Cleared)";
            }
            m_bossDescriptionText.text = "";
        }

        public void Button_SelectBoss()
        {
            GameManager.Instance.StartLocalCombat(m_refBossStageData);
        }

        public void Button_ShowInfo()
        {
            GameManager.Instance.MessageManager.ShowCommonMessage(
                ContextConverter.Instance.GetContext(m_refBossStageData.DescriptionContextID),
                m_bossNameText.text, null);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_showInfoTimer = GameDataManager.GameProperties.PressDownShowInfoTime;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (m_showInfoTimer > 0f)
            {
                Button_SelectBoss();
            }

            m_showInfoTimer = 0f;
        }

        private void Update()
        {
            if (m_showInfoTimer > 0f)
            {
                m_showInfoTimer -= Time.deltaTime;
                if (m_showInfoTimer <= 0f)
                {
                    Button_ShowInfo();
                    m_showInfoTimer = 0f;
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_showInfoTimer = 0f;
        }
    }
}