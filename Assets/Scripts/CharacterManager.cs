namespace ProjectBS
{
    public class CharacterManager
    {
        public static CharacterManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new CharacterManager();
                }
                return m_instance;
            }
        }
        private static CharacterManager m_instance = null;

        private CharacterManager() { }

        public Data.OwningCharacterData CreateNewCharacter()
        {
            return new Data.OwningCharacterData
            {
                Attack = 100,
                CharacterNameID = 0,
                CharacterSpriteID = 0,
                Defence = 100,
                Equipment_UDID_Body = null,
                Equipment_UDID_Foot = null,
                Equipment_UDID_Hand = null,
                Equipment_UDID_Head = null,
                HP = 1000,
                Level = 1,
                SkillSlot_0 = 0,
                SkillSlot_1 = 0,
                SKillSlot_2 = 0,
                SKillSlot_3 = 0,
                SP = 100,
                Speed = 10,
                UDID = System.Guid.NewGuid().ToString(),
                UpgradeAbilityIDs = null
            };
        }
    }
}
