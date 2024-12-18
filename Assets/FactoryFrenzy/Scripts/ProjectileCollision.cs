using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    [SerializeField] float knockbackForce = 30f;
    [SerializeField] float jumpForce = 5f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.collider.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
                // Calculate the knockback direction, ensuring it's mostly horizontal
                Vector3 knockbackDirection = (collision.transform.position - transform.position).normalized;
                knockbackDirection.y = 0; // Eliminate vertical contribution from direction

                // Apply a small vertical jump force
                playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

                // Apply horizontal knockback force
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);

                Destroy(gameObject);
            }
        }
    }
}