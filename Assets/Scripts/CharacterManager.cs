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
    }
}
