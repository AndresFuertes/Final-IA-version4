using UnityEngine;

public class BoidLimit : MonoBehaviour
{
    public static BoidLimit Instance;

    public float maxX;
    public float maxZ;
    public float maxY;

    private void Awake()
    {
        Instance = this;
    }

    public void CheckBounds(Boid boid)
    {
        Vector3 position = boid.transform.position;

        if (position.x > maxX)
            position.x = position.x - maxX * 2 * 0.9f;
        else if (position.x < -maxX)
            position.x = position.x + maxX * 2 * 0.9f;

        if (position.z > maxZ)
            position.z = position.z - maxZ * 2 * 0.9f;
        else if (position.z < -maxZ)
            position.z = position.z + maxZ * 2 * 0.9f;

        if (position.y > maxY)
            position.y = position.y - maxY * 2 * 0.9f;
        else if (position.y < -maxY)
            position.y = position.y + maxY * 2 * 0.9f;

        boid.transform.position = position;
    }
}