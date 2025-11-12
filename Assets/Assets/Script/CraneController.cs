using UnityEngine;
using System.Collections;  // ✅ required for IEnumerator



public class CraneController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float moveRange = 3f;
    public Transform hookPoint;
    public GameObject carPrefab;

    private Vector3 startPosition;
    private GameObject currentCar;
    private bool movingRight = true;

    void Start()
    {
        startPosition = transform.position;
        SpawnNewCar();
    }

    void Update()
    {
        if (StackManager.Instance != null && StackManager.Instance.IsGameOver())
            return;

        MoveCrane();

        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && currentCar != null)
        {
            currentCar.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * 3f) * 2f);
            DropCar();
        }
    }

    void MoveCrane()
    {
        float moveDir = movingRight ? 1 : -1;
        transform.Translate(Vector2.right * moveDir * moveSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - startPosition.x) > moveRange)
            movingRight = !movingRight;
    }

    void SpawnNewCar()
    {
        currentCar = Instantiate(carPrefab, hookPoint.position, Quaternion.identity);
        currentCar.transform.parent = hookPoint;

        Rigidbody2D rb = currentCar.GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = currentCar.AddComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic; // replaces isKinematic = true
        rb.gravityScale = 0f; // no gravity while attached to crane
    }

    void DropCar()
    {
        currentCar.transform.parent = null;
        Rigidbody2D rb = currentCar.GetComponent<Rigidbody2D>();
        currentCar.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-3f, 3f));

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0.5f; // slightly softer fall start
    
        StartCoroutine(EnableFullGravity(rb, 0.2f));
        currentCar.AddComponent<CarDropDetector>();
        Invoke(nameof(SpawnNewCar), 1.2f);
    }
    
    IEnumerator EnableFullGravity(Rigidbody2D rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.gravityScale = 1f;
    }

}
