using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WS3;

public class ViralChargeBehaviour : MonoBehaviour
{
    private Color color_virus_charge;

    // Start is called before the first frame update
    void Start()
    {
        color_virus_charge = AppConfig.Inst.ColorShotVirus;
        GetComponent<Renderer>().material.color = color_virus_charge;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var hit = collision.gameObject;


        ShieldBehavior sb = hit.GetComponent<ShieldBehavior>();

        if (sb != null)
        {
            sb.HitByKMSCharge();
        }
        else
        {
            if (hit.CompareTag("KMS"))
            {
                //UserManager um = hit.GetComponent<UserManager>();
                hit.GetComponent<Spawnable>().TakeDamage();
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

    }
}
