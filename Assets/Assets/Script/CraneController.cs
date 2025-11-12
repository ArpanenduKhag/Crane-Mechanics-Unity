using UnityEngine;

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
        MoveCrane();

        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && currentCar != null)
        {
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
        currentCar.GetComponent<Rigidbody2D>().isKinematic = true;
    }

    void DropCar()
    {
        currentCar.transform.parent = null;
        var rb = currentCar.GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
        rb.gravityScale = 1f;

        Invoke(nameof(SpawnNewCar), 1f); // Delay to spawn next car
    }
}
