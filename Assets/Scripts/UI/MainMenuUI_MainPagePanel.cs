using UnityEngine;
using TMPro;

namespace ProjectBS.UI
{
    public class MainMenuUI_MainPagePanel : MainMenuUI_PanelBase
    {
        [SerializeField] private TextMeshProUGUI m_hintText = null;

        private float m_updateHintTimer = 0f;

        protected override void OnHidden()
        {
        }

        protected override void OnShown()
        {
            UpdateHint();
        }

        protected override void Update()
        {
            if(m_updateHintTimer > 0f)
            {
                m_updateHintTimer -= Time.deltaTime;
                if(m_updateHintTimer <= 0f)
                {
                    UpdateHint();
                }
            }

            base.Update();
        }

        public void UpdateHint()
        {
            m_updateHintTimer = GameDataManager.GameProperties.UpdateMainMenuHintTime;
            m_hintText.text = ContextConverter.Instance.GetContext(Random.Range(GameDataManager.GameProperties.MainMenuHintMinID, GameDataManager.GameProperties.MainMenuHintMaxID + 1));
        }
    }
}
