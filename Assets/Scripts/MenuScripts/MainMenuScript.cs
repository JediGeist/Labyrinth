using UnityEngine;  

public class MainMenuScript : MonoBehaviour
{
    public AudioSource gameAudio;
    //  свитч звука
    public GameObject toggleSound;
    //  свитч отображения стен
    public GameObject toggleWalls;

    void Start()
    {
        ChooseLevelLogic.isGameAudioDisabled = PlayerPrefs.GetInt("IsSoundEnabled") == 1;
        ChooseLevelLogic.difficultMed = PlayerPrefs.GetInt("IsWallsEnabled") == 1;

        if (ChooseLevelLogic.isGameAudioDisabled)
        {
            gameAudio.Stop();
            toggleSound.GetComponent<UnityEngine.UI.Toggle>().isOn = false;
        }

        if (ChooseLevelLogic.difficultMed)
        {
            toggleWalls.GetComponent<UnityEngine.UI.Toggle>().isOn = false;
        }
    }

    public void ExitButtonPressed()
    {
        PlayerPrefs.SetInt("IsSoundEnabled", toggleSound.GetComponent<UnityEngine.UI.Toggle>().isOn == true ? 1 : 0);
        PlayerPrefs.SetInt("IsWallsEnabled", toggleWalls.GetComponent<UnityEngine.UI.Toggle>().isOn == true ? 1 : 0);
        PlayerPrefs.Save();
        Application.Quit();
    }
}
