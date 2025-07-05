using UnityEngine;

public class PathChecker : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float checkDistance = 15f;
    [SerializeField] private float checkWidth = 12.5f;
    [SerializeField] private float winDistance = 5f;

    public (bool canMove, Collider doorCollider, bool isWinDistance) CanMove()
    {
        Collider doorCollider = null;
        bool hasObstacle = false;
        bool isWinDistance = false;

        Vector3 center = new Vector3(player.position.x - checkDistance, player.position.y, player.position.z);
        Vector3 halfExtents = new Vector3(checkWidth, 0.25f, player.GetComponent<SphereCollider>().bounds.size.z / 2);
        Collider[] hitColliders = Physics.OverlapBox(center, halfExtents);

        foreach (Collider col in hitColliders)
        {
            bool isObstacle = col.GetComponent<Obstacle>() != null;
            if (isObstacle)
                hasObstacle = true;
            if (col.CompareTag("Door"))
                doorCollider = col;
        }

        float distance = -1f;
        if (!hasObstacle && doorCollider != null)
        {
            distance = Vector3.Distance(player.position, doorCollider.transform.position);
            if (distance <= winDistance)
                isWinDistance = true;
        }
        return (!hasObstacle, doorCollider, isWinDistance);
    }
} 