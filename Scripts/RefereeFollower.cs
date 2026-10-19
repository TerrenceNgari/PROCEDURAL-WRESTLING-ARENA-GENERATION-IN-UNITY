using UnityEngine;

public class RefereeFollower : MonoBehaviour
{
    public Transform wrestler1;
    public Transform wrestler2;

    public float moveSpeed = 2f;
    public float distanceFromFight = 2.5f;

    void Update()
    {
        if (wrestler1 == null || wrestler2 == null)
            return;

        GameObject ringFloor = GameObject.Find("Ring Floor");

        if (ringFloor == null)
            return;

        Renderer ringRenderer = ringFloor.GetComponent<Renderer>();

        if (ringRenderer == null)
            return;

        // Find the middle point between both wrestlers
        Vector3 middlePoint =
            (wrestler1.position + wrestler2.position) / 2f;

        // Stand slightly behind the action
        Vector3 targetPosition =
            middlePoint + new Vector3(0, 0, distanceFromFight);

        // Keep referee standing on the ring
        targetPosition.y =
            ringRenderer.bounds.max.y + 1f;

        // Keep referee inside procedural ring boundaries
        Bounds bounds = ringRenderer.bounds;

        float margin = 0.7f;

        targetPosition.x = Mathf.Clamp(
            targetPosition.x,
            bounds.min.x + margin,
            bounds.max.x - margin
        );

        targetPosition.z = Mathf.Clamp(
            targetPosition.z,
            bounds.min.z + margin,
            bounds.max.z - margin
        );

        // Move smoothly toward position
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // Look toward the wrestlers
        Vector3 direction =
            middlePoint - transform.position;

        direction.y = 0;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation =
                Quaternion.LookRotation(direction);
        }
    }
}