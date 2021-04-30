using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace ProjectBS.UI
{
    public class MainMenuUI_BossButton : MainMenuUI_CustomButtonBase
    {
        [SerializeField] private TextMeshProUGUI m_bossNameText = null;
        [SerializeField] private TextMeshProUGUI m_bossDescriptionText = null;
        [SerializeField] private TextMeshProUGUI m_levelText = null;

        private Data.BossStageData m_refBossStageData = null;

        public void SetUp(Data.BossStageData bossStageData)
        {
            m_refBossStageData = bossStageData;
            Data.BossData _mainBoss = GameDataManager.GetGameData<Data.BossData>(m_refBossStageData.MainBossID);
            m_bossNameText.text = _mainBoss.GetName();
            if(PlayerManager.Instance.Player.ClearedBossStage.Contains(bossStageData.ID))
            {
                m_bossNameText.text += " (" + ContextConverter.Instance.GetContext(1000015) + ")";
            }
            m_levelText.text = m_refBossStageData.Stamina.ToString();
            m_bossDescriptionText.text = ContextConverter.Instance.GetContext(m_refBossStageData.DescriptionContextID);
        }

        protected override void OnPressed()
        {
            GameManager.Instance.StartLocalCombat(m_refBossStageData);
        }

        protected override void OnLongPressed()
        {
            GameManager.Instance.MessageManager.ShowCommonMessage(
                ContextConverter.Instance.GetContext(m_refBossStageData.DescriptionContextID),
                m_bossNameText.text, null);
        }
    }
}