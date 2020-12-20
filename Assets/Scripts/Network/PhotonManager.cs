using JsonFx.Json;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBS.Network
{
    public class PhotonManager : MonoBehaviourPunCallbacks
    {
        public static PhotonManager Instance { get; private set; }

        public PhotonView PhotonView { get { return m_photonView; } }
        [SerializeField] private PhotonView m_photonView = null;

        private int m_id = 0;

        public bool IsConnected { get { return PhotonNetwork.IsConnected; } }
        public bool IsMaster { get { return m_id == 0; } }

        private Dictionary<int, string> m_idToTeamJson = new Dictionary<int, string>();

        private Dictionary<int, Action> m_callbackCodeToAction = new Dictionary<int, Action>();
        private int m_receiveCallbaclCode = -1;

        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Update()
        {
            if(m_callbackCodeToAction.Count > 0)
            {
                if(m_callbackCodeToAction.ContainsKey(m_receiveCallbaclCode))
                {
                    Action _todo = m_callbackCodeToAction[m_receiveCallbaclCode];
                    m_callbackCodeToAction.Remove(m_receiveCallbaclCode);
                    _todo.Invoke();
                }
            }
            m_receiveCallbaclCode = -1;
        }

        public void ConnectToLobby()
        {
            m_idToTeamJson.Clear();
            PhotonNetwork.GameVersion = "0.0.0";
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            PhotonNetwork.CreateRoom(Guid.NewGuid().ToString(), new RoomOptions { MaxPlayers = 2 });
        }

        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.IsMasterClient)
                m_id = 0;
            else
                m_id = 1;

            Debug.Log("Joined ID=" + m_id);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            // only called by master
            StartSyncMasterPartyToSlave();
        }

        private void StartSyncMasterPartyToSlave()
        {
            string _json = GetPartyJson();
            m_idToTeamJson.Add(0, _json);
            SetWaitCallback(CallbackCode.IsMasterPartySynced, MasterCallSlaveSyncParty);
            m_photonView.RPC(nameof(Pun_SyncMasterParty), RpcTarget.All, _json);
        }

        private void MasterCallSlaveSyncParty()
        {
            SetWaitCallback(CallbackCode.IsSyncSlavePartyTaskDone, OnBothSidePartySet);
            m_photonView.RPC(nameof(Pun_StartToSyncSlaveParty), RpcTarget.All);
        }

        private void OnBothSidePartySet()
        {
            SetCombatManager();
            SetWaitCallback(CallbackCode.IsSlaveCombatManagerInited, OnSlaveCombatManagerInited);
            m_photonView.RPC(nameof(Pun_CallSlaveInitCombatManager), RpcTarget.All);
        }

        private void OnSlaveCombatManagerInited()
        {
            StartBattle();
            SetWaitCallback(CallbackCode.IsSlaveBattleStarted, OnSlaveIsAllInited);
            m_photonView.RPC(nameof(CallSlaveStartBattle), RpcTarget.All);
        }

        [PunRPC]
        private void CallSlaveStartBattle()
        {
            if(m_id == 1)
            {
                StartBattle();
                SendCallback(CallbackCode.IsSlaveBattleStarted);
            }
        }

        private void StartBattle()
        {
            Data.OwningCharacterData[] _player = JsonReader.Deserialize<Data.OwningCharacterData[]>(m_idToTeamJson[m_id]);
            Data.OwningCharacterData[] _opponent = JsonReader.Deserialize<Data.OwningCharacterData[]>(m_idToTeamJson[m_id == 0 ? 1 : 0]);

            List<Combat.CombatUnit> _playerUnits = new List<Combat.CombatUnit>();
            List<Combat.CombatUnit> _opponentUnits = new List<Combat.CombatUnit>();

            for (int i = 0; i < _player.Length; i++)
            {
                _playerUnits.Add(Combat.CombatUtility.CreateUnit(_player[i], 0));
            }

            for (int i = 0; i < _opponent.Length; i++)
            {
                _opponentUnits.Add(Combat.CombatUtility.CreateUnit(_opponent[i], 1));
            }

            Combat.CombatUtility.ComabtManager.StartCombat(_playerUnits, _opponentUnits);
        }

        private void OnSlaveIsAllInited()
        {
            ((Combat.OnlineCombatManager)Combat.CombatUtility.ComabtManager).Master_StartBattle();
        }

        ////////////////////////////////////////////////////////////////////////////

        [PunRPC]
        private void Pun_SyncMasterParty(string json)
        {
            if (m_id == 1)
            {
                m_idToTeamJson.Add(0, json);
                SendCallback(CallbackCode.IsMasterPartySynced);
            }
        }

        [PunRPC]
        private void Pun_StartToSyncSlaveParty()
        {
            if (m_id == 1)
            {
                string _json = GetPartyJson();
                m_idToTeamJson.Add(1, _json);
                SetWaitCallback(CallbackCode.IsSlavePartySynced, OnSlavePartySynced);
                m_photonView.RPC(nameof(Pun_SyncSlaveParty), RpcTarget.All, _json);
            }
        }

        [PunRPC]
        private void Pun_SyncSlaveParty(string json)
        {
            if (m_id == 0)
            {
                m_idToTeamJson.Add(1, json);
                SendCallback(CallbackCode.IsSlavePartySynced);
            }
        }

        [PunRPC]
        private void Pun_CallSlaveInitCombatManager()
        {
            if (m_id == 1)
            {
                SetCombatManager();
                SendCallback(CallbackCode.IsSlaveCombatManagerInited);
            }
        }

        private void OnSlavePartySynced()
        {
            SendCallback(CallbackCode.IsSyncSlavePartyTaskDone);
        }

        ////////////////////////////////////////////////////////////////////////////

        public void SendCallback(int code)
        {
            if (m_id == 0)
            {
                m_photonView.RPC(nameof(Pun_MasterSendCallBack), RpcTarget.All, code);
            }
            else if(m_id == 1)
            {
                m_photonView.RPC(nameof(Pun_SlaveSendCallBack), RpcTarget.All, code);
            }
        }

        [PunRPC]
        private void Pun_SlaveSendCallBack(int code)
        {
            if (m_id == 0)
            {
                m_receiveCallbaclCode = code;
            }
        }

        [PunRPC]
        private void Pun_MasterSendCallBack(int code)
        {
            if (m_id == 1)
            {
                m_receiveCallbaclCode = code;
            }
        }

        public void SetWaitCallback(int waitCode, Action onReceived)
        {
            if(m_callbackCodeToAction.ContainsKey(waitCode))
            {
                throw new Exception("[PhotonManager][SetWaitCallback] Is already waiting code:" + waitCode);
            }

            m_callbackCodeToAction.Add(waitCode, onReceived);
        }

        public void CallTheOther(string command, string paras = "", int callbackCode = -1)
        {
            m_photonView.RPC(nameof(DoRPCCommand), RpcTarget.Others, m_id, command, callbackCode, paras);
        }

        [PunRPC]
        private void DoRPCCommand(int from, string command, int callbackCode, string paras)
        {
            if(from == m_id)
            {
                return;
            }

            ((Combat.OnlineCombatManager)Combat.CombatUtility.ComabtManager).DoRPCCommand(command, callbackCode, paras);
        }

        public void SyncMyCombatUnits()
        {
            List<Combat.CombatUnit> _myUnits = new List<Combat.CombatUnit>();
            List<Combat.CombatUnit> _allUnits = Combat.CombatUtility.ComabtManager.AllUnit;

            for (int i = 0; i < _allUnits.Count; i++)
            {
                if(_allUnits[i].camp == 0)
                {
                    _myUnits.Add(_allUnits[i].GetJsonableData());
                }
            }

            string _json = JsonWriter.Serialize(_myUnits.ToArray());
            m_photonView.RPC(nameof(Pun_SyncCombatUnits), RpcTarget.All, m_id, _json);
        }

        public void SyncAllCombatUnits()
        {
            List<Combat.CombatUnit> _needToSyncUnits = new List<Combat.CombatUnit>();
            List<Combat.CombatUnit> _allUnits = Combat.CombatUtility.ComabtManager.AllUnit;

            for (int i = 0; i < _allUnits.Count; i++)
            {
                _needToSyncUnits.Add(_allUnits[i].GetJsonableData());
            }

            string _json = JsonWriter.Serialize(_allUnits.ToArray());
            m_photonView.RPC(nameof(Pun_SyncCombatUnits), RpcTarget.All, m_id, _json);
        }

        [PunRPC]
        private void Pun_SyncCombatUnits(int sendBy, string json)
        {
            if (m_id == sendBy)
                return;

            Combat.CombatUnit[] _units = JsonReader.Deserialize<Combat.CombatUnit[]>(json);
            ((Combat.OnlineCombatManager)Combat.CombatUtility.ComabtManager).ForceUpdateCombatUnitsStatus(_units);
        }

        ////////////////////////////////////////////////////////////////////////////

        public void SyncLog(string log)
        {
            if (!PhotonNetwork.IsConnected)
            {
                Debug.Log(log);
                return;
            }

            m_photonView.RPC(nameof(Pun_Log), RpcTarget.All, m_id, log);
        }

        [PunRPC]
        private void Pun_Log(int id, string log)
        {
            Debug.LogFormat("PhotonID={0} log={1}", id, log);
        }

        ////////////////////////////////////////////////////////////////////////////

        private string GetPartyJson()
        {
            Data.OwningCharacterData[] _party = new Data.OwningCharacterData[4];
            _party[0] = PlayerManager.Instance.GetCharacterByUDID(PlayerManager.Instance.Player.Party.MemberUDID_0);
            _party[1] = PlayerManager.Instance.GetCharacterByUDID(PlayerManager.Instance.Player.Party.MemberUDID_1);
            _party[2] = PlayerManager.Instance.GetCharacterByUDID(PlayerManager.Instance.Player.Party.MemberUDID_2);
            _party[3] = PlayerManager.Instance.GetCharacterByUDID(PlayerManager.Instance.Player.Party.MemberUDID_3);

            return JsonWriter.Serialize(_party);
        }

        private void SetCombatManager()
        {
            Combat.OnlineCombatManager _combatManager = new Combat.OnlineCombatManager();
            Combat.CombatUtility.SetCombatManager(_combatManager);
        }
    }
}