using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Follower : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, 0);
    public float Msmooth = 0.3f;
    public float Rsmooth = 0.3f;
    private Vector3 velocity = Vector3.zero;

    // Update is called once per frame
    IEnumerator Start ()
    {
        while (true)
        {
            if (target == null)
            {
                GameObject t = GameObject.FindGameObjectWithTag("Player");

                if (t != null)
                {
                  /*  if (t.GetComponent<NetworkBehaviour>().isLocalPlayer)
                    {
                        target = t.transform;
                    }*/
                }
            }
            else
            {
                transform.position = Vector3.SmoothDamp(transform.position, target.position + offset, ref velocity, Time.deltaTime * Msmooth);
                Vector3 lTargetDir = target.position - transform.position;
                lTargetDir.x = 0.0f;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lTargetDir), Time.time * Rsmooth);
            }
            yield return null;
        }
    }
}
