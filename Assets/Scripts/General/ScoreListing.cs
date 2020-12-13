using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreListing : MonoBehaviour
{
    [SerializeField]
    private Text _text;


    public void SetScore(string ScoreString)
    {
        _text.text = ScoreString;
    }
}
 