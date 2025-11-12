using UnityEngine;

public class CarCollision2D : MonoBehaviour
{
    private bool hasLanded = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasLanded) return;
        hasLanded = true;

        if (StackManager.Instance != null)
            StackManager.Instance.RegisterCar(gameObject, collision.gameObject);
    }
}
