using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

public class ChatBox : MonoBehaviourPunCallbacks, IPunObservable
{

    //public Player player { get; private set };
    public GameObject chatPanel, TextObject;

    public InputField TextInputField;

    private List<Message> MessageList = new List<Message>();

    private bool IsTyping = false;

    public ScrollRect scrollRect;
    public RectTransform contenRectTransform;
    private Vector2 defaultPosition;
    private float maxScroll;
    public Scrollbar vertscroll;
     
    float Timer;



    void Start()
    {
        Timer = 0f;
        if(StaticVariables.IsSpectator)
        {

        
            if (StaticVariables.TextDefinedPath == false)
            {
                string thispath = StaticVariables.Path;
                thispath = thispath.Replace(".csv", string.Empty);
                StaticVariables.TextPath = thispath + "_chat.csv";
                StaticVariables.TextDefinedPath = true;
            
           

            }
            string DataToAdd = Timer.ToString() + "," + "TRIAL START";
            string path = StaticVariables.TextPath;
           // path = Path.Combine(Application.persistentDataPath, path);
            using (StreamWriter file = new StreamWriter(@path, true))
            {

                //write line all at once
                file.WriteLine(DataToAdd);
                //file.WriteLine("\n");
                //file.Close();
            }
        }

    }
     

    private void Update()
    {
        Timer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(IsTyping == false)
            {
                EventSystem.current.SetSelectedGameObject(TextInputField.gameObject);
                TextInputField.ActivateInputField();
                IsTyping = true;
            }
            else
            {
                string textInput = TextInputField.text;
                TextInputField.text = null;
                TextInputField.DeactivateInputField();
                string InputPlayer = " [" + (PhotonNetwork.LocalPlayer.ActorNumber-1).ToString() + "]";
                this.photonView.RPC("MessageToChat", RpcTarget.All,textInput, InputPlayer);
                IsTyping = false;
            }  
         
          
        }  
    }
       
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {


        if (stream.IsWriting)
        {

        } 
        else
        {
       
        }

         
    }

    [PunRPC]
    private void MessageToChat(string text, string player)
    {
        if (text != null)
        {

            Message newMessage = new Message();

            newMessage.sender = player;
            newMessage.text = text;


            GameObject newText = Instantiate(TextObject, chatPanel.transform);

            newMessage.TextObject = newText.GetComponent<Text>();

            newMessage.TextObject.text = string.Format(newMessage.TextObject.text, newMessage.sender, newMessage.text);

            MessageList.Add(newMessage);

            vertscroll.value = 0;
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f; 
            Canvas.ForceUpdateCanvases();


            if(StaticVariables.IsSpectator)
            {
                string DataToAdd = Timer.ToString() + "," + player + "," + text;
                string path = StaticVariables.TextPath;
                using (StreamWriter file = new StreamWriter(@path, true))
                {

                    //write line all at once
                    file.WriteLine(DataToAdd);
                    //file.WriteLine("\n");
                    //file.Close();
                }
            }
    

        }
    }

    public class Message
    {
        public string text;
        public string sender;
        public Text TextObject;
    }


}