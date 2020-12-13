using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class DominancePlayer : MonoBehaviourPun
{

    [SerializeField]
    private GameObject crown;

    [SerializeField]
    private Camera camera;


    private Transform selectedPlayer;


    private int me = 0;

    private bool CanActivate = true;
    private bool IsActive = false;


    private void Start()
    {

        crown.SetActive(false);
        

    }

    void Update()
    {
        //zorgt ervoor dat je enkel gaat veranderen als het jou player is
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }



      
        if (Input.GetKeyDown(KeyCode.Space) && CanActivate == true)
        {
            if (crown.activeSelf)
            {
                photonView.RPC("CrownChange", RpcTarget.All, false);
                StartCoroutine("CrownDelay");
                //IsActive = false;
                //LavaSpawner.toggleLeadingText(false);
            }
            else
            {
                photonView.RPC("CrownChange", RpcTarget.All, true);
                //IsActive = true;
                // LavaSpawner.toggleLeadingText(true);
            }

        }
       


     


    }

    
    [PunRPC]
    void CrownChange(bool onof)
    {
        crown.SetActive(onof);
        this.GetComponent<PlayerManager>().dominanceON = onof;
        //if (IsActive)
        //    this.GetComponent<PlayerManager>().dominanceON = true;
        //else
        //    this.GetComponent<PlayerManager>().dominanceON = false;
    }

    IEnumerator CrownDelay()
    {
        CanActivate = false;
        yield return new WaitForSeconds(4);
        CanActivate = true;
    }

}