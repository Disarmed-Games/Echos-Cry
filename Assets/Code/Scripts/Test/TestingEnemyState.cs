using UnityEngine;
using EchosCry.Enemy.StateSystem;
using System;

public class TestingEnemyState : MonoBehaviour
{
    private void Start()
    {
        NewEnemyStateCache cache = new();
        EnemyStateHandler handler = new(cache);
    }
}
