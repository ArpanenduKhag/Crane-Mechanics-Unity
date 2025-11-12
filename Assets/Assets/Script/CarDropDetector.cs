using UnityEngine;

public class CarDropDetector : MonoBehaviour
{
    private bool landed = false;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (landed) return;

        if (collision.gameObject.CompareTag("Base"))
        {
            landed = true;

            if (!StackManager.Instance)
                return;

            // If first car hits base → become stack base
            if (!StackManager.Instance.IsGameOver() && StackManager.Instance != null)
            {
                StackManager.Instance.RegisterBaseCar(gameObject);
            }

            Destroy(this);
        }
        else if (collision.gameObject.CompareTag("Car"))
        {
            landed = true;

            if (StackManager.Instance != null)
            {
                StackManager.Instance.RegisterCar(gameObject, collision.gameObject);
            }

            Destroy(this);
        }
    }
}
