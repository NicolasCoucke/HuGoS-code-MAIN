using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
using System;

public class TUTPART1TutorialTexts : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject text1;
    [SerializeField]
    private GameObject text2;
    [SerializeField]
    private GameObject text3;
    [SerializeField]
    private GameObject text4;
    [SerializeField]
    private GameObject text5;
    //[SerializeField]
    //private GameObject text51;
    [SerializeField]
    private GameObject text6;
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
        Texts.Add(text2);
        Texts.Add(text3);
        Texts.Add(text4);
        Texts.Add(text5);
        Texts.Add(text6);
        //Texts.Add(text7);


        Text_wait_times.Add(7); // first text stays this long (you're in volcanic area)
        Text_wait_times.Add(5); // prevent lava flow
        Text_wait_times.Add(7); // encircle
        Text_wait_times.Add(5); // turn black
        Text_wait_times.Add(8); //faster = better
        Text_wait_times.Add(8); // choose largest one
        //Text_wait_times.Add(7);

        StartCoroutine("TextSequence", 2f);

    }

    // Update is called once per frame
    void Update()
    {
        //if (HoleClosed == true & OnceHoleClosed == false)
        //{
        //    IsSolidified();
        //    HoleClosed = false;
        //    OnceHoleClosed = true;

        //}
    }

    //insert function to start rest of sequence connected to the generator


    //public void IsSolidified()
    //{

    //    text51.SetActive(true);
    //    if (!Sequence2Active)
    //    {
    //        StartCoroutine("TextSequence2");
    //    }



    //}

    IEnumerator TextSequence()
    {
        for (int i = 0; i < 6; i++)
        {

            Texts[i].SetActive(true);
            if (i != 0)
                Texts[i - 1].SetActive(false);
            yield return new WaitForSeconds(Text_wait_times[i]);
        }
        Texts[Texts.Count-1].SetActive(false);
        // yield return new WaitForSeconds(10);
        //if (!Sequence2Active)
        //{
        //    StartCoroutine("TextSequence2");
        //}

    }

//    IEnumerator TextSequence2()
//    {
//        Sequence2Active = true;
//        yield return new WaitForSeconds(4);
//        text51.SetActive(false);
//        for (int i = 5; i < 7; i++)
//        {
//            Texts[i].SetActive(true);

//            Texts[i - 1].SetActive(false);
//            yield return new WaitForSeconds(Text_wait_times[i]);
//        }
//    }

}
