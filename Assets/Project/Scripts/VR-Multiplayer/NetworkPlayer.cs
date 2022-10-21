using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using Valve.VR;

public class NetworkPlayer : MonoBehaviour
{
    

    public Transform head; 
    public Transform rightHand;
    public Transform leftHand;
    public Transform body;


    public Animator leftHandAnimator;
    public Animator rightHandAnimator;

    private PhotonView photonView;

    private Transform headRig;
    private Transform leftHandRig;
    private Transform rightHandRig;
    private Transform bodyRig;


    private Transform virusPlayer;


    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();


        virusPlayer = GameObject.FindGameObjectWithTag("Virus Player").transform;

        Debug.Log("Player Virus : " + virusPlayer.name);

        headRig = virusPlayer.Find("SteamVRObjects/VRCamera");
        leftHandRig = virusPlayer.Find("SteamVRObjects/LeftHand");
        rightHandRig = virusPlayer.Find("SteamVRObjects/RightHand");
        bodyRig = virusPlayer.Find("Body");

        if (photonView.IsMine)
        {
            foreach (var item in GetComponentsInChildren<Renderer>())
            {
                item.enabled = false;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            
            MapPosition(head, headRig);
            MapPosition(leftHand, leftHandRig);
            MapPosition(rightHand, rightHandRig);
            MapPosition(body, bodyRig);
                           
        }
    }


    void MapPosition(Transform target, Transform virusPlayerTransform)
    {
       
        target.position = virusPlayerTransform.position;
        target.rotation = virusPlayerTransform.rotation;
    }

}
