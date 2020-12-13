using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticVariables
{
    private static int condition = 0;

    private static int players;

    private static bool isSpectator = false;

    private static bool definedPath = false;

    private static string path;

    private static bool textdefinedPath = false;

    private static string textpath;

    private static string prolificID;

    private static string rewardCode;

    private static bool doReconnect = false;


    private static int maxPlayers;

    public static int MaxPlayers
    {
        get
        {
            return maxPlayers;
        }
        set
        {
            maxPlayers = value;
        }
    }

    public static string ProlificID
    {
        get
        {
            return prolificID;
        }
        set
        {
            prolificID = value;
        }
    }

    public static string RewardCode
    {
        get
        {
            return rewardCode;
        }
        set
        {
            rewardCode = value;
        }
    }

    public static string Path
    {
        get
        {
            return path;
        }
        set
        {
            path = value;
        }
    }

    public static bool DefinedPath
    {
        get
        {
            return definedPath;
        }
        set
        {
            definedPath = value;
        }
    }

    public static string TextPath
    {
        get
        {
            return textpath;
        }
        set
        {
            textpath = value;
        }
    }

    public static bool TextDefinedPath
    {
        get
        {
            return textdefinedPath;
        }
        set
        {
            textdefinedPath = value;
        }
    }

    public static int Players
    {
        get
        {
            return players;
        }
        set
        {
            players = value;
        }
    }

    public static int Condition
    {
        get
        {
            return condition;
        }
        set
        {
            condition = value;
        }
    }

    public static bool IsSpectator
    {
        get
        {
            return isSpectator;
        }
        set
        {
            isSpectator = value;
        }
    }

    public static bool DoReconnect
    {
        get
        {
            return doReconnect;
        }
        set
        {
            doReconnect = value;
        }
    }


}
