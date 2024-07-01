using PlayerInventorySystem;
using System.Collections;
using UnityEngine;

public class Popper : MonoBehaviour
{
    public Transform childObject; // Assign the child object in the Inspector
    public float moveDuration = 0.05f; // Duration for moving up and down
    public float minWaitTime = 0.5f; // Minimum random wait time
    public float maxWaitTime = 2.0f; // Maximum random wait time

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private Jumper jumper;

    void Start()
    {
        if (childObject == null)
        {
            Debug.LogError("Child object is not assigned.");
            return;
        }


        jumper = GetComponent<Jumper>();
        jumper.enabled = true; // move to a random position in the grid

        startPosition = childObject.localPosition;
        targetPosition = startPosition + Vector3.up;

        StartCoroutine(MoveObject());
    }

    private IEnumerator MoveObject()
    {
        while (true)
        {
            float waitTime = Random.Range(moveDuration, maxWaitTime);
            yield return new WaitForSeconds(waitTime * .5f);
            yield return StartCoroutine(SmoothMove(childObject.localPosition, targetPosition, moveDuration));
            yield return new WaitForSeconds(waitTime * .5f);
            popItem();
            yield return new WaitForSeconds(waitTime * .5f);
            yield return StartCoroutine(SmoothMove(childObject.localPosition, startPosition, moveDuration));
            jumper.enabled = true; // move to a random position in the grid
            yield return new WaitForSeconds(waitTime * .5f);
        }
    }


    private void popItem()
    {
        // spawn an item here in a random driecton
        // pick a random item from the item catalog and place it on the grid
        Item item = InventoryController.Instance.ItemCatalog.GetRandomItem();

        if (item != null)
        {

            // place the item on the grid at a random position
            GameObject itemObject = InventoryController.Instance.SpawnItem(item.Data.id, transform.position + new Vector3(.5f, .25f, .5f), 1);
            itemObject.GetComponent<DroppedItem>().TimeToLive = Random.Range(5, 8);
            // apply a force to the item that will make it move in a random direction, + or - x and z axis but only in the positive y axis
            Rigidbody rb = itemObject.GetComponent<Rigidbody>();
           
            // add a random rotation to the item to make it spin end over end
            rb.AddTorque(new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), Random.Range(-3, 3)), ForceMode.Impulse);
            rb.AddForce(new Vector3(Random.Range(-3, 3), 1, Random.Range(-3, 3)), ForceMode.Impulse);

        }
    }

    private IEnumerator SmoothMove(Vector3 start, Vector3 end, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            childObject.localPosition = Vector3.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        childObject.localPosition = end;
    }
}
