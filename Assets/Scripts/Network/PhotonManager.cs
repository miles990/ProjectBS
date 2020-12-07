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

        private int m_waitCallbackCode = -1;
        private int m_receiveCallbaclCode = -1;
        private Action m_nextStep = null;

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
            if(m_waitCallbackCode > 0)
            {
                if(m_waitCallbackCode == m_receiveCallbaclCode)
                {
                    m_waitCallbackCode = -1;
                    m_receiveCallbaclCode = -1;
                    m_nextStep?.Invoke();
                }
            }
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
            Data.OwningCharacterData[] _player = JsonReader.Deserialize<Data.OwningCharacterData[]>(m_idToTeamJson[m_id]);
            Data.OwningCharacterData[] _opponent = JsonReader.Deserialize<Data.OwningCharacterData[]>(m_idToTeamJson[m_id == 0 ? 1 : 0]);

            List<Combat.CombatUnit> _playerUnits = new List<Combat.CombatUnit>();
            List<Combat.CombatUnit> _opponentUnits = new List<Combat.CombatUnit>();

            for (int i = 0; i < _player.Length; i++)
            {
                _playerUnits.Add(Combat.CombatUtility.GetUnit(_player[i], 0));
            }

            for (int i = 0; i < _opponent.Length; i++)
            {
                _opponentUnits.Add(Combat.CombatUtility.GetUnit(_opponent[i], 1));
            }

            Combat.CombatUtility.CurrentComabtManager.StartCombat(_playerUnits, _opponentUnits);
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

        private void SendCallback(int code)
        {
            if(m_id == 0)
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

        private void SetWaitCallback(int waitCode, Action onReceived)
        {
            if(m_waitCallbackCode != -1)
            {
                throw new Exception("[PhotonManager][SetWaitCallback] Is waiting other code:" + waitCode);
            }

            m_waitCallbackCode = waitCode;
            m_nextStep = onReceived;
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