using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    public bool trigger;
    public string[] killTags;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!trigger)
        {
            if (other.gameObject.GetComponent<Tags>())
            {
                foreach (string tag in other.gameObject.GetComponent<Tags>().tags)
                {
                    foreach (string kt in killTags)
                    {
                        if (tag == kt)
                        {
                            Destroy(other.gameObject);
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (trigger)
        {
            if (other.gameObject.GetComponent<Tags>())
            {
                foreach (string tag in other.gameObject.GetComponent<Tags>().tags)
                {
                    foreach (string kt in killTags)
                    {
                        if (tag == kt)
                        {
                            Destroy(other.gameObject);
                        }
                    }
                }
            }
        }
    }
}
