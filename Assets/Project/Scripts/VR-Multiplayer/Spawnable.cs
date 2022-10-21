using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawnable : MonoBehaviourPunCallbacks, IPunObservable
{
    private int Health, previousHealth;

    [SerializeField] public Image healthBar;
    [SerializeField] public int MaxHp;

    [SerializeField] GameObject SpawnPointPool;

    private void Awake()
    {
        MaxHp = AppConfig.Inst.LifeNumber;
                
    }

    private void Start()
    {

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
        var material = healthBar;//.GetComponent<Image>();//.gameObject.GetComponent<Image>();
        Debug.Log(healthBar);
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
        Debug.Log("Color HealthBAR : " + material.color);
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
        Debug.Log("J'ai pris des dégats");
        previousHealth = Health;
        Debug.Log("LIFE POINT Before shoot : " + Health);
        Health -= 1;
        Debug.Log("LIFE POINT After Shoot : " + Health);
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
