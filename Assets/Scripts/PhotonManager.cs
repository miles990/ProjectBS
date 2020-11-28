using Photon.Pun;
using UnityEngine;

namespace ProjectBS.Network
{
    public class PhotonManager : MonoBehaviourPunCallbacks
    {
        public static PhotonManager Instance { get; private set; }

        public PhotonView PhotonView { get { return m_photonView; } }
        [SerializeField] private PhotonView m_photonView = null;

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
            Debug.Log("OnConnectedToMaster");
        }
    }
}
