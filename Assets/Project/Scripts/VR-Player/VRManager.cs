using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Photon.Pun;
using UnityEngine.UI;

namespace WS3
{
    public class VRManager : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject UserMeInstance;

        [SerializeField] private Transform viralSpawner;
        [SerializeField] private SteamVR_Action_Boolean m_shootAction;
        [SerializeField] private GameObject viralPrefab;
        [SerializeField] private int speed;

        GameObject goFreeLookCameraRig;

        private float DelayShoot;

        private bool canFire = true;

        [SerializeField] GameObject SpawnPointPool;

        [HideInInspector] public GameObject virusPlayer;

        private void Awake()
        {

            if (photonView.IsMine)
            {
                Debug.LogFormat("Avatar UserMe created for userId {0}", photonView.ViewID);
                UserMeInstance = gameObject;
                virusPlayer = GameObject.FindGameObjectWithTag("Virus Player");

            }
        }

        private void Start()
        {
            DelayShoot = AppConfig.Inst.DelayShoot;
            updateGoFreeLookCameraRig();
        }

        // Update is called once per frame
        void Update()
        {

            if (!photonView.IsMine) return;

            if (m_shootAction[SteamVR_Input_Sources.RightHand].stateDown && canFire)
            {
                photonView.RPC("ThrowViralCharge", RpcTarget.AllViaServer, viralSpawner.position, speed * viralSpawner.forward);
                canFire = false;
                StartCoroutine(ShootDelayCoroutine());
            }
        }

        protected void updateGoFreeLookCameraRig()
        {
            if (!photonView.IsMine) return;
            try
            {
                goFreeLookCameraRig = GameObject.Find("FreeLookCameraRig");
                goFreeLookCameraRig.SetActive(false);

            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Warning, no goFreeLookCameraRig found\n" + ex);
            }
        }


        IEnumerator ShootDelayCoroutine()
        {
            yield return new WaitForSeconds(DelayShoot);
            canFire = true;
        }

        [PunRPC]
        void ThrowViralCharge(Vector3 position, Vector3 directionAndSpeed, PhotonMessageInfo info)
        {
            float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
            GameObject charge = Instantiate(viralPrefab, position + directionAndSpeed * Mathf.Clamp(lag, 0, 1.0f), Quaternion.identity);

            charge.GetComponent<Rigidbody>().velocity = directionAndSpeed;

            // Destroy the Charge after 3 seconds
            if (charge != null)
            {
                Destroy(charge, 3.0f);
            }
        }

    }
}

