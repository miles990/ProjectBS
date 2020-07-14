using KahaGameCore.Interface;

namespace ProjectBS.Data
{
    public class SkillEffectData : IGameData
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Command { get; private set; }
    }
}