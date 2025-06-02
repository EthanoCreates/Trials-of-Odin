using System.Collections.Generic;
using UnityEngine;

public class AiSensor : MonoBehaviour
{
    [SerializeField] private EnemyStateMachine enemyStateMachine;

    [SerializeField] private float distance = 10;
    //radial segments of sensor, allow a smoother curve
    [SerializeField] private int segments = 10;
    //fov
    [SerializeField] private float angle = 30;
    [SerializeField] private float height = 1.0f;
    [SerializeField] private Color meshColor = Color.cyan;
    [SerializeField] private int scanFrequency = 30;
    //layers sensor looks for
    [SerializeField] private LayerMask layers;
    //Layers that will hide object from line of sight
    [SerializeField] private LayerMask occlusionLayers;

    //sensor mesh
    Mesh mesh;

    //all objects inside sphere
    Collider[] colliders = new Collider[50];
    //all game object inside sensor
    [SerializeField] private List<GameObject> Objects = new();
    public List<GameObject> Players = new();    
    int count;
    float scanInterval;
    float scanTimer;

    // Start is called before the first frame update
    void Start()
    {
        if(enemyStateMachine.SpawningForBossArena) Destroy(gameObject);
        scanInterval = 1f / scanFrequency;
    }

    // Update is called once per frame
    void Update()
    {
        scanTimer -= Time.deltaTime;
        if(scanTimer < 0)
        {
            scanTimer += scanInterval;
            Scan();
        }
    }

    private void Scan()
    {
        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layers, QueryTriggerInteraction.Collide);

        Objects.Clear();
        Players.Clear();
        for(int i = 0; i < count; i++)
        {
            GameObject obj = colliders[i].gameObject;
            if(obj.CompareTag("Player"))
            {
                if(IsInSight(obj,Vector3.up))
                {
                    Players.Add(obj);
                }
            }
            else if(IsInSight(obj, Vector3.zero))
            {
                Objects.Add(obj);
            }
        }
        enemyStateMachine.EnemyContext.NearPlayers = Players;
    }

    private bool IsInSight(GameObject obj, Vector3 originDest)
    {
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        if (obj.CompareTag("Player")) dest = obj.transform.position + originDest;
        Vector3 direction = dest - origin;
        if(direction.y < 0 || direction.y > height)
        {
            return false;
        }

        direction.y = 0;

        float deltaAngle = Vector3.Angle(direction, transform.forward);
        if (deltaAngle > angle) return false;

        origin.y += height / 2;
        dest.y = origin.y;
        if(Physics.Linecast(origin, dest, occlusionLayers))
        {
            return false;
        }

        return true;
    }


    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();

        int numTriangles = (segments * 4) + 2 + 2;
        int numVertices  = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0f, -angle, 0f) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * distance;

        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;

        int vert = 0;

        //left side

        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;


        //right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;

        for(int i = 0; i < segments; i++)
        {

            bottomLeft = Quaternion.Euler(0f, currentAngle, 0f) * Vector3.forward * distance;
            bottomRight = Quaternion.Euler(0f, currentAngle + deltaAngle, 0f) * Vector3.forward * distance;


            topRight = bottomRight + Vector3.up * height;
            topLeft = bottomLeft + Vector3.up * height;

            currentAngle += deltaAngle;

        //far side
        vertices[vert++] = bottomLeft;
        vertices[vert++] = bottomRight;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = topLeft;
        vertices[vert++] = bottomLeft;


        //top
        vertices[vert++] = topCenter;
        vertices[vert++] = topLeft;
        vertices[vert++] = topRight;

        //bottom

        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomLeft;

        }

        for(int i = 0; i < numVertices; i++)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void OnValidate()
    {
        mesh = CreateWedgeMesh();
        scanInterval = 1f / scanFrequency;
    }

    private void OnDrawGizmos()
    {
        if(mesh)
        {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
        }


        Gizmos.DrawWireSphere(transform.position, distance);
        Gizmos.color = Color.red;
        for (int i = 0; i < count; i++)
        {
            Gizmos.DrawSphere(colliders[i].transform.position + Vector3.up * 4, .2f);
        }

        Gizmos.color = Color.green;
        foreach (var obj in Players)
        {
            Gizmos.DrawSphere(obj.transform.position + Vector3.up * 4, .2f);
        }
    }

}
