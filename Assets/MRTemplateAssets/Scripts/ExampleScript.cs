using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    public int score;
    public string name = "player";
    public bool isStarted = false;
    public float speed = 5.0f;

    private void Start() {
        Debug.Log("Сцена запущена!");
        InitializePlayer();
    }

    // private void Update() {
    //     if (Time.frameCount % 60 == 0) {
    //         Debug.Log($"Кадр обновлён! Скорость: {speed}, кадр: {Time.frameCount}");
    //     }
    // }

    private void InitializePlayer() {
        Debug.Log($"Игрок '{name}' инициализирован! Счёт: {score}, статус запуска: {isStarted}");
    }
}

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    private void Start() => currentHealth = maxHealth;

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($" Получено {amount} урона!! Хп: {currentHealth} / {maxHealth}");
        if (currentHealth <= 0) Die();
    }

    private void Die() => Debug.Log("Умер");
}
