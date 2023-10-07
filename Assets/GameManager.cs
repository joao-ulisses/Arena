using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Transform> Enemies;
    public List<Transform> Waypoints;

    public void removeEnemy(string name)
    {
        for (int i = 0; i < Enemies.Count; i++)
        {
            if (Enemies[i].name == name)
            {
                Enemies.RemoveAt(i);
                break;
            }
        }
    }
}
