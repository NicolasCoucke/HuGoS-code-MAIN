using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
public class InitiateSpectator : MonoBehaviourPunCallbacks
{

    //[SerializeField]
    //private GameObject AccesCodeField;

    public bool IsSpectator = false;

    //[SerializeField]
    //GameObject canvas;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        string scene = SceneManager.GetActiveScene().name;
       // Debug.Log(scene);
        if (scene == "PreRoomLobbyScene")
        {
            if (IsSpectator == true)
                PhotonNetwork.NickName = "spectator";
            //Debug.Log("spectator");
            Destroy(this.gameObject);
        }
    }
}
