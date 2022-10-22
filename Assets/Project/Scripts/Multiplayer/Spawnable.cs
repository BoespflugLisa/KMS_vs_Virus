using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using WS3;

public class Spawnable : MonoBehaviourPunCallbacks, IPunObservable
{
    private int Health, previousHealth;

    [SerializeField] public Image healthBar;
    [SerializeField] public int MaxHp;


    [SerializeField] GameObject SpawnPointPool;

    private GameObject virusPlayer;
    [HideInInspector] public bool isHtcUser;
    private float healthVirus;


   
    private void Awake()
    {
        MaxHp = AppConfig.Inst.LifeNumber;
                
    }

    private void Start()
    {
        var vrm = gameObject.GetComponent<VRManager>();
        if (vrm != null)
        {
            isHtcUser = true;
            virusPlayer = vrm.virusPlayer;
        }        
        
        Health = MaxHp;
        previousHealth = MaxHp;
        
        HealthUpdate();
    }

    void Update()
    {

        if (this.Health <= 0)
        {
            Respawn();

            ScoreManager sm = GameObject.Find("Score Manager").GetComponent<ScoreManager>();
            sm.ScoreUpdate();
        }
    }

    void HealthUpdate()
    {
        Image material;
        if (!isHtcUser)
        {
            material = healthBar;
        }
        else
        {
            material = virusPlayer.GetComponent<Spawnable>().healthBar;
            virusPlayer.GetComponent<Spawnable>().Health = Health;
        }

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
    }


    void Respawn()
    {
        Health = MaxHp;
        previousHealth = 0;
        transform.position = SpawnPointPool.transform.position;

        HealthUpdate();

    }
    public void TakeDamage()
    {
        previousHealth = Health;
        Health -= 1;
        HealthUpdate();
    }


    #region IPunObservable implementation

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

    #endregion
}
