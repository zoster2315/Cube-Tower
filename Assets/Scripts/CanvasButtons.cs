using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasButtons : MonoBehaviour
{
    public Sprite musicOn, musicOff;

    public void Start()
    {
        if (PlayerPrefs.GetString("music") == "Off" && gameObject.name == "Music")
            GetComponent<Image>().sprite = musicOff;
    }

    public void RestartGame()
    {
        if (PlayerPrefs.GetString("music") != "Off")
            GetComponent<AudioSource>().Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadInstagram()
    {
        if (PlayerPrefs.GetString("music") != "Off")
            GetComponent<AudioSource>().Play();
        Application.OpenURL("https://www.instagram.com/zoster_gm/");
    }

    public void MusicWork()
    {
        if (PlayerPrefs.GetString("music") == "Off")
        {
            PlayerPrefs.SetString("music", "On");
            GetComponent<Image>().sprite = musicOn;
            GetComponent<AudioSource>().Play();
        }
        else
        {
            PlayerPrefs.SetString("music", "Off");
            GetComponent<Image>().sprite = musicOff;
        }
    }
}
