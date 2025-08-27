using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public void PlayGameButton() {
        SceneManager.LoadScene("Mainpage");
    }
    public void CreateNewCaseButton(){
        SceneManager.LoadScene("CreateNewCase");
    }
    public void CreateCaseButton(){
        SceneManager.LoadScene("Ingame");
    }
    public void ExitAllGameButton(){
        Application.Quit();
    }
}
