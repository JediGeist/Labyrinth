using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class ChooseLevelLogic : MonoBehaviour
{
    public static bool isGameAudioDisabled;
    public static bool difficultMed;
    //  свитч звука
    public GameObject toggleSound;
    //  свитч отображения стен
    public GameObject toggleWalls;

    public void AudioToggleChanged()
    {
        isGameAudioDisabled = !toggleSound.GetComponent<UnityEngine.UI.Toggle>().isOn;
    }

    public void DifficultLevelChanged()
    {
        difficultMed = !toggleWalls.GetComponent<UnityEngine.UI.Toggle>().isOn;
    }

    public void DefaultPlayButtonPressed()
    {
        int savedLevel = PlayerPrefs.GetInt("LevelNum");
        PlayButtonPressed(savedLevel == 0 ? 1 : savedLevel);
    }

    public void PlayButtonPressed(int levelNumber)
    {
        try
        {
            PlayerPrefs.SetInt("IsSoundEnabled", isGameAudioDisabled == true ? 1 : 0);
            PlayerPrefs.SetInt("IsWallsEnabled", difficultMed == true ? 1 : 0);

            GameMenuLogic.level = levelNumber;

            FadeInOut.sceneEnd = true;
        }
        catch (Exception e)
        {
            Debug.Log("Cannot load scene 'GameScene': " + e.Message);
        }
    }
}
