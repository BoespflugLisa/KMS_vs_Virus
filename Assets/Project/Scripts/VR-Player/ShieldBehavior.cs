using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBehavior : MonoBehaviour
{

    [SerializeField] int MaxShieldPoint = 2;
    private int ShieldPoint;
    private Vector3 BaseScale;
    // Start is called before the first frame update

    private void Awake()
    {
        BaseScale = new Vector3((float)0.5, (float)0.01, (float)0.5);
        ShieldPoint = MaxShieldPoint;
    }
    void Start()
    {
        
    }
    IEnumerator RespawnSHield()
    {

        ShieldPoint = MaxShieldPoint;

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(5);


        transform.localScale = BaseScale;

    }

    // Update is called once per frame
    void Update()
    {
        if (this.ShieldPoint <= 0)
        {
            Debug.Log("I'm Here");
            transform.localScale = transform.localScale * (float)0;
            StartCoroutine(RespawnSHield());

            // PhotonNetwork.LeaveRoom();
        }
    }

    public void HitByKMSCharge()
    {
        //if (!photonView.IsMine) return;
        Debug.Log("Shield Hit " + ShieldPoint);
        ShieldPoint -= 1;
        ShieldUpdate();
    }

    public void ShieldUpdate()
    {

        transform.localScale = transform.localScale * (float)0.9;
    }
}
