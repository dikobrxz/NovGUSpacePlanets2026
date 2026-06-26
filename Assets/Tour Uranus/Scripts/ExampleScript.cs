using System;
using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    [SerializeField] private int playerHealth = 100;
    [SerializeField] private string playerName = "Player";
    [SerializeField] private bool isGameActive = true;
    [SerializeField] private float playerSpeed = 5.5f;

    void Start()
    {
        Debug.Log("Start");
        ShowWelcomeMessage();
        Move();
        Damage();
        EndGame();
    }

    void Update()
    {
        //Debug.Log("Update");
    }

    void ShowWelcomeMessage()
    {
        Debug.Log($"Добро пожаловать, {playerName}!");
    }

    void Move()
    {
        Debug.Log("Игрок движется со скоростью " + playerSpeed);
    }

    void Damage()
    {
        int health = playerHealth;
        health -= 10;
        Debug.Log("Урон! Осталось здоровья: " + health);
    }

    void EndGame()
    {
        if (isGameActive)
        {
            Debug.Log("Игра окончена!");
        }
    }
}