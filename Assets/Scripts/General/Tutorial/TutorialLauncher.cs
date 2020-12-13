using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun.Demo.Cockpit;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System.IO;

//this file starts the next scene when the necessary conditions are met
public class TutorialLauncher : MonoBehaviourPunCallbacks, IPunObservable
{
    private int PlayerCount = 6;
    public int ReadyCount = 0;

    [SerializeField]
    private GameObject CountDownText;
    [SerializeField]
    private GameObject ReadyStartText;

    [SerializeField]
    private GameObject OverrideButton;

    [SerializeField]
    private GameObject PlayerNumberInput;

    [SerializeField]
    private GameObject StartOption;

    [SerializeField]
    private GameObject StartButton;

    [SerializeField]
    private GameObject RewardCodeInput;


    [SerializeField]
    private GameObject NextRound;

    [SerializeField]
    private GameObject ConditionDropdown;

    int Condition;







    bool IfStartPressed; 

    int _startOption;

    // Start is called before the first frame update
    public static TutorialLauncher instance;




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
    void Start()
    {
        IfStartPressed= false;
        _startOption = 0;
        //PlayerCount = 0;
        //ReadyCount = 0;

        if (PhotonNetwork.NickName == "spectator")
        {
            OverrideButton.SetActive(true);
        }
        else
        {
            OverrideButton.SetActive(false);
        }

        PlayerCount = 6;

            
    }

    public void ChangeStartOption()
    {
        _startOption = StartOption.GetComponent<Dropdown>().value;
        IfStartPressed = false;


    }

    // Update is called once per frame
    void Update()
    {
        //uncomment voor fixed aantal spelers

   


        bool spectatorPresent = false;
        foreach (Player player in PhotonNetwork.PlayerList)
            if (player.NickName == "spectator")
                spectatorPresent = true;

        if ((spectatorPresent == true && PhotonNetwork.NickName == "spectator"))
        {
            if(IfStartPressed)
            {
               // Debug.Log("playercount " + PlayerCount + "readycount " + ReadyCount);
                if (_startOption == 0)
                {
                    if (ReadyCount == PlayerCount)
                    {
                        Debug.Log("above_rpc");
                        this.photonView.RPC("StartCountDown", RpcTarget.All);
                        ReadyCount = 0;
                        IfStartPressed = false;
                    }
                }
                else
                {
                    this.photonView.RPC("StartCountDown", RpcTarget.All);
                    IfStartPressed = false;
                    StartButton.SetActive(false);
                }
            }
        }

        //start without a spectator
        if (spectatorPresent == false &&  PhotonNetwork.IsMasterClient)
        {
            if (ReadyCount == PlayerCount)
            {
                this.photonView.RPC("StartCountDown", RpcTarget.All);
                ReadyCount = 0;
            }
        }



        //PlayerCount = PhotonNetwork.PlayerList.Count();
        //Debug.Log("Readycount " + ReadyCount);
        //Debug.Log("Playercount " + PlayerCount);

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {



    }



    public void ApproveStart()
    {
        IfStartPressed = true;
        Debug.Log("startpressed");
        string _rewardCode = RewardCodeInput.GetComponent<InputField>().text;
        this.photonView.RPC("SaveRewardCode", RpcTarget.All, _rewardCode);

    }

    [PunRPC]
    public void SaveRewardCode(string _rewardCode)
    {

        StaticVariables.RewardCode = _rewardCode;

    }



    public void UpdatePlayerCount()
    {
        PlayerCount = int.Parse(PlayerNumberInput.GetComponent<InputField>().text);
        StaticVariables.MaxPlayers = PlayerCount;
        IfStartPressed = false;

    }

    public void ForceStartCountDown()
    {
        StartCountDown();
    }
    
    [PunRPC]
    public void StartCountDown()
    {
        StartCoroutine("UpdateCountDown");
        ReadyStartText.SetActive(false);

    }

    IEnumerator UpdateCountDown()
    {
        NextRound.SetActive(true);
        for (int t = 10; t > 0; t--)
        {
            yield return new WaitForSeconds(1);
            CountDownText.GetComponent<TMP_Text>().text = t.ToString();
        }
        if(PhotonNetwork.NickName == "spectator" || PhotonNetwork.IsMasterClient)
        {
            StartTutorial();
        }
        
    }

    public void StartTutorial()
    {
        if (PhotonNetwork.IsMasterClient)
        {

            string _rewardCode = RewardCodeInput.GetComponent<InputField>().text;
            this.photonView.RPC("SaveRewardCode", RpcTarget.All, _rewardCode);
            StartNoticeToFile();
            //PhotonNetwork.LoadLevel("EndScene");
            //PhotonNetwork.CurrentRoom.IsOpen = false;
            VerifyConditionChange();

            if (StaticVariables.Condition == 4)
            {
                PhotonNetwork.LoadLevel("BlockTutPart1");
            }
            else
            {
                PhotonNetwork.LoadLevel("TutorialPart1");

            }



            //PhotonNetwork.LoadLevel("EndScene");


        }

    }

    public void VerifyConditionChange()
    {
        Condition = ConditionDropdown.GetComponent<Dropdown>().value;
        photonView.RPC("VerifyChangeCondition", RpcTarget.All, Condition + 1);

    }

    [PunRPC]
    public void VerifyChangeCondition(int Condition)
    {
        StaticVariables.Condition = Condition;

    }

    void StartNoticeToFile()
    {


         //string filepath = BasicPath + "\\" + player_entry.Key + ".csv";
        string Path = StaticVariables.Path;
        using (StreamWriter file = new StreamWriter(@Path, true))
        {

            //write line all at once
            file.WriteLine("TutorialStart" + DateTime.Now.ToString("HH_mm_ss"));
            //file.WriteLine("\n");
            //file.Close();
        }
    }


}
