using KahaGameCore.Static;
using ProjectBS.Data;
using System;
using System.Collections;
using UnityEngine.Networking;
using JsonFx.Json;
using System.Collections.Generic;
using KahaGameCore.Interface;

namespace ProjectBS
{
    public static class GameDataManager
    {
        private static Dictionary<Type, IGameData[]> m_gameData = new Dictionary<Type, IGameData[]>();

        private const string DATA_URL = "https://wannasaynone.github.io/ProjectBSGameData/{0}/{1}.txt";

        public static bool useLocalData = false;

        public static GameProperties GameProperties { get; private set; }

        private class DownloadDataTask : IProcessable
        {
            private IEnumerator m_task = null;
            private Action m_onCompleted = null;

            public DownloadDataTask(IEnumerator task)
            {
                m_task = task;
            }

            public void Process(Action onCompleted)
            {
                m_onCompleted = onCompleted;
                GeneralCoroutineRunner.Instance.StartCoroutine(IEDoTask());
            }

            private IEnumerator IEDoTask()
            {
                yield return m_task;
                m_onCompleted?.Invoke();
            }
        }

        public static void StartLoad(Action onCompleted) 
        {
            DownloadDataTask[] _tasks = new DownloadDataTask[]
            {
                new DownloadDataTask(IELoadData<ContextData>("ContextData")),
                new DownloadDataTask(IELoadData<SkillData>("SkillData")),
                new DownloadDataTask(IELoadData<AbilityData>("AbilityData")),
                new DownloadDataTask(IELoadData<BuffData>("BuffData")),
                new DownloadDataTask(IELoadData<ExpData>("ExpData")),
                new DownloadDataTask(IELoadData<ContextData>("ContextData")),
                new DownloadDataTask(IELoadData<AppearanceData>("AppearanceData")),
                new DownloadDataTask(IELoadData<RandomSkillData>("RandomSkillData")),
                new DownloadDataTask(IELoadData<RawEquipmentData>("RawEquipmentData")),
                new DownloadDataTask(IELoadData<BossStageData>("BossStageData")),
                new DownloadDataTask(IELoadData<BossData>("BossData")),
                new DownloadDataTask(IELoadGameProperties("GameProperties")),
            };

            new KahaGameCore.Common.Processer<DownloadDataTask>(_tasks).Start(onCompleted);
        }

        public static T GetGameData<T>(int id) where T : IGameData
        {
            if (m_gameData.ContainsKey(typeof(T)))
            {
                for (int i = 0; i < m_gameData[typeof(T)].Length; i++)
                {
                    if (m_gameData[typeof(T)][i].ID == id)
                    {
                        return (T)m_gameData[typeof(T)][i];
                    }
                }
            }
            else
            {
                UnityEngine.Debug.LogErrorFormat("{0} can't be found, use LoadGameData first", typeof(T).Name);
            }

            return default;
        }

        public static T[] GetAllGameData<T>() where T : IGameData
        {
            if (m_gameData.ContainsKey(typeof(T)))
            {
                T[] _gameDatas = new T[m_gameData[typeof(T)].Length];
                for (int i = 0; i < m_gameData[typeof(T)].Length; i++)
                {
                    _gameDatas[i] = (T)m_gameData[typeof(T)][i];
                }
                return _gameDatas;
            }

            return default;
        }

        private static IEnumerator IELoadData<T>(string name) where T : IGameData
        {
            UnityEngine.Debug.Log("Start Load Data:" + typeof(T).ToString());

            string _url;

            if (useLocalData)
                _url = UnityEngine.Application.streamingAssetsPath + "/" + GameSetting.dataSource + "/" + name + ".txt";
            else
                _url = string.Format(DATA_URL, GameSetting.dataSource, name);

            UnityWebRequest _request = UnityWebRequest.Get(_url);
            
            yield return _request.SendWebRequest();

            if (m_gameData.ContainsKey(typeof(T)))
            {
                T[] _datas = JsonReader.Deserialize<T[]>(_request.downloadHandler.text);
                m_gameData[typeof(T)] = new IGameData[_datas.Length];
                for (int i = 0; i < _datas.Length; i++)
                {
                    m_gameData[typeof(T)][i] = _datas[i];
                }
            }
            else
            {
                T[] _data = JsonReader.Deserialize<T[]>(_request.downloadHandler.text);
                IGameData[] _gameDatas = new IGameData[_data.Length];
                for (int i = 0; i < _data.Length; i++)
                {
                    _gameDatas[i] = _data[i];
                }
                m_gameData.Add(typeof(T), _gameDatas);
            }
            UnityEngine.Debug.Log("Completed Load Data:" + typeof(T).ToString());
        }

        private static IEnumerator IELoadGameProperties(string name)
        {
            UnityWebRequest _request = UnityWebRequest.Get(string.Format(DATA_URL, GameSetting.dataSource, name));
            yield return _request.SendWebRequest();
            GameProperties = JsonReader.Deserialize<GameProperties>(_request.downloadHandler.text);
        }
    }
}
