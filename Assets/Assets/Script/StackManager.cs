using UnityEngine;
using TMPro;
using System.Collections;

public class StackManager : MonoBehaviour
{
    public static StackManager Instance;

    [Header("Stack Settings")]
    public float maxOffset = 1.2f;          // Maximum allowed X misalignment before failing
    public float perfectThreshold = 0.2f;   // Distance range considered a perfect drop
    public float settleTime = 2.0f;         // Time before checking stability
    public float tipThreshold = 20f;        // Max tilt (in degrees) before considered tipped over
    public float fallThresholdY = -10f;     // Y-position limit before considered fallen

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

        firstCarPlaced = true;
        lastCar = car;

        Rigidbody2D rb = car.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static; // Base should never move

        score = 1;
        UpdateScoreUI();
        Debug.Log("🏁 First car placed as base of stack.");
    }

    public void RegisterCar(GameObject newCar, GameObject hitObject)
    {
        if (gameOver) return;

        // Prevent hitting the base directly
        if (hitObject.CompareTag("Base"))
        {
            GameOver("❌ Hit the base again!");
            return;
        }

        // Valid landing — only if it hits the last stacked car
        if (hitObject == lastCar)
        {
            float offset = Mathf.Abs(newCar.transform.position.x - lastCar.transform.position.x);

            if (offset > maxOffset)
            {
                GameOver("❌ Missed the stack!");
                return;
            }

            if (offset < perfectThreshold)
                Debug.Log("💥 Perfect Drop!");
            else
                Debug.Log("✅ Good Drop");

            score++;
            UpdateScoreUI();

            // Let physics determine if it’s stable or falls off
            StartCoroutine(CheckCarStability(newCar));
            lastCar = newCar;
        }
        else
        {
            // Hit something that’s not the last car — fail
            GameOver("❌ Not stacked properly!");
        }
    }

    IEnumerator CheckCarStability(GameObject car)
    {
        Rigidbody2D rb = car.GetComponent<Rigidbody2D>();

        // Wait a short period for initial impact to settle
        yield return new WaitForSeconds(settleTime);

        // Wait until motion nearly stops (helps avoid false triggers)
        while (rb != null && (rb.linearVelocity.magnitude > 0.1f || Mathf.Abs(rb.angularVelocity) > 0.1f))
            yield return null;

        if (car == null) yield break;

        float zRotation = Mathf.Abs(car.transform.eulerAngles.z);
        if (zRotation > 180f) zRotation = 360f - zRotation; // normalize rotation (e.g., 350° = 10°)

        // Fail if car has fallen off or tipped beyond threshold
        if (car.transform.position.y < fallThresholdY || zRotation > tipThreshold)
        {
            GameOver("💥 Car fell or tipped over!");
            yield break;
        }

        // Freeze stable car as part of the stack
        rb.bodyType = RigidbodyType2D.Static;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        Debug.Log("🚗 Car stabilized and locked in place.");
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
