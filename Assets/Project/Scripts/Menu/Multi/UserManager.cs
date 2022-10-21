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
    public class UserManager : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject UserMeInstance;

        public float pressDuration = 0.0f;

        [SerializeField]
        public int MaxHp;

        public bool isHTCuser;

        private int Health, previousHealth;
        //[SerializeField] public Image healthBar;

        private GameObject kms_ui;

        //private Color ColorShotKMS;
        

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
            // Tips for Photon lag compensation. Il faut compenser le temps de lag pour l'envoi du message.
            // donc décaler la position de départ de la balle dans la direction
            float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

            // Instantiate the Snowball from the Snowball Prefab at the position of the Spawner
            GameObject charge = Instantiate(ViralPrefab, position + directionAndSpeed * Mathf.Clamp(lag, 0, 1.0f), Quaternion.identity);
            charge.tag = tag;
            
            /*Color colorCharge = ColorShotKMS;
            var material = charge.GetComponent<MeshRenderer>().material;
            var color = material.color;
            color.a = 1f;

            material.color = colorCharge;*/
            
            charge.GetComponent<Rigidbody>().velocity = directionAndSpeed;

            // Destroy the Charge after 3 seconds
            if(charge != null)
            {
                Destroy(charge, 3.0f);
            }
        }

        
        public void HitByViralCharge(string test)
        {
            Debug.Log("HIT BY " + test + " CHARGE");
            var rb = GetComponent<Rigidbody>();
            //rb.AddForce((-transform.forward + (transform.up * 0.1f)) * 1.5f, ForceMode.Impulse);
            
            gameObject.GetComponent<Spawnable>().TakeDamage();
        }

        #endregion

        void Awake()
        {
            MaxHp = AppConfig.Inst.LifeNumber;

            if (photonView.IsMine)
            {
                Debug.LogFormat("Avatar UserMe created for userId {0}", photonView.ViewID);
                UserMeInstance = gameObject;

            }

            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            //DontDestroyOnLoad(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
            Health = MaxHp;
            previousHealth = MaxHp;
            DelayShoot = AppConfig.Inst.DelayShoot;
            //ColorShotKMS = AppConfig.Inst.ColorShotKMS;

            Debug.Log("isLocalPlayer:" + UserMeInstance);

            updateGoFreeLookCameraRig();

            if (!isHTCuser)
            {
                followLocalPlayer();
                activateLocalPlayer();
            }


            //HealthUpdate();

        }


       /* void HealthUpdate()
        {
            var material = healthBar.gameObject.GetComponent<Image>();
            float ratio = (float)Health / MaxHp;

            try
            {
                healthBar.fillAmount = Mathf.Clamp((float)Health / MaxHp, 0.0f, (float)AppConfig.Inst.LifeNumber);

                if (ratio > 0.75f)
                {
                    material.color = Color.green;
                }
                else if (ratio >= 0.25f && ratio <= 0.75f)
                {
                    material.color = Color.yellow;

                }
                else if (ratio < 0.25f)
                {
                    material.color = Color.red;
                }
            }
            catch (System.Exception)
            {
                Debug.Log("Can't change health bar color");
            }
        }*/

        

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
                    var vir_ui = UserMeInstance.transform.Find("SteamVRObjects/LeftHand/UI");
                    vir_ui.gameObject.SetActive(true);
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
                    /*var healthBarInnerParent = healthBar.transform.parent;
                    kms_ui = healthBarInnerParent.parent.gameObject;
                    kms_ui.SetActive(true);*/
                }
                catch (System.Exception)
                {
                    Debug.Log("Glasses can't change colors, silly");
                }
            }

        }

        public IEnumerator shoot()
        {
            //photonView.RPC("ThrowCharge", RpcTarget.AllViaServer, ViralSpawner.position, 10.0f * ViralSpawner.forward);
            canFire = false;
            yield return new WaitForSeconds(DelayShoot);
            canFire = true;
        }


        // Update is called once per frame
        void Update()
        {
            if (!photonView.IsMine) return;

            /*if (this.Health <= 0)
            {
                Respawn();

                ScoreManager sm = GameObject.Find("Score Manager").GetComponent<ScoreManager>();
                sm.ScoreUpdate();
            }*/

            if (!isHTCuser)
            {
                if (Input.GetButtonDown("Fire1") && canFire)
                {
                    photonView.RPC("ThrowCharge", RpcTarget.AllViaServer, ViralSpawner.position, 10.0f * ViralSpawner.forward);
                    StartCoroutine(shoot());
                }
            }

        }
/*
        void Respawn()
        {
            Health = MaxHp;
            previousHealth = 0;
            transform.position = SpawnPointPool.transform.position;

            HealthUpdate();

        }*/

        /*#region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(Health);
                
            }
            else
            {
                // Network player, receive data
                Health = (int)stream.ReceiveNext();
            }

            if (previousHealth != Health) { HealthUpdate(); }
        }

        #endregion*/

    }
}