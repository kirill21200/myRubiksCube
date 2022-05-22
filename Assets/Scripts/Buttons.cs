using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
    public int a;
    [SerializeField] private Dropdown level;
    public void Exit()
    {
        Application.Quit();
    }
    public void Game()
    {
        if (level.value == 0) StaticScript.Time = 600;
        else if (level.value == 1) StaticScript.Time = 360;
        else if (level.value == 2) StaticScript.Time = 180;
        SceneManager.LoadScene("SampleScene");
    }
    public void Restart()
    {
        SceneManager.LoadScene("Menu");
    }
}
