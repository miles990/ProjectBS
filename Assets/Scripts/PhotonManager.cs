using Photon.Pun;
using Photon.Realtime;
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

        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void ConnectToLobby()
        {
            PhotonNetwork.GameVersion = "0.0.0";
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            PhotonNetwork.CreateRoom(System.Guid.NewGuid().ToString(), new RoomOptions { MaxPlayers = 2 });
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
            Log(JsonFx.Json.JsonWriter.Serialize(PlayerManager.Instance.Player.Characters[0]));
        }

        public void Log(string log)
        {
            m_photonView.RPC("Pun_Log", RpcTarget.All, m_id, log);
        }

        [PunRPC]
        private void Pun_Log(int id, string log)
        {
            Debug.LogFormat("PhotonID={0} log={1}", id, log);
        }
    }
}
