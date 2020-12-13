using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListing : MonoBehaviour
{
    [SerializeField]
    private Text _text;
    [SerializeField]
    private Text Countertext;
    [SerializeField]
    private Text Readytext;

    public Player Player { get; private set; }
    int counter = 0;

    public void SetPlayerInfo(Player player)
    {
        Player = player;
        _text.text = "Player " + (player.ActorNumber-1).ToString();
        if(StaticVariables.IsSpectator)
        {
            StartCoroutine("CounterStarted");
        }
       
    }

    public void PlayerReady()
    {
        Countertext.enabled = false;
        Readytext.enabled = true;
    }

    IEnumerator CounterStarted()
    {
        while(1==1)
        {
            yield return new WaitForSeconds(1);
            counter += 1;
            Countertext.text = counter.ToString();
        }



    }

}
