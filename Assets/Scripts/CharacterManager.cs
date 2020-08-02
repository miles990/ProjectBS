using ProjectBS.Data;
using System.Collections.Generic;

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

        private bool m_inited = false;
        private List<AbilityData> m_hpAbiPool = null;
        private int m_hpTotalWeight = -1;
        private List<AbilityData> m_attackAbiPool = null;
        private int m_attackTotalWeight = -1;
        private List<AbilityData> m_defenceAbiPool = null;
        private int m_defenceTotalWeight = -1;
        private List<AbilityData> m_speedAbiPool = null;
        private int m_speedTotalWeight = -1;

        public OwningCharacterData CreateNewCharacter()
        {

        }
    }
}
