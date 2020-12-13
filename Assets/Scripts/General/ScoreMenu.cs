using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Security.Cryptography;

public class ScoreMenu : MonoBehaviourPunCallbacks
{

    //public Player player { get; private set };
    [SerializeField]
    private Transform _content;
    [SerializeField]
    public  ScoreListing _scoreListing;

    public static ScoreMenu instance;
    [SerializeField]
    public Canvas _canvas;

    [SerializeField]
    public GameObject ScoreListPanel;

    [SerializeField]
    public GameObject ScoreNotice;

    public Toggle azerty;
    public Toggle qwerty;
    public static bool Keyboard_variable = false;

    public static int Gamecounter = 0;

    public static Dictionary<string, float> ScoreDic = new Dictionary<string, float>();
    public static List<string> ScoreList = new List<string>();
    private List<ScoreListing> ScoreListingList = new List<ScoreListing>();

    static float score = 0;
    static float maxScore = 0;

    private void Awake()
    {
        if (instance != null)
        {
            if(instance != this)
            {
                Destroy(this);
                //instance = this;
                //DontDestroyOnLoad(_canvas.gameObject);
            }
           
        }
        else
        {
            instance = this;
            //DontDestroyOnLoad(_canvas.gameObject);
        }

    }

    private void Start()
    {

        // Debug.Log("score size + " + ScoreList.Count);
        if (Gamecounter == 0)
        {
            ScoreNotice.SetActive(true);
            ScoreListPanel.SetActive(false);
        }
        else
        {
            ScoreNotice.SetActive(false);
            ScoreListPanel.SetActive(true);


            if(StaticVariables.Condition==4)
            {
                score = BLOCKLavaSpawner.Total_Score;
                maxScore = BLOCKLavaSpawner.Max_Total_Score;
            }
            else
            {
                score = LavaSpawnerv2.Total_Score;
                maxScore = LavaSpawnerv2.Max_Total_Score;
            }
            Debug.Log("update score ");
            

            ScoreDic.Add("Game " + Gamecounter.ToString(), Mathf.Round(score));
            ScoreList.Add("Game " + Gamecounter.ToString() + ":   " + Mathf.Round(score).ToString() + " out of " + Mathf.Round(maxScore).ToString());
            

            for (int i = 0; i < ScoreList.Count; i++)
            {
                // Debug.Log(ScoreList[i]);
                ScoreListing listing = Instantiate(_scoreListing, _content);
                if (listing != null)
                {

                }
                listing.SetScore(ScoreList[i]);

                //_listings.Add(listing);
            }


        }
        Gamecounter = ScoreList.Count + 1;


    }

    public static void Update_Score()
    {

    }


    public void Toggle_qwerty()
    {
        //qwerty.isOn = true;
        azerty.isOn = false;
        Keyboard_variable = true;
    }

    public void Toggle_azerty()
    {
        //azerty.isOn = true;
        qwerty.isOn = false;
        Keyboard_variable = false;
    }



}