using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
using System;

public class TutorialTexts : MonoBehaviourPunCallbacks
{

    public bool DominanceCondition;

    [SerializeField]
    private GameObject text1;

    [SerializeField]
    private GameObject text1_2;

    [SerializeField]
    private GameObject text2;
    [SerializeField]
    private GameObject text3;
    //[SerializeField]
    //private GameObject text4;
    //[SerializeField]
    //private GameObject text5;
    //[SerializeField]
    //private GameObject text51;
    //[SerializeField]
    //private GameObject text6;
    //[SerializeField]
    //private GameObject text7;

    public static Boolean HoleClosed = false;
    private Boolean OnceHoleClosed = false;

    private Boolean Sequence2Active = false;

    private List<int> Text_wait_times = new List<int>();
    private List<GameObject> Texts = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        HoleClosed = false;
        Texts.Add(text1);

        if(StaticVariables.Condition == 3)
            Texts.Add(text1_2);
        if (StaticVariables.Condition == 4)
            Texts.Add(text1_2);

        Texts.Add(text2);
        Texts.Add(text3);
   

        Text_wait_times.Add(15); // move 
        if (StaticVariables.Condition == 3)
            Text_wait_times.Add(15); // crown

        if (StaticVariables.Condition == 4)
            Text_wait_times.Add(15); // crown

        Text_wait_times.Add(10); // encircle 
       // Text_wait_times.Add(15); // good job
      

        StartCoroutine("TextSequence", 2f);

    }

    // Update is called once per frame
    void Update()
    {
        if(HoleClosed == true & OnceHoleClosed == false)
        {
            StartCoroutine("IsSolidified");
            HoleClosed = false;
            OnceHoleClosed = true;

        }
    }

    //insert function to start rest of sequence connected to the generator


    IEnumerator IsSolidified()
    {

        text2.SetActive(false);
        text3.SetActive(true);
        yield return new WaitForSeconds(4);
        PhotonNetwork.LoadLevel("RoomLobbyScene");
        //if (!Sequence2Active)
        //{
        //    StartCoroutine("TextSequence2");
        //}



    }

    IEnumerator TextSequence()
    {
        for (int i = 0; i < Texts.Count-1; i++)
        {

            Texts[i].SetActive(true);
            if (i != 0)
                Texts[i - 1].SetActive(false);
            yield return new WaitForSeconds(Text_wait_times[i]);
        }
        yield return new WaitForSeconds(10);
        //if (!Sequence2Active)
        //{
        //    StartCoroutine("TextSequence2");
        //}

    }

    //IEnumerator TextSequence2()
    //{
    //    Sequence2Active = true;
    //    yield return new WaitForSeconds(4);
    //    text51.SetActive(false);
    //    for (int i = 5; i < 7; i++)
    //    {
    //        Texts[i].SetActive(true);

    //        Texts[i - 1].SetActive(false);
    //        yield return new WaitForSeconds(Text_wait_times[i]);
    //    }
    //}

}
