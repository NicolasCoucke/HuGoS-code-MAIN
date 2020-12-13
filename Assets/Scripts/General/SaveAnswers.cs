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
using TMPro;

public class SaveAnswers : MonoBehaviourPunCallbacks, IPunObservable
{

    public GameObject Sex;
    public GameObject Age;
    public GameObject Experience;

    public GameObject PleaseFill;

    string sex;
    string age = "0";
    string experience;

    public GameObject Slider1;
    public GameObject Slider2;
    public GameObject Slider3;
    public GameObject Slider4;
    public GameObject Slider5;
   // public GameObject Slider6;

    private float Answer1;
    private float Answer2;
    private float Answer3;
    private float Answer4;
    private float Answer5;

    string QuestionString;
    //  private float Answer6;



    // Start is called before the first frame update
    void Start()
    {
        //store once for if they don't change
        SaveSex();
        SaveExperience();
    }

    // Update is called once per frame
    void Update()
    {
        Answer1 = Slider1.GetComponent<Slider>().value;
        Answer2 = Slider2.GetComponent<Slider>().value;
        Answer3 = Slider3.GetComponent<Slider>().value;
        Answer4 = Slider4.GetComponent<Slider>().value;
        Answer5 = Slider5.GetComponent<Slider>().value;
        //Answer6 = Slider6.GetComponent<Slider>().value;
        SaveSex();
        SaveAge();
        SaveExperience();

    }


    public void SaveSex()
    {
        TMP_Dropdown dropdown = Sex.GetComponent<TMP_Dropdown>();
        sex = dropdown.options[dropdown.value].text;
    }

    public void SaveAge()
    {
        age = Age.GetComponent<TMP_InputField>().text;
    }

    public void SaveExperience()
    {
        TMP_Dropdown dropdown = Experience.GetComponent<TMP_Dropdown>();
        experience = dropdown.options[dropdown.value].text;
    }

    public void Clicknext1()
    {
        //check if everthing is filled in
        if(int.Parse(age) != 0)
        {
            //nextpage
            PreLobbyWork.instance.Next1();
            Debug.Log(sex + " " + age + " " + experience);
           
        }
        else
        {
            //setactive fill in
            PleaseFill.SetActive(true);
        }

    }

    public void Clicknext2()
    {
       
            //nextpage
            PreLobbyWork.instance.Next2();
            Debug.Log(sex + " " + age + " " + experience);
            //SendQuestionnaireData();
            SendQuestionnaireDataLocal();

    }

    public void SendQuestionnaireDataLocal()
    {


        //use this space to add player info to data array
        //if(this.photonView.IsMine)
        //{
            QuestionString = DateTime.Now.ToString("HH_mm_ss") + "," + PhotonNetwork.NickName + "," + "P" + (PhotonNetwork.LocalPlayer.ActorNumber - 1).ToString() + "," + StaticVariables.ProlificID + "," + sex + "," + age + "," + experience + "," + Answer1.ToString() + "_" + Answer2.ToString() + "_" + Answer3.ToString() + "_" + Answer4.ToString() + "_" + Answer5.ToString(); //+ "_" + Answer6.ToString();
            this.photonView.RPC("UpdateQuestionnaire", RpcTarget.All, QuestionString);

        //}




    }

   [PunRPC]
   public void UpdateQuestionnaire(string _questionString)
    {
        Debug.Log("questionstring 1 = " + _questionString);

        QuestionString = _questionString;
       
        if (StaticVariables.IsSpectator)
        {
            SaveQuestionnaireData();
        }

    }

    public void SaveQuestionnaireData()
    {
        Debug.Log("questionstring = " +  QuestionString);
        string Path = StaticVariables.Path;
        using (StreamWriter file = new StreamWriter(@Path, true))
        {

            //write line all at once
            file.WriteLine(QuestionString);
            //file.WriteLine("\n");
            //file.Close();
        }
    }

    public void SendQuestionnaireData()
    {
         
        //use this space to add player info to data array
        string QuestionString = "P" + (this.photonView.OwnerActorNr - 1).ToString() + sex + "," + age + "," + experience + "," + Answer1.ToString() + "_" + Answer2.ToString() + "_" + Answer3.ToString() + "_" + Answer4.ToString() + "_" + Answer5.ToString(); //+ "_" + Answer6.ToString();
        List<string> StringList = new List<string>();
        StringList.Add(QuestionString);
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
