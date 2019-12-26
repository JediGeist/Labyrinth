using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    public void GenLab(string str)
    {
        int w = int.Parse(str[0].ToString());
        int h = int.Parse(str[1].ToString());

        string result = GenerateNewLevel(w, h);

        PlayerPrefs.SetString("Generated_Level_" + str, result);
    }

    public string GenerateNewLevel(int width, int height)
    {
        Cell furthest = new Cell(0, 0);
        int a = width, b = height;
        Cell[,] cells = new Cell[a, b];

        for (int i = 0; i < a; i++)
        {
            for (int j = 0; j < b; j++)
            {
                cells[i, j] = new Cell(i, j);
            }
        }

        Cell current = cells[UnityEngine.Random.Range(0, a), UnityEngine.Random.Range(0, b)];
        current.visited = true;
        current.distanceFromStart = 0;

        string result = string.Format("{0},{1}\n", current.x, current.y);

        Stack<Cell> stack = new Stack<Cell>();
        do
        {
            List<Cell> unvisitedNeighbours = new List<Cell>();

            int x = current.x;
            int y = current.y;

            if (x > 0 && !cells[x - 1, y].visited) unvisitedNeighbours.Add(cells[x - 1, y]);
            if (y > 0 && !cells[x, y - 1].visited) unvisitedNeighbours.Add(cells[x, y - 1]);
            if (x < a - 1 && !cells[x + 1, y].visited) unvisitedNeighbours.Add(cells[x + 1, y]);
            if (y < b - 1 && !cells[x, y + 1].visited) unvisitedNeighbours.Add(cells[x, y + 1]);

            if (unvisitedNeighbours.Count > 0)
            {
                Cell chosen = unvisitedNeighbours[UnityEngine.Random.Range(0, unvisitedNeighbours.Count)];
                RemoveWall(current, chosen);

                chosen.visited = true;
                stack.Push(chosen);
                chosen.distanceFromStart = current.distanceFromStart + 1;
                if (chosen.distanceFromStart > furthest.distanceFromStart)
                    furthest = chosen;
                current = chosen;
            }
            else
            {
                current = stack.Pop();
            }
        } while (stack.Count > 0);

        for (int i = 0; i < a; i++)
        {
            for (int j = 0; j < b; j++)
            {
                result += cells[i, j].ToString();
            }
        }
        result += string.Format(",{0},{1},{2}", furthest.x, furthest.y, furthest.isBottom ? "3" : furthest.isUp ? "1" : furthest.isLeft ? "4" : "2");

        return result;
    }

    private void RemoveWall(Cell curr, Cell chosen)
    {
        //  на одной горизонтали 
        if (curr.x == chosen.x)
        {
            //  выбранная находится ниже
            if (curr.y > chosen.y)
            {
                curr.isBottom = false;
                chosen.isUp = false;
            }
            //  выбранная выше
            else
            {
                chosen.isBottom = false;
                curr.isUp = false;
            }
        }
        //  на одной вертикали
        else
        {
            //  выбранная находится левее
            if (curr.x > chosen.x)
            {
                curr.isLeft = false;
                chosen.isRight = false;
            }
            //  выбранная правее
            else
            {
                chosen.isLeft = false;
                curr.isRight = false;
            }
        }
    }
}

class Cell
{
    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;
        isBottom = isUp = isLeft = isRight = true;
        distanceFromStart = 0;
    }

    public int x, y, distanceFromStart;
    public bool isBottom, isUp, isRight, isLeft, visited;

    public override string ToString()
    {
        string temp = string.Format("{0},{1},", x, y);
        string res = "";

        if (isBottom)
        {
            res += temp + "3\n";
        }
        if (isUp)
        {
            res += temp + "1\n";
        }
        if (isRight)
        {
            res += temp + "2\n";
        }
        if (isLeft)
        {
            res += temp + "4\n";
        }

        return res;
    }

}