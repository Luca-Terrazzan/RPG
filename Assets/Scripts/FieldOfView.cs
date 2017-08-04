using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour {

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public LayerMask lowObstacleMask;

    public List<Transform> visibleTargets = new List<Transform>();
    Transform target;

    // private Bracciante bracciante;
    // private RagazzoMucca cowBoy;
    public Vector3 lastPlayerSeenPoint;
    public Transform playerTransform;

    public float meshResolution;
    public int edgeResolveIterations;
    public float edgeDstThreshold;

    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    private PlayerActions player;
    private Animator animPlayer;

    private AudioSource allarm;
    
    public AudioClip allarmSound;

    private bool allarmTrigger = true;



    void Start()
    {
        allarm = GetComponent<AudioSource>();
        animPlayer = GameObject.Find("WakandaSprite").GetComponent<Animator>();
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        playerTransform = GameObject.FindWithTag("Player").transform;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerActions>();

    }

    void LateUpdate()
    {
        DrawFieldOfView();

    }

    private void Update()
    {
        
        
        if (((AmIHearingPlayer() || FindVisibleTarget()) && allarmTrigger ))
        {
            if (!animPlayer.GetBool("isMoving"))
            {
                allarm.clip = allarmSound;
                allarm.Play();
                allarmTrigger = false;
            }
        }
    }
   
   
    public bool AmIHearingPlayer()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(new Vector3(transform.position.x, transform.position.y, transform.position.z), viewRadius, playerMask);

        if (targetsInViewRadius.Length > 0)
        {
            if (targetsInViewRadius[0].GetComponent<PlayerActions>().canBeHeard)
            {
                
                return true;
            }
            else
            {
                
                return false;
            }


        }
        else
        {
            allarmTrigger = true;
            return false;
        }
    }

    public bool FindVisibleTarget()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(new Vector3(transform.position.x,transform.position.y,transform.position.z), viewRadius,playerMask);

        if (targetsInViewRadius.Length > 0)
        {

            visibleTargets.Clear();
            target = targetsInViewRadius[0].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.up, dirToTarget) < viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                RaycastHit hit;



                if (!Physics.Raycast(transform.position, dirToTarget, out hit, distToTarget, obstacleMask) && !Physics.Raycast(transform.position, dirToTarget, out hit, distToTarget, lowObstacleMask))
                {
                    visibleTargets.Add(target);

                    return true;
                }
                else
                {
                    //ciao
                    if (Physics.Raycast(transform.position, dirToTarget, out hit, distToTarget, lowObstacleMask) && !Physics.Raycast(transform.position, dirToTarget, out hit, distToTarget, obstacleMask))
                    {
                        if (player.lowInvisible && player.isCrouched)
                        {
                            Debug.Log("non ti vedo piu porcodio");

                            return false;
                        }
                        else
                        {
                            Debug.Log("non sei abbassato/vicino a low obstacle");
                            return true;
                        }
                    }
                    else
                    {

                        return false;
                    }


                    /*float distToObstacle = Vector3.Distance(player.transform.position, hit.collider.transform.position);
                    
                    if ( hit.collider.tag != "LowObstacle")
                    {
                        return false;
                    }
                    if (hit.collider.tag == "LowObstacle" && distToObstacle >= 2f)
                    {
                        visibleTargets.Add(target);
                        return true;
                    }
                    else if (hit.collider.tag == "LowObstacle" && distToObstacle < 1.5f && player.isCrouched)
                    {
                        
                        return false;
                    }
                    else
                    {
                       
                        return true;
                    }*/
                }


            }
            else return false;
        }
        else
        {
            allarmTrigger = true;
            return false;
        }
        
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(meshResolution * viewAngle);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.z - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if(i>0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst)>edgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit || oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded)
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount-2)*3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }          
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for(int i=0;i<edgeResolveIterations;i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;

            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;
        if (Physics.Raycast(transform.position,dir,out hit,viewRadius,obstacleMask) )     //(Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            if (hit.collider.tag == "LowObstacle")
            {
                return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
            }
            else
            {
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
            }
        }
        else
        {
            return new ViewCastInfo(false, transform.position+dir*viewRadius, viewRadius, globalAngle);
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

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
