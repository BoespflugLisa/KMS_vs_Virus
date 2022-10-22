using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Photon.Pun;


public class ViralChargeScript : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform viralSpawner;
    [SerializeField] private SteamVR_Action_Boolean m_shootAction;
    [SerializeField] private GameObject viralPrefab;
    [SerializeField] private int speed;

    private Color ColorShotVirus;


    private float DelayShoot;

    private bool canFire = true;


    private void Start()
    {
        DelayShoot = AppConfig.Inst.DelayShoot;
        ColorShotVirus = AppConfig.Inst.ColorShotVirus;
    }

    // Update is called once per frame
    void Update()
    {


        if (!photonView.IsMine) return;

        if (m_shootAction[SteamVR_Input_Sources.RightHand].stateDown && canFire)
        {
            photonView.RPC("ViralShot", RpcTarget.AllViaServer, viralSpawner.position, speed * viralSpawner.forward);
            canFire = false;
            StartCoroutine(ShootDelayCoroutine());
        }
    }

    IEnumerator ShootDelayCoroutine()
    {
        yield return new WaitForSeconds(DelayShoot);
        canFire = true;
    }

    [PunRPC]
    void ViralShot(Vector3 position, Vector3 directionAndSpeed, PhotonMessageInfo info)
    {
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
        GameObject charge = Instantiate(viralPrefab, position + directionAndSpeed * Mathf.Clamp(lag, 0, 1.0f), Quaternion.identity);

        Color colorCharge = ColorShotVirus;
        var material = charge.GetComponent<MeshRenderer>().material;
        var color = material.color;
        color.a = 1f;

        material.color = colorCharge;

        charge.GetComponent<Rigidbody>().velocity = directionAndSpeed;

        // Destroy the Charge after 3 seconds
        if (charge != null)
        {
            Destroy(charge, 3.0f);
        }

        charge.GetComponent<Rigidbody>().velocity = directionAndSpeed;
    }
}
