using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class InfoDisplayer : MonoBehaviour
{
    [SerializeField]
    private Text round;
    [SerializeField]
    private Text playerHealth;
    [SerializeField]
    private Text time;
    [SerializeField]
    private GameObject deathScreen;


    public void ShowHp(int playerHealth)
    {
        this.playerHealth.text = playerHealth.ToString();
    }

    public void ShowRound(int round)
    {
        this.round.text = round.ToString();
    }

    public void ShowTime(string time)
    {
        this.time.text = time;
    }

    public void ShowDeathScreen()
    {
        deathScreen.gameObject.SetActive(true);
    }

    public void EndGame()
    {
        Application.Quit();
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
