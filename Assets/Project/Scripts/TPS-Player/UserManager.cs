using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Cameras;
using UnityStandardAssets.Characters.ThirdPerson;
using Valve.VR;

namespace WS3
{
    public class UserManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        [SerializeField]
        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject UserMeInstance;

        public float pressDuration = 0.0f;

        [SerializeField]
        public int MaxHp;

        public bool isHTCuser;


        private GameObject kms_ui;
        

        private float DelayShoot;

        private bool canFire = true;

        [SerializeField] List<GameObject> SpawnPoint;

        [SerializeField] GameObject SpawnPointPool;

        /// <summary>
        /// Represents the GameObject on which to change the color for the local player
        /// </summary>
        public GameObject GameObjectLocalPlayerColor;

        /// <summary>
        /// The FreeLookCameraRig GameObject to configure for the UserMe
        /// </summary>
        GameObject goFreeLookCameraRig; 
        GameObject cameraVirusRig;


        #region Projectiles Spawn
        /// <summary>
        /// The Transform from which the snow ball is spawned
        /// </summary>
        [SerializeField] Transform ViralSpawner;
        /// <summary>
        /// The prefab to create when spawning
        /// </summary>
        [SerializeField] GameObject ViralPrefab;

        /*[Range(0.2f, 100.0f)] public float MinSpeed;
        [Range(0.2f, 100.0f)] public float MaxSpeed;
        [Range(0.2f, 100.0f)] public float MaxSpeedForPressDuration;*/


        [PunRPC]
        void ThrowCharge(Vector3 position, Vector3 directionAndSpeed, PhotonMessageInfo info)
        {
            float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

            // Instantiate the ViralCharge from the Charge Prefab at the position of the Spawner
            GameObject charge = Instantiate(ViralPrefab, position + directionAndSpeed * Mathf.Clamp(lag, 0, 1.0f), Quaternion.identity);
            charge.tag = tag;
            
            charge.GetComponent<Rigidbody>().velocity = directionAndSpeed;

            // Destroy the Charge after 3 seconds
            if(charge != null)
            {
                Destroy(charge, 3.0f);
            }
        }

        
        /*public void HitByViralCharge(string test)
        {            
            gameObject.GetComponent<Spawnable>().TakeDamage();
        }*/

        #endregion

        void Awake()
        {
            MaxHp = AppConfig.Inst.LifeNumber;

            if (photonView.IsMine)
            {
                Debug.LogFormat("Avatar UserMe created for userId {0}", photonView.ViewID);
                UserMeInstance = gameObject;

            }
        }

        // Start is called before the first frame update
        void Start()
        {
            DelayShoot = AppConfig.Inst.DelayShoot;

            Debug.Log("isLocalPlayer:" + UserMeInstance);

            updateGoFreeLookCameraRig();

            if (!isHTCuser)
            {
                followLocalPlayer();
                activateLocalPlayer();
            }
        }


        /// <summary>
        /// Get the GameObject of the CameraRig
        /// </summary>
        protected void updateGoFreeLookCameraRig()
        {
            //if (!photonView.IsMine) return;
            if (!UserMeInstance) return;
            try
            {
                goFreeLookCameraRig = GameObject.Find("FreeLookCameraRig");
                cameraVirusRig = GameObject.FindGameObjectWithTag("Virus Player");


                if (isHTCuser)
                {
                    goFreeLookCameraRig.SetActive(false);
                }
                else
                {
                    cameraVirusRig.SetActive(false);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Warning, no goFreeLookCameraRig found\n" + ex);
            }
        }

        /// <summary>
        /// Make the CameraRig following the LocalPlayer only.
        /// </summary>
        protected void followLocalPlayer()
        {
            if (photonView.IsMine)
            {
                if (goFreeLookCameraRig != null)
                {
                    Transform transformFollow = GameObject.FindGameObjectWithTag("KMS").transform.Find("EthanSkeleton").Find("EthanHips").transform;

                    goFreeLookCameraRig.GetComponent<FreeLookCam>().SetTarget(transformFollow);
                    Debug.Log("ThirdPersonControllerMultiuser follow:" + transformFollow);
                }
            }
        }

        protected void activateLocalPlayer()
        {
            GetComponent<ThirdPersonUserControl>().enabled = photonView.IsMine;
            GetComponent<Rigidbody>().isKinematic = !photonView.IsMine;
            
            
            if (photonView.IsMine)
            {
                try
                {
                    GameObjectLocalPlayerColor.GetComponent<Renderer>().material.color = Color.blue;
                    var kms_ui = UserMeInstance.transform.Find("UI");
                    kms_ui.gameObject.SetActive(true);
                }
                catch (System.Exception)
                {
                    Debug.Log("Glasses can't change colors, silly");
                }
            }

        }

        public IEnumerator shoot()
        {
            canFire = false;
            yield return new WaitForSeconds(DelayShoot);
            canFire = true;
        }


        // Update is called once per frame
        void Update()
        {
            if (!photonView.IsMine) return;

            if (!isHTCuser)
            {
                if (Input.GetButtonDown("Fire1") && canFire)
                {
                    photonView.RPC("ThrowCharge", RpcTarget.AllViaServer, ViralSpawner.position, 10.0f * ViralSpawner.forward);
                    StartCoroutine(shoot());
                }
            }

        }

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            return;
        }

        #endregion

    }
}