using KahaGameCore.Static;
using ProjectBS.Data;

namespace ProjectBS
{
    public class ContextConverter 
    {
        public static ContextConverter Instance 
        {
            get
            {
                if(m_instance == null)
                {
                    m_instance = new ContextConverter();
                }
                return m_instance;
            }
        }
        private static ContextConverter m_instance = null;

        public enum Area
        {
            zh_tw,
            en_us,
            ja_jp
        }

        public Area area = Area.zh_tw;

        private ContextConverter() { }

        public string GetContext(int ID)
        {
            switch(area)
            {
                case Area.zh_tw:
                    {
                        return GameDataManager.GetGameData<ContextData>(ID).zh_tw;
                    }
                case Area.en_us:
                    {
                        return GameDataManager.GetGameData<ContextData>(ID).en_us;
                    }
                case Area.ja_jp:
                    {
                        return GameDataManager.GetGameData<ContextData>(ID).en_us;
                    }
                default:
                    {
                        throw new System.Exception("[ContextConverter][GetContext] Invaild Area=" + area.ToString());
                    }
            }
        }
    }
}
