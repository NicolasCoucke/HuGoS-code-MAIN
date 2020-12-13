using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine;
using System.IO;

// this file keeps track of players in the lobby
public class PreLobbyWork : MonoBehaviourPunCallbacks, IPunObservable
{

    [SerializeField]
    private GameObject PlayerMenu;

    [SerializeField]
    private GameObject KickInputField;

    [SerializeField]
    private GameObject ConsentPanel;
    [SerializeField]
    private GameObject QuestionPanel;
    [SerializeField]
    private GameObject QuestionPanel2;
    [SerializeField]
    private GameObject StartText;

    [SerializeField]
    private GameObject RewardCodeInput;

    [SerializeField]
    private GameObject SpectatorPanel;

    [SerializeField]
    private GameObject ConditionDropdown;


    [SerializeField]
    private GameObject MainCanvas;
    [SerializeField]
    private GameObject Disconnectedpannel;

    int Condition;

    int Beatcounter;

    private static int Playercount = 0;
    private static int Readycount = 0;

    public static string BasicPath;
    public static string path;
    public static bool _definePath = false;

    public static PreLobbyWork instance;

    private void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(instance);
                instance = this;
            }

        }
        else
        {
            instance = this;
        }

    }
    private void Start()
    {
        Condition = 1;

        if (StaticVariables.IsSpectator == true)
        {
            Debug.Log("staticvar " + StaticVariables.IsSpectator);
            PhotonNetwork.NickName = "spectator";
            ConsentPanel.SetActive(false);
            QuestionPanel.SetActive(false);
            QuestionPanel2.SetActive(false);
            SpectatorPanel.SetActive(true);


            InvokeRepeating("SendHeartbeat", 0f, 5f);
            
        }
        else
        {

            PhotonNetwork.NickName = "Player " + (PhotonNetwork.LocalPlayer.ActorNumber - 1);
            ConsentPanel.SetActive(true);
            QuestionPanel.SetActive(false);
            StartText.SetActive(false);
            SpectatorPanel.SetActive(false);

            InvokeRepeating("BeatCountDown", 0f, 1f);
            Beatcounter = 30;
        }

        LocalStart();
    }

    void LocalStart()
    {
        if (StaticVariables.IsSpectator)
        {
            Debug.Log("pathdefine");
            if (StaticVariables.DefinedPath == false)
            {
                DefinePath();
            }
            else
            {
                path = StaticVariables.Path;
            }
        }


    }

    // this function checks whether the player remains connected to the experimenter via the server
    void BeatCountDown()
    {
        Beatcounter -= 1;
        if(Beatcounter < 2)
        {
            MainCanvas.SetActive(false);
            Disconnectedpannel.SetActive(true);
            PhotonNetwork.Disconnect();
        }
    }

    void SendHeartbeat()
    {
        photonView.RPC("HeartBeat", RpcTarget.All);
    }

    [PunRPC]
    void HeartBeat()
    {
        Beatcounter += 5;
    }


    public static void DefinePath()
    {

        string BasicPath = @"C:\Users\Administrator\Documents\GAMEDATA";
        string DataString = DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss");
        path = BasicPath + "\\" + DataString + ".csv";

        StaticVariables.Path = path;
        Debug.Log(StaticVariables.Path);

        StaticVariables.DefinedPath = true;


        using (StreamWriter file = new StreamWriter(@path, true))
        {
            string Condition = "condition " + StaticVariables.Condition.ToString();
            file.WriteLine(Condition);
        }


    }


    private void Update()
    {
     
    }


    public void ConditionChange()
    {
        Condition = ConditionDropdown.GetComponent<Dropdown>().value;
        Debug.Log(Condition);
        photonView.RPC("ChangeCondition", RpcTarget.All, Condition+1); 



    }

    public void KickPlayer()
    {
      int PlayerToKick = int.Parse(KickInputField.GetComponent<InputField>().text);
      //PhotonNetwork.CloseConnection(PhotonNetwork.PlayerList[PlayerToKick]);
      photonView.RPC("KickPlayerSide", RpcTarget.All, PlayerToKick);
    }


    [PunRPC]
    public void KickPlayerSide(int PlayerToKick)
    {
        if(PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[PlayerToKick])
        {
            MainCanvas.SetActive(false);
            Disconnectedpannel.SetActive(true);
            PhotonNetwork.Disconnect();
        }
       
     
    }

    [PunRPC]
    public void ChangeCondition(int Condition)
    {
        StaticVariables.Condition = Condition;

    }

    public void StartPanelButton()
    {
        SpectatorPanel.SetActive(false);
        StartText.SetActive(true);

    }

    public void Accept()
    {
        ConsentPanel.SetActive(false);
        QuestionPanel.SetActive(true);
    }

    public void Next1()
    {
        QuestionPanel.SetActive(false);
        QuestionPanel2.SetActive(true);
    }

    public void Next2()
    {
        QuestionPanel2.SetActive(false);
        StartText.SetActive(true);
        photonView.RPC("UpdateReadyCount", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);

    }

    [PunRPC]
    public void UpdateReadyCount(int actornumber)
    {
        Debug.Log("Updatecount");
        TutorialLauncher.instance.ReadyCount += 1;

        if(StaticVariables.IsSpectator)
        {
            PlayerMenu.GetComponent<PlayerListingMenu>().MakePlayerReady(actornumber);
        }

    }



    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {
            // We own this player: send the others our data

            stream.SendNext(Playercount);
            stream.SendNext(Readycount);
            // stream.SendNext(Health);
        }
        else
        {
            // Network player, receive data
            Playercount = (int)stream.ReceiveNext();
            Readycount = (int)stream.ReceiveNext();

            //this.Health = (float)stream.ReceiveNext();
        }


    }





}