using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour
{
    private Rigidbody r;

    private void Awake()
    {
        transform.TryGetComponent(out r);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Plane"))
        {
            r.isKinematic = true;
            nomalrize_position();
        }
    }

    public void nomalrize_position()
    {

        float parent_y = -transform.parent.transform.position.y;
        float postion_y = transform.localScale.y * 0.5f;
        float y = parent_y + postion_y;

        transform.localPosition = new Vector3
            (
            transform.localPosition.x,
            y,
            transform.localPosition. z
            );



    }

    public void set_Kinematic(bool isk)
    {
        r.isKinematic = isk;
    }
}
