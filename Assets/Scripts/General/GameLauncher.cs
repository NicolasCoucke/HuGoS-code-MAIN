using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;

public class GameLauncher : MonoBehaviourPunCallbacks
{

    [SerializeField]
    private int Number_of_games = 6;

    [SerializeField]
    private GameObject endsession;

    [SerializeField]
    private GameObject CountDownText;

    [SerializeField]
    private GameObject RoomCanvas;

    private int WaitTime;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (ScoreMenu.Gamecounter > Number_of_games)
        {
            endsession.SetActive(true);
            
        }
        else
        { 
            StartCoroutine("UpdateCountDown");
        }

      
    }


    IEnumerator UpdateCountDown()
    {
        for(int t = 10; t>0;t--)
        {
            yield return new WaitForSeconds(1);
            CountDownText.GetComponent<TMP_Text>().text = t.ToString();
        }
        StartGame();
    }

    public void StartGame()
    {

        RoomCanvas.SetActive(false);
        if (PhotonNetwork.IsMasterClient)
        { 
            if(StaticVariables.Condition == 4)
            {
                PhotonNetwork.LoadLevel("BlockScene");// + PhotonNetwork.CurrentRoom.PlayerCount);

            }
            else
            {
                PhotonNetwork.LoadLevel("PlayScene");// + PhotonNetwork.CurrentRoom.PlayerCount);

            }

        }
        else
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        }
    }

    public void LeaveRoom()
    {
        //PhotonNetwork.LoadLevel("Launcher");
        PhotonNetwork.LeaveRoom();
        //Destroy(ScoreMenu.instance._canvas.gameObject);
        
    }

 
}
