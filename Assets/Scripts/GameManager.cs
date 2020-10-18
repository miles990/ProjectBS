using KahaGameCore.Static;
using System.Collections.Generic;

namespace ProjectBS
{
    public class GameManager : KahaGameCore.Interface.Manager
    {
        public static GameManager Instance 
        {
            get
            { 
                if(m_instance == null)
                {
                    m_instance = new GameManager();
                }

                return m_instance;
            }
        }
        private static GameManager m_instance = null;
        private GameManager() { }

        private enum State
        {
            None,
            InitData,
            MainMenu,
            Combat
        }

        public KahaGameCore.Common.ConfirmWindowManager MessageManager { get; private set; }

        private State m_currentState = State.None;

        private Combat.CombatManager m_combatManager = null;
        private Data.BossStageData m_currentPlayingStage = null;

        public void StartGame()
        {
            if(m_currentState != State.None)
            {
                throw new System.Exception("[GameManager][StartGame] Game is already started");
            }

            StartInitData();
        }

        public void StartCombat(Data.BossStageData bossStageData)
        {
            if (m_currentState != State.MainMenu)
                throw new System.Exception("[GameManager][StartGame] Can't start comabt now");

            if (PlayerManager.Instance.Player.Stamina < bossStageData.Stamina)
            {
                return;
            }

            m_currentPlayingStage = bossStageData;
            m_currentState = State.Combat;

            if (PlayerManager.Instance.Player.ClearedBossStage.Contains(bossStageData.ID))
            {
                EndCombat();
                return;
            }

            GetPage<UI.MainMenuUIView>().Show(this, false, null);

            string[] _bossIDs = bossStageData.BossIDs.Split('$');
            List<Data.BossData> _bosses = new List<Data.BossData>();
            for(int i = 0; i < _bossIDs.Length; i++)
            {
                if(string.IsNullOrEmpty(_bossIDs[i]))
                {
                    continue;
                }
                _bosses.Add(GameDataManager.GetGameData<Data.BossData>(int.Parse(_bossIDs[i])));
            }

            m_combatManager = new Combat.CombatManager();
            m_combatManager.StartCombat(
                PlayerManager.Instance.Player.Party, _bosses);
        }

        public void EndCombat()
        {
            if (m_currentState != State.Combat)
                throw new System.Exception("[GameManager][EndCombat] Can't end comabt since it didn't start");

            m_currentState = State.MainMenu;
            GetPage<UI.MainMenuUIView>().Show(this, true, null);

            DropUtility.DropInfo _drop = DropUtility.Drop(m_currentPlayingStage);

            // TODO: should set UI here, use _resultString for testing now
            string _resultString = "Add Exp: " + _drop.exp + "\n\nAdd Equipments:\n";

            PlayerManager.Instance.Player.OwnExp += _drop.exp;
            for(int i = 0; i < _drop.equipments.Count; i++)
            {
                Data.RawEquipmentData _source = GameDataManager.GetGameData<Data.RawEquipmentData>(_drop.equipments[i].EquipmentSourceID);
                _resultString += ContextConverter.Instance.GetContext(_source.NameContextID);
                PlayerManager.Instance.Player.Equipments.Add(_drop.equipments[i]);

                if (i != _drop.equipments.Count - 1)
                    _resultString += ", ";
                else
                    _resultString += "\n\nAdd Skills:\n";
            }

            PlayerManager.Instance.Player.ClearedBossStage.Add(m_currentPlayingStage.ID);
            PlayerManager.Instance.Player.Stamina -= m_currentPlayingStage.Stamina;
            for(int i = 0; i < _drop.skillIDs.Count; i++)
            {
                Data.SkillData _source = GameDataManager.GetGameData<Data.SkillData>(_drop.skillIDs[i]);
                _resultString += ContextConverter.Instance.GetContext(_source.NameContextID);

                PlayerManager.Instance.AddSkill(_drop.skillIDs[i]);

                if (i != _drop.skillIDs.Count - 1)
                    _resultString += ", ";
            }

            m_currentPlayingStage = null;
            m_combatManager = null;

            PlayerManager.Instance.SavePlayer();

            MessageManager.ShowCommonMessage(_resultString, "Victory", null);
        }

        private void StartInitData()
        {
            m_currentState = State.InitData;
            GameDataLoader.StartLoad();
            PlayerManager.Instance.Init();
            ShowMainMenu();
        }

        private void ShowMainMenu()
        {
            m_currentState = State.MainMenu;
            MessageManager = new KahaGameCore.Common.ConfirmWindowManager(GetPage<UI.ConfirmWindowUIView>());
            GetPage<UI.MainMenuUIView>().Show(this, true, null);
        }
    }
}
