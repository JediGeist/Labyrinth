using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public struct Wall
{
    public int x, y;
    public Directions dir;

    public Wall(string[] positions)
    {
        this.x = int.Parse(positions[0]);
        this.y = int.Parse(positions[1]);

        switch (positions[2].ToString())
        {
            case "1":
                this.dir = Directions.Up;
                break;
            case "2":
                this.dir = Directions.Right;
                break;
            case "3":
                this.dir = Directions.Down;
                break;
            case "4":
                this.dir = Directions.Left;
                break;
            default:
                this.dir = Directions.Right;
                break;
        }
    }
}

public enum Directions
{
    Up,
    Down,
    Right,
    Left
}

public static class Player
{
    public static Rigidbody2D playerRB;
    public static int playerPositionX, playerPositionY;
}

public class GameMenuLogic : MonoBehaviour
{
    public Text levelHeaderText;
    public Transform playerGameObj, exitGameObj;
    public static int? level;
    public static Wall exit;
    public static List<Wall> walls;
    public AudioSource gameAudio;

    private BaseCharacterMoving baseMoving;

    void Start()
    {
        if (ChooseLevelLogic.isGameAudioDisabled)
            gameAudio.Stop();

        if (level == null)
            level = 1;
        levelHeaderText.text += level.ToString();

        walls = new List<Wall>();

        TextAsset levelText = Resources.Load("LevelInfo_" + level) as TextAsset;
        string[] lines = levelText.text.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            string[] wallsPositions = lines[i][0] == ',' ? lines[i].Split(',') : lines[i].Substring(0, lines[i].Length - 1).Split(',');
            
            // стартовая позиция игрока
            if (wallsPositions.Length == 2)
            {
                Player.playerPositionX = int.Parse(wallsPositions[0]);
                Player.playerPositionY = int.Parse(wallsPositions[1]);
            }
            // стены
            else if (wallsPositions.Length == 3)
            {
                Wall wall = new Wall(wallsPositions);
                walls.Add(wall);
            }
            // выход
            else if (wallsPositions.Length == 4)
            {
                string[] exitCoor = { wallsPositions[1], wallsPositions[2], wallsPositions[3] };
                exit = new Wall(exitCoor);
            }
        }

        int saveLevel = PlayerPrefs.GetInt("LevelNum");
        if (saveLevel == level)
        {
            int savePlayerX = PlayerPrefs.GetInt("PlayerPositionX");
            if (savePlayerX != 0)
            {
                Player.playerPositionX = savePlayerX;
                Player.playerPositionY = PlayerPrefs.GetInt("PlayerPositionY");
            }
        }
            
        playerGameObj.position = new Vector3(140f + 99f * Player.playerPositionX, 422f + 99f * Player.playerPositionY, 0);

        Vector3 vect = new Vector3(140f + 99f * exit.x, 422f + 99f * exit.y, 0);

        if (exit.dir == Directions.Down)
        {
            exitGameObj.position = vect + new Vector3(0, -99f, 0);
            exitGameObj.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (exit.dir == Directions.Up)
            exitGameObj.position = vect + new Vector3(0, 99f, 0);
        else if (exit.dir == Directions.Left)
        {
            exitGameObj.position = vect + new Vector3(-99f, 0, 0);
            exitGameObj.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (exit.dir == Directions.Right)
        {
            exitGameObj.position = vect + new Vector3(99f, 0, 0);
            exitGameObj.rotation = Quaternion.Euler(0, 0, -90);
        }
    }

    public void ExitButtonPressed()
    {
        try
        {
            int temp = int.Parse(level.ToString());
            PlayerPrefs.SetInt("LevelNum", temp);
            PlayerPrefs.SetInt("PlayerPositionX", Player.playerPositionX);
            PlayerPrefs.SetInt("PlayerPositionY", Player.playerPositionY);
            FadeInOut.sceneEnd = true;
        }
        catch (Exception e)
        {
            Debug.Log("Cannot load scene 'GameScene': " + e.Message);
        }
    }

    public void NextLevelButtonPressed(GameObject congratulationsPanel)
    {
        level++;
        if (level <= 5)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        else
            congratulationsPanel.SetActive(true);
    }

    public static void LevelComplete(GameObject levelCompletePanel)
    {
        levelCompletePanel.SetActive(true);
    }
}
