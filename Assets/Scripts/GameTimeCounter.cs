using System;
using UnityEngine;

namespace ProjectBS
{
    public class GameTimeCounter : MonoBehaviour
    {
        public static GameTimeCounter Instance { get; private set; }

        public event Action OnOneSecPassed = null;

        private const string KEY = "Time";
        private float m_timer = 0f;

        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            m_timer = 0f;
            OnApplicationPause(true);
        }

        private void OnApplicationPause(bool pause)
        {
            if(pause)
            {
                PlayerPrefs.SetFloat(KEY, DateTimeOffset.Now.ToUnixTimeSeconds());
            }
            else
            {
                long _last = Convert.ToInt64(PlayerPrefs.GetFloat(KEY, DateTimeOffset.Now.ToUnixTimeSeconds()));
                long _current = Convert.ToInt64(PlayerPrefs.GetFloat(KEY, DateTimeOffset.Now.ToUnixTimeSeconds()));
                long _passed = _current - _last;
                if(_passed > GameDataManager.GameProperties.MaxPassedTimeCount)
                {
                    _passed = GameDataManager.GameProperties.MaxPassedTimeCount;
                }
                for(long i = 0; i < _passed; i++)
                {
                    OnOneSecPassed?.Invoke();
                }
                m_timer = 0f;
            }
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.SetFloat(KEY, DateTimeOffset.Now.ToUnixTimeSeconds());
        }

        private void Update()
        {
            if (GameManager.Instance.IsCombating)
                return;

            m_timer += Time.deltaTime;
            if(m_timer >= 1f)
            {
                OnOneSecPassed?.Invoke();
                m_timer = 0f;
            }
        }
    }
}
