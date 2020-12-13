using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class EndLobbyWork : MonoBehaviourPunCallbacks, IPunObservable
{
    //Toggle Variables

    [SerializeField]
    private GameObject Question0Toggle;

    [SerializeField]
    private GameObject Question1Toggle;
    [SerializeField]
    private GameObject Question2Toggle;

    [SerializeField]
    private GameObject Question21Dropdown;

    [SerializeField]
    private GameObject Question3Toggle;
    [SerializeField]
    private GameObject Question4Toggle;

    [SerializeField]
    private GameObject Question41Dropdown;

    [SerializeField]
    private GameObject Question5Toggle;
    [SerializeField]
    private GameObject Question6Toggle;
    [SerializeField]
    private GameObject Question7Toggle;
    [SerializeField]
    private GameObject Question8Toggle;

    [SerializeField]
    private GameObject StrategyInput;
    [SerializeField]
    private GameObject BetterInput;



    [SerializeField]
    private GameObject LinkInputField;

    [SerializeField]
    private GameObject QuestionPage1;
    [SerializeField]
    private GameObject QuestionPage2;
    [SerializeField]
    private GameObject QuestionPage3;

    [SerializeField]
    private GameObject CodeInputField;




    // insert bug questions

    [SerializeField]
    private GameObject GoodbyeText;

    //Answer variables
    private Boolean question0;
    private Boolean question1;
    private Boolean question2;
    private int question21;
    private Boolean question3;
    private Boolean question4;
    private int question41;

    private Boolean question5;
    private Boolean question6;
    private Boolean question7;
    private Boolean question8;


    public string StrategyText = "0";
    public string BetterText = "0";


    public static EndLobbyWork instance;

    public string QuestionString = "0";




    private void Awake()
    {
        StaticVariables.DoReconnect = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
        //Playercount = PhotonNetwork.CountOfPlayers;
       // QuestionPage1.SetActive(true);
        QuestionPage2.SetActive(false);


        LinkInputField.GetComponent<InputField>().text = "https://docs.google.com/forms/d/e/1FAIpQLSe6xHVhDCpwgt-pc79G7v4cgT96J22smORxb6BTI36f147jvA/viewform?usp=sf_link";
        StartCoroutine("Endlimit");

        //StartCoroutine("KickOutPlayerv2");
    }

    IEnumerator Endlimit()
    {
        yield return new WaitForSeconds(300);
        photonView.RPC("KickEveryone", RpcTarget.All);
       
    }

    [PunRPC]
    void KickEveryone()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void Next()
    {
        QuestionPage1.SetActive(false);
        QuestionPage2.SetActive(true);
    }

    public void Next2()
    {
        QuestionPage1.SetActive(false);
        QuestionPage2.SetActive(false);
        QuestionPage3.SetActive(true);

    }

    public void FinishButton()
    {
        QuestionPage1.SetActive(false);
        QuestionPage2.SetActive(false);
        QuestionPage3.SetActive(false);
        GoodbyeText.SetActive(true);
        CodeInputField.SetActive(true);
        CodeInputField.GetComponentInChildren<InputField>().text = StaticVariables.RewardCode; //"50AD7E13";
       // SendEvaluationData();
        StartCoroutine("KickOutPlayerv2");

        
    }



    private void Update()
    {
        question0 = Question0Toggle.GetComponent<Toggle>().isOn;
        question1 = Question1Toggle.GetComponent<Toggle>().isOn;
        question2 = Question2Toggle.GetComponent<Toggle>().isOn;
        question21 = Question21Dropdown.GetComponent<Dropdown>().value;
        question3 = Question3Toggle.GetComponent<Toggle>().isOn;
        question4 = Question4Toggle.GetComponent<Toggle>().isOn;
        question41 = Question41Dropdown.GetComponent<Dropdown>().value;

        question5 = Question5Toggle.GetComponent<Toggle>().isOn;
        question6 = Question6Toggle.GetComponent<Toggle>().isOn;
        question7 = Question7Toggle.GetComponent<Toggle>().isOn;
        question8 = Question8Toggle.GetComponent<Toggle>().isOn;

        StrategyText = StrategyInput.GetComponent<InputField>().text;
        BetterText = BetterInput.GetComponent<InputField>().text;

        QuestionString = "P" + (PhotonNetwork.LocalPlayer.ActorNumber - 1).ToString() + "," + Convert.ToInt32(question0).ToString() + Convert.ToInt32(question1).ToString() + "_" + Convert.ToInt32(question2).ToString() + "_" + question21.ToString() + "_" + Convert.ToInt32(question3).ToString() + "_" + Convert.ToInt32(question4).ToString() + "_" + question41.ToString() + "_" + Convert.ToInt32(question5).ToString() + "_" + Convert.ToInt32(question6).ToString() + "_" + Convert.ToInt32(question7).ToString() + "_" + Convert.ToInt32(question8).ToString() + "_" + StrategyText + "_" + BetterText; ;

    }



    public void SendQuestionnaireDataLocal()
    {


        //use this space to add player info to data array
        //if(this.photonView.IsMine)
        //{
        this.photonView.RPC("UpdateQuestionnaire", RpcTarget.All, QuestionString);

        //}




    }

    [PunRPC]
    public void UpdateQuestionnaire(string _questionString)
    {
        Debug.Log("questionstring 1 = " + QuestionString);

        QuestionString = _questionString;

        if (StaticVariables.IsSpectator)
        {
            SaveQuestionnaireData();
        }

    }

    public void SaveQuestionnaireData()
    {
        Debug.Log("questionstring = " + QuestionString);
        string Path = StaticVariables.Path;
        using (StreamWriter file = new StreamWriter(@Path, true))
        {

            //write line all at once
            file.WriteLine(QuestionString);
            //file.Close();
        }
    }

    //public void RemarkChange()
    //{
    //    remarks = InputRemarks.GetComponent<InputField>().text;
    //}



    IEnumerator KickOutPlayer()
    {
        yield return new WaitForSeconds(2);
        SendEvaluationData();
        yield return new WaitForSeconds(10);
        SendEvaluationData();
        yield return new WaitForSeconds(20);
        PhotonNetwork.LeaveRoom();


    }

    IEnumerator KickOutPlayerv2()
    {
        SendQuestionnaireDataLocal();
        yield return new WaitForSeconds(100);
        PhotonNetwork.LeaveRoom();


    }

    public void SendEvaluationData()
    {

        //use this space to add player info to data array

        
        List<string> StringList = new List<string>();
        StringList.Add(QuestionString);
        //StringList.Add(remarks);
        //PhotonView[] View_array = PhotonNetwork.PhotonViews;
        //foreach (PhotonView view in View_array)
        //{


        //    if (!view.IsMine)
        //    {
        //        GameObject Player_object = view.gameObject;//

        //        if (Player_object.tag == "Player")
        //        {
        //            StringList.Add("Player " + view.OwnerActorNr.ToString()); 
        //            StringList.Add(Player_object.GetComponent<EndLobbyWork>().QuestionString);
        //            StringList.Add(Player_object.GetComponent<EndLobbyWork>().remarks);
        //        }
        //    }

        //}


        string[] GameArray = StringList.ToArray();
        byte evCode = 1; // Custom Event 1: Used as "MoveUnitsToTargetPosition" event
        //object[] content = new object[] { "testing" };
        object[] content = new object[] { GameArray }; // Array contains the target position and the IDs of the selected units
        var flags = new WebFlags(WebFlags.HttpForwardConst);
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All, Flags = flags }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {


    }

}