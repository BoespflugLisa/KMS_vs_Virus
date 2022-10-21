using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    int score = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }


    [PunRPC]
    public void ScoreUpdate()
    {
        Debug.Log("Score : " + score);
        score += 1;

        if (score == 5)
        {
            PhotonNetwork.LoadLevel("ScoreScreen");
        }
    }
}
