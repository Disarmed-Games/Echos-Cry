using UnityEngine;
using EchosCry.Enemy.StateSystem;
using System;

public class TestingEnemyState : MonoBehaviour
{
    private void Start()
    {
        EnemyStateCache cache = new();
        EnemyStateHandler handler = new(cache);
    }
}
