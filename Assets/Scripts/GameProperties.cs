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
    }
}

