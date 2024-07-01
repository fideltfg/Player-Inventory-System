using UnityEngine;

public class Jumper : MonoBehaviour
{
    public int gridSize = 5; // Size of the grid

    private Vector3 startPosition;

    void OnEnable()
    {
        startPosition = transform.position;
        MoveObjectToRandomPoint();
        this.enabled = false; // Disable the component after the move
    }

    private void MoveObjectToRandomPoint()
    {
        Vector3 targetPosition = GetRandomGridPoint();
        transform.position = targetPosition;
    }

    private Vector3 GetRandomGridPoint()
    {
        Vector3 newPosition;
        do
        {
            int x = Random.Range(-gridSize, gridSize); // Includes -gridSize to gridSize
            int z = Random.Range(-gridSize + 1, gridSize); // Includes -gridSize to gridSize
            newPosition = new Vector3(x, startPosition.y, z); // Assuming movement in the x-z plane
        } while (newPosition == transform.position);

        return newPosition;
    }
}
