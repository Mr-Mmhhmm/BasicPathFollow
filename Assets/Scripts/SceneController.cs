using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void PickUpGame()
    {
        SceneManager.LoadScene("PickUp_Scene");
    }

    public void HunterGame()
    {
        SceneManager.LoadScene("Hunter_Scene");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}