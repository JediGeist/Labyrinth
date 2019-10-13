using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine;

public class BaseCharacterMoving : MonoBehaviour
{
    public float speed = 300.0f;
    public float cellSize = 99.0f;
    public Vector2 startPoisition, direction;
    public Transform wallPrefab;
    public GameObject levelCompletePanel;

    private static bool difficultMed;
    private bool isMoving = false;
    private Vector3 destination;
    private List<GameObject> wallsOnLevel;

    void Start()
    {
        difficultMed = ChooseLevelLogic.difficultMed;
        Player.playerRB = GetComponent<Rigidbody2D>();
        wallsOnLevel = new List<GameObject>();

        if (!difficultMed)
        {
            ShowClosestWalls();
        }
    }

    void Update()
    {
        if (isMoving == true)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, destination, step);

            //  При достижении нужной позиции меняем значение флага для возможности дальше передвигаться
            if (transform.position == destination)
            {
                isMoving = false;

                // После хода необхожимо показать новые стены, поскольку позиция игрока сменилась
                if (!difficultMed)
                {
                    ShowClosestWalls();
                }
            }
        }
        else if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            //  позиция начала свайпа
            if (touch.phase == TouchPhase.Began)
            {
                startPoisition = touch.position;
            }
            //  направление свайпа
            else if (touch.phase == TouchPhase.Moved)
            {
                direction = touch.position - startPoisition;
            }
            //  при завершении свайпа перемещаем персонажа
            else if (startPoisition != touch.position && touch.phase == TouchPhase.Ended)
            {
                //  высчитываем модуль угла направления свайпа относительно вектора, направленного вправо (ось OX)
                float angleValue = Vector3.Angle(direction, Vector3.right);

                //  если угол по модулю меньше 45 градусов, то считаем свайп правым
                if (angleValue < 45 && angleValue >= 0)
                {
                    if (IsMovementEnable(Directions.Right))
                    {
                        destination = transform.position + Vector3.right * cellSize;
                        Player.playerPositionX++;
                        isMoving = true;
                    }
                    else if (difficultMed)
                    {
                        InstantiateAndDestroyWall(new Vector3(51f, 0, 0), true);
                    }
                }
                //  если же угол от 45 до 135, то это направления вверх и вних взависимости от проекции свайпа на ось OY
                else if (angleValue <= 135 && angleValue >= 45)
                {
                    if (direction.y > 0)
                    {
                        if (IsMovementEnable(Directions.Up))
                        {
                            destination = transform.position + Vector3.up * cellSize;
                            Player.playerPositionY++;
                            isMoving = true;
                        }
                        else if (difficultMed)
                        {
                            InstantiateAndDestroyWall(new Vector3(0, 49.5f, 0));
                        }
                    }
                    else
                    {
                        if (IsMovementEnable(Directions.Down))
                        {
                            destination = transform.position + Vector3.down * cellSize;
                            Player.playerPositionY--;
                            isMoving = true;
                        }
                        else if(difficultMed)
                        {
                            InstantiateAndDestroyWall(new Vector3(0, -49.5f, 0));
                        }
                    }
                }
                //  иначе влево
                else
                {
                    if (IsMovementEnable(Directions.Left))
                    {
                        destination = transform.position + Vector3.left * cellSize;
                        Player.playerPositionX--;
                        isMoving = true;
                    }
                    else if (difficultMed)
                    {
                        InstantiateAndDestroyWall(new Vector3(-49.5f, 0, 0), true);
                    }
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.W))
            { 
                if (IsMovementEnable(Directions.Up))
                {
                    destination = transform.position + Vector3.up * cellSize;
                    isMoving = true;
                    Player.playerPositionY++;
                }
                else if(difficultMed)
                {
                    InstantiateAndDestroyWall(new Vector3(0, 49.5f, 0));
                }
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                if (IsMovementEnable(Directions.Left))
                {
                    destination = transform.position + Vector3.left * cellSize;
                    isMoving = true;
                    Player.playerPositionX--;
                }
                else if(difficultMed)
                {
                    InstantiateAndDestroyWall(new Vector3(-49.5f, 0, 0), true);
                }
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                if (IsMovementEnable(Directions.Down))
                {
                    destination = transform.position + Vector3.down * cellSize;
                    isMoving = true;
                    Player.playerPositionY--;
                }
                else if (difficultMed)
                {
                    InstantiateAndDestroyWall(new Vector3(0, -49.5f, 0));
                }
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                if (IsMovementEnable(Directions.Right))
                {
                    destination = transform.position + Vector3.right * cellSize;
                    isMoving = true;
                    Player.playerPositionX++;
                }
                else if (difficultMed)
                {
                    InstantiateAndDestroyWall(new Vector3(51f, 0, 0), true);
                }
            }
        }
    }

    /// <summary> Проверка столкновения со стенами </summary>
    /// <param name="dir"> Направление движения игрока </param>
    bool IsMovementEnable(Directions dir)
    {
        if (GameMenuLogic.exit.x == Player.playerPositionX && GameMenuLogic.exit.y == Player.playerPositionY && GameMenuLogic.exit.dir == dir)
        {
            GameMenuLogic.LevelComplete(levelCompletePanel); 
            return false;
        }

        foreach(Wall wall in GameMenuLogic.walls)
        {
            if (wall.x == Player.playerPositionX && wall.y == Player.playerPositionY && wall.dir == dir)
                return false;
        }

        return true;
    }

    /// <summary> Подчищает "старые" (отображаемые на прошлом ходу) стены </summary>
    void DestroyOldWalls()
    {
        foreach(var wall in wallsOnLevel)
        {
            Destroy(wall);
        }

        wallsOnLevel = new List<GameObject>();
    }

    /// <summary> Отображает близлежащие стены к игроку </summary>
    void ShowClosestWalls()
    {
        if (wallsOnLevel.Count > 0)
            //  для начала удалим все предыдущие стены с таймером в секунду
            DestroyOldWalls();

        List<Wall> closestWalls = new List<Wall>();
        List<KeyValuePair<Wall, int>> shouldShow = new List<KeyValuePair<Wall, int>>();
        bool isUp, isDown, isRight, isLeft;
        isUp = isDown = isRight = isLeft = true;

        foreach (Wall wall in GameMenuLogic.walls)
        {
            if (wall.x == Player.playerPositionX && wall.y <= Player.playerPositionY + 1 && wall.y >= Player.playerPositionY - 1
                || wall.y == Player.playerPositionY && wall.x <= Player.playerPositionX + 1 && wall.x >= Player.playerPositionX - 1)
                closestWalls.Add(wall);
            if (wall.x == Player.playerPositionX && wall.y == Player.playerPositionY)
            {
                if (wall.dir == Directions.Up)
                {
                    isUp = false;
                    shouldShow.Add(new KeyValuePair<Wall, int>(wall, -1));
                }
                else if (wall.dir == Directions.Down)
                {
                    isDown = false;
                    shouldShow.Add(new KeyValuePair<Wall, int>(wall, -1));
                }
                else if (wall.dir == Directions.Right)
                {
                    isRight = false;
                    shouldShow.Add(new KeyValuePair<Wall, int>(wall, -1));
                }
                else
                {
                    isLeft = false;
                    shouldShow.Add(new KeyValuePair<Wall, int>(wall, -1));
                }
            }
        }

        foreach (Wall wall in closestWalls)
        {
            if (wall.x == Player.playerPositionX && wall.y == Player.playerPositionY + 1 && isUp)
                shouldShow.Add(new KeyValuePair<Wall, int>(wall, 0));
            else if (wall.x == Player.playerPositionX + 1 && wall.y == Player.playerPositionY && isRight)
                shouldShow.Add(new KeyValuePair<Wall, int>(wall, 2));
            else if (wall.x == Player.playerPositionX - 1 && wall.y == Player.playerPositionY && isLeft)
                shouldShow.Add(new KeyValuePair<Wall, int>(wall, 3));
            else if (wall.x == Player.playerPositionX && wall.y == Player.playerPositionY - 1 && isDown)
                shouldShow.Add(new KeyValuePair<Wall, int>(wall, 1));
        }

        foreach (KeyValuePair<Wall, int> wall in shouldShow)
        {
            float rightStep = 0f, upStep = 0f, downStep = 0f, leftStep = 0f, eps = 49.5f, x = 0f, y = 0f;

            //  добавим шаг в полклетки для корректного отображения стены
            if (wall.Key.dir == Directions.Up)
                upStep += eps;
            else if (wall.Key.dir == Directions.Down)
                downStep -= eps;
            else if (wall.Key.dir == Directions.Left)
                leftStep -= eps;
            else
                rightStep += eps;

            //  по координатам отложим соответствующие шаги 
            x = rightStep + leftStep;
            y = upStep + downStep;

            //  если боковое направление, то необходимо повернуть стену
            bool dirFlag = wall.Key.dir == Directions.Left || wall.Key.dir == Directions.Right;

            //  также необходимо сместить стены на соответствующие клетки
            if (wall.Value == 0)
                y += eps * 2;
            else if (wall.Value == 1)
                y -= eps * 2;
            else if (wall.Value == 2)
                x += eps * 2;
            else if (wall.Value == 3)
                x -= eps * 2;

            //  и создаём стены на игровом поле
            GameObject clone = Instantiate(wallPrefab, transform.position + new Vector3(x, y, 0), dirFlag ? Quaternion.Euler(0, 0, 90) : Quaternion.identity).gameObject;
            wallsOnLevel.Add(clone);
        }
    }

    /// <summary> Создаёт стену на месте столкновения и задаёт таймер, через который стена удалится </summary>
    /// <param name="position"> Расположение стены относительно игрока </param>
    /// <param name="rotation"> Поворот стены (горизонтально - Euler(0, 0, 90) или вертикально - identity) </param>
    void InstantiateAndDestroyWall(Vector3 position, bool rotation = false)
    {
        GameObject clone = Instantiate(wallPrefab, transform.position + position, rotation ? Quaternion.Euler(0, 0, 90) : Quaternion.identity).gameObject;
        Destroy(clone, 2);
    }
}
