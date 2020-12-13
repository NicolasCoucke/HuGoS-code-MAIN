using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class KickFuntion : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    GameObject KickInputField;
    [SerializeField]
    GameObject KickButton;
    [SerializeField]
    private GameObject Disconnectedpannel;

    // Start is called before the first frame update
    void Start()
    {
        if(StaticVariables.IsSpectator)
        {
            KickInputField.SetActive(true);
            KickButton.SetActive(true);
            //InvokeRepeating("RemoveExtraPlayers", 5f, 5f);

        }
        else
        {
            KickInputField.SetActive(false);
            KickButton.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void KickPlayer()
    {
        int PlayerToKick = int.Parse(KickInputField.GetComponent<InputField>().text);
        //PhotonNetwork.CloseConnection(PhotonNetwork.PlayerList[PlayerToKick]);
        photonView.RPC("KickPlayerSide", RpcTarget.All, PlayerToKick);
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }


    [PunRPC]
    public void KickPlayerSide(int PlayerToKick)
    {
        if (PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[PlayerToKick])
        {
            Disconnectedpannel.SetActive(true);
            PhotonNetwork.Disconnect();
        }


    }


    void RemoveExtraPlayers()
    {
        int countPlayers = PhotonNetwork.CountOfPlayersInRooms-1;
        if (countPlayers > StaticVariables.MaxPlayers)
        {
            photonView.RPC("KickPlayerSide", RpcTarget.All, countPlayers);
        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
      

    }
}
 