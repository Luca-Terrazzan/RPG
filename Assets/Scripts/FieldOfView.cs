using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour {

    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;

    public LayerMask playerMask;
    public LayerMask obstacleMask;

    public List<Transform> visibleTargets = new List<Transform>();
    Transform target;
    private Bracciante bracciante;

    void Start()
    {
        StartCoroutine("FindTargetsWithDelay", 0f);
        bracciante = GetComponent<Bracciante>();
        
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return null;
            FindVisibleTarget();
        }
    }

    void FindVisibleTarget()
    {
        Collider2D targetsInViewRadius = Physics2D.OverlapCircle(new Vector2(transform.position.x,transform.position.y), viewRadius,playerMask);

        if (targetsInViewRadius != null)
        {

            visibleTargets.Clear();

                target = targetsInViewRadius.transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.up, dirToTarget) < viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics2D.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                    bracciante.stopPatroling = true;

                }
            }
        }
        
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.z;
        }
        return new Vector3(-Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
