using UnityEngine;

namespace ProjectBS.UI
{
    public class CombatUI_StatusObjLayout : MonoBehaviour
    {
        [SerializeField] private Transform[] m_4pos = null;
        [SerializeField] private Transform[] m_3pos = null;
        [SerializeField] private Transform[] m_2pos = null;
        [SerializeField] private Transform m_1pos = null;

        public void SetUp(int count)
        {
            Transform[] _target = null;

            switch(count)
            {
                case 4: _target = m_4pos; break;
                case 3: _target = m_3pos; break;
                case 2: _target = m_2pos; break;
                case 1: _target = new Transform[] { m_1pos }; break;
            }

            if (_target == null) return;

            CombatUI_CharacterPanel[] _panels = GetComponentsInChildren<CombatUI_CharacterPanel>();
            int _index = 0;
            foreach (CombatUI_CharacterPanel child in _panels)
            {
                child.transform.position = _target[_index].position;
                _index++;
            }
        }
    }
}


