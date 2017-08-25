using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour {

    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;

    public LayerMask unitMask;
    public LayerMask obstacleMask;

    public List<Transform> visibleUnits = new List<Transform>();

    public float meshResolution;
    public int edgeResolveIterations;
    public float edgeDistanceThreshold;
    public Material onAlertMaterial;
    public Material onNormalMaterial;
    public MeshFilter viewMeshFilter;
    Mesh viewMesh;
    LineRenderer lineRenderer;

    EnemyAI enmAI;

    void Start()
    {
        enmAI = GetComponent<EnemyAI>();
        //lineRenderer = GetComponent<LineRenderer>();

        //lineRenderer.enabled = false;

        viewMesh = new Mesh
        {
            name = "View Mesh"
        };
        viewMeshFilter.mesh = viewMesh;

        StartCoroutine("FindUnitsWithDelay", .2f);
    }

    void LateUpdate()
    {
        DrawFieldOfView();
        if (visibleUnits.Count > 0)
            SendTargetToEnemyAI();
    }

    IEnumerator FindUnitsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleUnits();
        }     
    }

    public void RemoveUnitFromVisibleUnits(CharacterStats unit)
    {
        if(visibleUnits.Count > 0)
        {
            for (int i = 0; i < visibleUnits.Count; i++)
            {
                if (visibleUnits[i].GetComponent<CharacterStats>() == unit)
                {
                    visibleUnits.Remove(visibleUnits[i]);
                }
                return;
            }
        }
    }

    void FindVisibleUnits()
    {
        for (int i = 0; i < visibleUnits.Count; i++)
        {
            if (visibleUnits[i].GetComponent<CharacterStats>().isRiding)
            {
                RemoveUnitFromVisibleUnits(visibleUnits[i].GetComponent<CharacterStats>());
            }
            visibleUnits[i].GetComponent<CharacterStats>().isBeenChased = false;
        }
        visibleUnits.Clear();

        Collider[] unitsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, unitMask);

        for (int i = 0; i < unitsInViewRadius.Length; i++)
        {
            Transform unit = unitsInViewRadius[i].transform;
            Vector3 dirToUnit = (unit.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToUnit) < viewAngle / 2)
            {
                float dstToUnit = Vector3.Distance(transform.position, unit.position);
                if(!Physics.Raycast(transform.position, dirToUnit, dstToUnit, obstacleMask))
                {
                    visibleUnits.Add(unit);
                }
            }
        }
    }

    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);
            if (i > 0)
            {
                bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
                if(oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if(edge.pointA != Vector3.zero)
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
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if(i < vertexCount - 2)
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

        if(enmAI.aiStates == EnemyAI.AIStates.patrol)
        {
            viewMeshFilter.gameObject.GetComponent<MeshRenderer>().material = onNormalMaterial;
        }
        else
        {
            viewMeshFilter.gameObject.GetComponent<MeshRenderer>().material = onAlertMaterial;
        }
    }

    void SendTargetToEnemyAI()
    {
        int randomValue = Random.Range(0, visibleUnits.Count);

        for (int i = 0; i < visibleUnits.Count; i++)
        {
            if (visibleUnits[i].GetComponent<CharacterStats>().isRiding)
            {
                RemoveUnitFromVisibleUnits(visibleUnits[i].GetComponent<CharacterStats>());
                enmAI.aiStates = EnemyAI.AIStates.search;
            }
            if(!visibleUnits[randomValue].GetComponent<CharacterStats>().isBeenChased)
            {
                enmAI.target = visibleUnits[randomValue].GetComponent<CharacterStats>();
                visibleUnits[randomValue].GetComponent<CharacterStats>().isBeenChased = true;
                //DrawLineTowardsTarget(visibleUnits[randomValue].position);
                Debug.Log(visibleUnits[randomValue].name + " is been chased by " + gameObject.name);
            }
        }
    }

    void DrawLineTowardsTarget(Vector3 target)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(1, target);
    }

    void EraseLine()
    {
        lineRenderer.enabled = false;
    }

    float CalculateDistanceToUnit(Transform unit)
    {
        return Vector3.Distance(transform.position, unit.position);
    }


    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = dirFromAngle(globalAngle, true);
        RaycastHit hit;

        if(Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDistanceThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDistanceThresholdExceeded)
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

    public Vector3 dirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle)
        {
            hit = _hit;
            point = _point;
            distance = _distance;
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