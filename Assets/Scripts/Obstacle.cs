using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float _radiusAtachObstacles = 1.75f;

    private bool _isTouched = false;

    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);

        if (_isTouched) return;
        Infect();
    }

    public void Infect()
    {
        Queue<Obstacle> queue = new Queue<Obstacle>();
        queue.Enqueue(this);

        while (queue.Count > 0)
        {
            Obstacle current = queue.Dequeue();
            if (current._isTouched) continue;
            current._isTouched = true;

            Collider[] hitColliders = Physics.OverlapSphere(current.transform.position, current._radiusAtachObstacles);
            foreach (Collider col in hitColliders)
            {
                Obstacle neighbor = col.gameObject.GetComponent<Obstacle>();
                if (neighbor != null && neighbor != current && !neighbor._isTouched)
                {
                    queue.Enqueue(neighbor);
                }
            }
            current.StartCoroutine(current.DestroyObstacle());
        }
    }

    private IEnumerator DestroyObstacle()
    {

        this.gameObject.GetComponent<Renderer>().material.color = Color.green;
        yield return new WaitForSeconds(0.5f);
        this.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
        yield return new WaitForSeconds(0.5f);
        this.gameObject.GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
        PlayerController.onCheckTrap?.Invoke();
    }
}
