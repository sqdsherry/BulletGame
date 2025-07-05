using UnityEngine;

public class ObstacleSpawnew : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;

    private void Start()
    {
        SpawnObstaclesAhead();
    }

    public void SpawnObstaclesAhead()
    {
        float startX = -10f;
        float endX = -60f; 

        for (float x = startX; x > endX;)
        {
            float spacingChanceX = Random.Range(0f, 100f);
            float spacingChanceZ = Random.Range(0f, 100f);
            float spacingX;
            float spacingZ;

            if (spacingChanceX < 70f)
                spacingX = Random.Range(1f, 2f);
            else if (spacingChanceX < 90f)
                spacingX = Random.Range(2f, 4f);
            else
                spacingX = Random.Range(3f, 5);

            if (spacingChanceZ < 40f)
                spacingZ = Random.Range(-2.5f, -1f);
            else if (spacingChanceZ < 70f)
                spacingZ = Random.Range(-1f, 1f);
            else
                spacingZ = Random.Range(1f, 2.5f);

            Vector3 pos = new Vector3(x, -0.25f, spacingZ);
            Instantiate(obstaclePrefab, pos, Quaternion.identity, this.transform);

            x -= spacingX;
        }
    }
}
