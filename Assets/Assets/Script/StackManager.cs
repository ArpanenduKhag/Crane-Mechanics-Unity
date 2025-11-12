using UnityEngine;
using TMPro;

public class StackManager : MonoBehaviour
{
    public static StackManager Instance;

    [Header("Stack Settings")]
    public float maxOffset = 1.2f;       // how far off from last car is allowed
    public float perfectThreshold = 0.2f; // perfect stack range

    [Header("UI")]
    public TMP_Text scoreText;
    public GameObject gameOverText;

    private GameObject lastCar;
    private bool gameOver = false;
    private int score = 0;
    private bool firstCarPlaced = false;

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterBaseCar(GameObject car)
    {
        if (firstCarPlaced) return;

        // First car becomes base
        firstCarPlaced = true;
        lastCar = car;

        Rigidbody2D rb = car.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

        score = 1;
        UpdateScoreUI();
        Debug.Log("🏁 First car placed as base of stack.");
    }

    public void RegisterCar(GameObject newCar, GameObject hitObject)
    {
        if (gameOver) return;

        // If the new car hits the base instead of the last car → Game Over
        if (hitObject.CompareTag("Base"))
        {
            GameOver("❌ Hit the base again!");
            return;
        }

        // Check if landed on last stacked car
        if (hitObject == lastCar)
        {
            float offset = Mathf.Abs(newCar.transform.position.x - lastCar.transform.position.x);

            if (offset > maxOffset)
            {
                GameOver("❌ Missed the stack!");
                return;
            }

            // Perfect or good drop
            if (offset < perfectThreshold)
                Debug.Log("💥 Perfect Drop!");
            else
                Debug.Log("✅ Good Drop");

            score++;
            UpdateScoreUI();

            // Freeze new car as part of the stack
            Rigidbody2D rb = newCar.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;

            // Update the top of the stack
            lastCar = newCar;
        }
        else
        {
            // Landed on something else → Game Over
            GameOver("❌ Not stacked properly!");
        }
    }

    void GameOver(string message)
    {
        if (gameOver) return;
        gameOver = true;

        Debug.Log(message);
        if (gameOverText != null)
            gameOverText.SetActive(true);
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    public bool IsGameOver()
    {
        return gameOver;
    }
}
