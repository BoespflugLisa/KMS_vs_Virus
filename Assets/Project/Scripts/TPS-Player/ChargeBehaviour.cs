using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WS3
{
    public class ChargeBehaviour : MonoBehaviour
    {
        private Color color_kms_charge; 

        // Start is called before the first frame update
        void Start()
        {
            color_kms_charge = AppConfig.Inst.ColorShotKMS;
            GetComponent<Renderer>().material.color = color_kms_charge;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnCollisionEnter(Collision collision)
        {
            var hit = collision.gameObject;
            Debug.Log("Charge hit Virus: " + hit);

            ShieldBehavior sb = hit.GetComponent<ShieldBehavior>();

            if(sb != null)
            {
                sb.HitByKMSCharge();
            }
            else{ 

                if(hit.CompareTag("Virus"))
                {
                    var vrm = hit.GetComponentInParent<VRManager>();
                    GameObject pv = vrm.virusPlayer;
                    Debug.Log("Get userManager : " + pv.name);
                    pv.GetComponent<Spawnable>().TakeDamage();
                    Destroy(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }

            }
            
        }


    }
}