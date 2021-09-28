using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    //match 3 game
    public int scoreNeeded = 250, level = 0, totalScore = 0;
    public bool firstTimeMatch3 = true, firstTimeMenu = true, firstTimeWord = true, firstTimeBicycling = true;

    //word game
    private int wordSlot = 0;
    public int wordLevel = 0;

    //runner game
    public int Trash = 0, Points;
    public bool invincible = false;

    // dialogues
    public bool recyclingDialogueDone = false;
    public bool endlessDialogueDone = false;
    public bool wordDialogueDone = false;
    public bool coloringDialogueDone = false;

    public int getWordSlot()
    {
        return wordSlot;
    }

    public void setWordSlot()
    {
        wordSlot++;
    }

    public void resetWordSlot()
    {
        wordSlot = 0;
    }

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
}
