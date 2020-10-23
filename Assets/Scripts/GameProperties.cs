using UnityEngine;

namespace ProjectBS
{
    [CreateAssetMenu(menuName = "Data/Game Properties")]
    public class GameProperties : ScriptableObject
    {
        public int MaxDropCount { get { return m_maxDropCount; } }
        [SerializeField] private int m_maxDropCount = 20;

        public int MinDropCount { get { return m_minDropCount; } }
        [SerializeField] private int m_minDropCount = 10;

        public float DropSkillChance { get { return m_dropSkillChance; } }
        [SerializeField] private float m_dropSkillChance = 50f;

        public float PressDownShowInfoTime { get { return m_pressDownShowInfoTime; } }
        [SerializeField] private float m_pressDownShowInfoTime = 1f;
    }
}

