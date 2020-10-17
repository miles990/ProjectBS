using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KahaGameCore.Static;

namespace ProjectBS.UI
{
    public class MainMenuUI_BossButton : MonoBehaviour
    {
        [SerializeField] private Text m_bossNameText = null;
        [SerializeField] private Text m_bossDescriptionText = null;

        private Data.BossStageData m_refBossStageData = null;

        public void SetUp(Data.BossStageData bossStageData)
        {
            m_refBossStageData = bossStageData;
            Data.BossData _mainBoss = GameDataManager.GetGameData<Data.BossData>(m_refBossStageData.MainBossID);
            m_bossNameText.text = ContextConverter.Instance.GetContext(_mainBoss.NameContextID);
            m_bossDescriptionText.text = ContextConverter.Instance.GetContext(m_refBossStageData.DescriptionContextID);
        }

        public void Button_SelectBoss()
        {
            GameManager.Instance.StartCombat(m_refBossStageData);
        }
    }
}