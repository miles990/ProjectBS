namespace ProjectBS
{
    public class GameProperties 
    {
        public int MaxDropCount { get; private set; }
        public int MinDropCount { get; private set; }
        public float DropNormalSkillChance { get; private set; }
        public float PressDownShowInfoTime { get; private set; }
        public long MaxPassedTimeCount { get; private set; }
        public int MaxStamina { get; private set; }
        public int AddStaminaPerSec { get; private set; }
        public int AddStaminaPerTime { get; private set; }
        public int DrawCharacterCost { get; private set; }
        public int DrawSkillCost { get; private set; }
        public int AddCharacterCountPerDraw { get; private set; }
        public int AddSkillCountPerDraw { get; private set; }
        public float UpdateMainMenuHintTime { get; private set; }
        public int MainMenuHintMinID { get; private set; }
        public int MainMenuHintMaxID { get; private set; }
        public int AddStaminaByAd { get; private set; }
    }
}

