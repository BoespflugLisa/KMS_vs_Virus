using System;
using System.Collections;


using UnityEngine;
using UnityEngine.SceneManagement;


using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

namespace WS3
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        private GameObject spawnedPlayerPrefab;

        public static NetworkManager Instance;

        GameObject goFreeLookCameraRig;


        [Tooltip("The prefab to use for representing the user on a PC. Must be in Resources folder")]
        public GameObject playerPrefabPC;

        [Tooltip("The prefab to use for representing the user in VR. Must be in Resources folder")]
        public GameObject playerPrefabVR;

        [SerializeField] List<GameObject> SpawnPoint;

        [SerializeField] GameObject SpawnPointPool;


        #region Photon Callbacks


        /// <summary>
        /// Called when the local player left the room. 
        /// </summary>
        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            PhotonNetwork.Destroy(spawnedPlayerPrefab);

        }
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            /*if(UserDeviceManager.GetDeviceUsed() == UserDeviceType.HTC)
            {
                spawnedPlayerPrefab = PhotonNetwork.Instantiate("Prefabs/Network Virus Player", transform.position, transform.rotation);
            }*/
        }

        /// <summary>
        /// Called when Other Player enters the room and Only other players
        /// </summary>
        /// <param name="other"></param>
        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting
            // TODO: 

        }

        /// <summary>
        /// Called when Other Player leaves the room and Only other players
        /// </summary>
        /// <param name="other"></param>
        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
            // TODO: 
        }
        #endregion


        #region Public Methods

        /// <summary>
        /// Our own function to implement for leaving the Room
        /// </summary>
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        private void updatePlayerNumberUI()
        {
            // TODO: Update the playerNumberUI

        }

        /*protected void updateGoFreeLookCameraRig()
        {
            if (!photonView.IsMine) return;
            try
            {
                goFreeLookCameraRig = GameObject.Find("FreeLookCameraRig");
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Warning, no goFreeLookCameraRig found\n" + ex);
            }
        }*/

        void Start()
        {
            Instance = this;
            GameObject playerprefab = UserDeviceManager.GetPrefabToSpawnWithDeviceUsed(playerPrefabPC, playerPrefabVR);
            

            if (playerprefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if (UserManager.UserMeInstance == null)
                {
                    var playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

                    //Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    Vector3 initialPos = UserDeviceManager.GetDeviceUsed() == UserDeviceType.HTC ? new Vector3(0f, 0.5f, 0f) : new Vector3(0f, 0f, 0f);
                    Debug.Log("A new player has been instantiated with the prefab "+ playerprefab.name);
                    PhotonNetwork.Instantiate("Prefabs/" + playerprefab.name, SpawnPoint[playerCount - 1].transform.position, Quaternion.identity);
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }

            }


        }

        private void Update()
        {
            // Code to leave the room by pressing CTRL + the Leave button
            if (Input.GetButtonUp("Leave") && Input.GetKeyDown(KeyCode.LeftShift))
            {
                Debug.Log("Leave event");
                LeaveRoom();
            }
        }
        #endregion
    }
}