using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[System.Serializable]
public class TrailPoint
{
    public float timeCreated = 0.0f;
    public Vector3 basePosition;
    public Vector3 rotationVector;
    public List<Vector3> circlePoints;
}



public class CarTrail : MonoBehaviour
{
    #region Fields

    [SerializeField] Material material;
    [SerializeField] float lifeTime;

    [SerializeField] float coneCoef;
    [SerializeField] List<Transform> dots;
    bool emit = true;

    List<TrailPoint> points = new List<TrailPoint>();
    [SerializeField] Vector3 colliderSize;
    [SerializeField] Vector3 colliderCenter;


    int collidersPoolCount = 2;
    int addOnceToPool = 10;
    int maxColliders = 500;

    GameObject trailObject;
    Mesh trailMesh;
    Material trailMaterial;

    Vector3 radiusVector;
    List<Vector3> vertexList;
    Quaternion quaternionStep;
    Player owner;

    float _radius;
    bool _isInitialized;
    Gradient _gradient;

    Transform _baseAnchor;

    Vector3[] newVertices = new Vector3[5];

    Vector2[] newUV = new Vector2[5];
    int[] newTriangles = new int[5];
    Color[] colors = new Color[5];
    TrailTrigger trailTrigger;
    
    List<BoxCollider> spawnedBoxColliders = new List<BoxCollider>();
    #endregion


    #region Properties

    public bool Emit
    {
        get
        {
            return emit;
        }
        set
        {
            emit = value;
        }
    }

    public bool IsActiveTrail
    {
        get
        {
            return (trailObject != null) ? trailObject.activeInHierarchy : false;
        }

        set
        {
            if (trailObject != null && value != trailObject.activeInHierarchy)
            {
                trailObject.SetActive(value);
            }
        }
    }

    public Material CurrentMaterial
    {
        get
        {
            return trailMaterial;
        }
        set
        {
            if (value != trailMaterial)
            {
                trailMaterial = value;
                trailObject.GetComponent<Renderer>().material = trailMaterial;
            }
        }
    }

    #endregion


    #region Unity lifecycle

    BoxCollider trailTriggerPrefab = null;

    void OnEnable()
    {
    }


    void InitializeCollidersPool()
    {
        for (int i = 0; i < collidersPoolCount; i++)
        {
            spawnedBoxColliders.Add(Instantiate<BoxCollider>(trailTriggerPrefab, transform));
        }
    }


    bool AddItemsToPool()
    {
        bool isAdded = false;
        if (spawnedBoxColliders.Count < maxColliders)
        {
            for (int i = 0; i < addOnceToPool; i++)
            {
                isAdded = true;
                spawnedBoxColliders.Add(Instantiate<BoxCollider>(trailTriggerPrefab, transform));
            }
        }
        return isAdded;
    }
    
    

    int n = 10;
    [SerializeField] List<Color> colorss;

    [SerializeField] float deltaAngle = 60f;
    public void CustomUpdate(float deltaTime)
    {
        if (_isInitialized)
        {

            int numPoints = dots.Count;
            if (Emit)
            {
                TrailPoint p = new TrailPoint();
                p.basePosition = _baseAnchor.position;
                p.circlePoints = MakeCircle();
                p.rotationVector = _baseAnchor.rotation.eulerAngles;
                p.timeCreated = Time.time;
                points.Add(p);
            }

            RemoveOldPoints(points);

            if (points.Count == 0)
            {
                trailMesh.Clear();
            }


            if (points.Count > 0)
            {
                newVertices = new Vector3[points.Count * numPoints];
                newUV = new Vector2[points.Count * numPoints];
                newTriangles = new int[points.Count * (numPoints * 6 + 6)]; //+6
                colors = new Color[points.Count * numPoints];

                int trianglesCounter = 0;

                float uRatio = 1.0f / (float)(points.Count);
                float vRatio = 1.0f / (points.Count);
                float colorPart = 1.0f / points.Count;

                float coneDecresCoef = 1f;//(1.0f - coneCoef) / points.Count;

                Vector3[] prevPos = null;

                trianglesCounter = (numPoints) * (points.Count) * 6 - 1;

                List<Vector3> basePoints = new List<Vector3>();
                basePoints.Add(points[0].basePosition);
                for (int i = 0; i < points.Count; i++)
                {
                    basePoints.Add(points[i].basePosition);
                }

                List<Vector3> approximatedPositions = Qwerty(basePoints);

                for (int i = 0; i < approximatedPositions.Count - 1; i++)
                {
                    Debug.DrawLine(approximatedPositions[i], approximatedPositions[i + 1], Color.red);
                }


                for (int i = points.Count - 1; i >= 0; i--)
                {
                    TrailPoint p = points[i];

                    Color curColor = _gradient.Evaluate((points.Count - i) * colorPart);

                    for (int j = numPoints - 1; j >= 0; j--)
                    {
                        Vector3 currentPoint = p.circlePoints[j];
                        Vector3 newPoint = Quaternion.Euler(p.rotationVector) * currentPoint + p.basePosition;
                        newVertices[i * numPoints + j] = newPoint;
                        newUV[i * numPoints + j] = new Vector2((numPoints - j) * uRatio, (points.Count - i) * vRatio);

                        colors[i * numPoints + j] = curColor;

                        if (j % n == 0)
                        {
                            int index = j / (numPoints - n);
                            if (prevPos != null)
                            {
                                Debug.DrawLine(prevPos[index], newPoint, Color.blue);
                            }
                            else
                            {
                                prevPos = new Vector3[n];
                            }

                            prevPos[index] = newPoint;
                        }
                    }

                    if (i > 0)
                    {
                        for (int j = numPoints - 2; j >= 0; j--)
                        {
                            newTriangles[trianglesCounter--] = (i - 1) * numPoints + j;
                            newTriangles[trianglesCounter--] = (i - 1) * numPoints + (j + 1);
                            newTriangles[trianglesCounter--] = i * numPoints + j;

                            newTriangles[trianglesCounter--] = (i - 1) * numPoints + (j + 1);
                            newTriangles[trianglesCounter--] = i * numPoints + (j + 1);
                            newTriangles[trianglesCounter--] = i * numPoints + j;
                        }

                        newTriangles[trianglesCounter--] = (i - 1) * numPoints;
                        newTriangles[trianglesCounter--] = i * numPoints;
                        newTriangles[trianglesCounter--] = (i - 1) * numPoints + (numPoints - 1);

                        newTriangles[trianglesCounter--] = i * numPoints;
                        newTriangles[trianglesCounter--] = i * numPoints + (numPoints - 1);
                        newTriangles[trianglesCounter--] = (i - 1) * numPoints + (numPoints - 1);
                    }
                }
                
                if (approximatedPositions.Count > spawnedBoxColliders.Count)
                {
                    bool isAdded = false;
                    do
                    {
                        isAdded = AddItemsToPool();
                    } while (!isAdded || approximatedPositions.Count > spawnedBoxColliders.Count);
                }

                Vector3 startDrowPosition = Vector3.zero;
                Vector3 preLastDrwPosition = Vector3.zero;
                int collidersCount = spawnedBoxColliders.Count;
                int lastIndex = 0;
                Vector3 distance = Vector3.zero;
                BoxCollider currentBoxCollider = null;
                for (lastIndex = 0; lastIndex < approximatedPositions.Count - 1 - 1; lastIndex++)
                {
                    if (lastIndex <= collidersCount)
                    {
                        startDrowPosition = approximatedPositions[lastIndex];
                        preLastDrwPosition = approximatedPositions[lastIndex + 1];

                        distance = preLastDrwPosition - startDrowPosition;
                        currentBoxCollider = spawnedBoxColliders[lastIndex];
                        currentBoxCollider.transform.position = startDrowPosition;
                        currentBoxCollider.transform.LookAt(preLastDrwPosition);
                        currentBoxCollider.size = new Vector3(colliderSize.x, colliderSize.y, distance.magnitude);
                        currentBoxCollider.center = new Vector3(colliderCenter.x, colliderCenter.y, 0.5f * distance.magnitude);
                        currentBoxCollider.enabled = true;
                    }
                    else
                    {
                        lastIndex--;
                        break;
                    }
                }

                for (int i = lastIndex; i < collidersCount; i++)
                {
                    if (!spawnedBoxColliders[i].enabled)
                    {
                        break;
                    }
                    spawnedBoxColliders[i].enabled = false;
                }

                trailMesh.Clear();

                trailMesh.vertices = newVertices;
                trailMesh.uv = newUV;
                trailMesh.colors = colors;
                trailMesh.triangles = newTriangles;
                
                trailMesh.RecalculateNormals();
            }
        }
    }




    #endregion

    #region Public methods

    public void Initialize(Transform anchor, Player owner)
    {
        this.owner = owner;
        _baseAnchor = anchor;
        _gradient = new Gradient();


        IsActiveTrail = true;

        if (trailTriggerPrefab == null)
        {
            GameObject colliderGameObject = new GameObject("ColliderItem");
            colliderGameObject.transform.SetParent(transform);
            TrailTrigger trailTrigger = colliderGameObject.AddComponent<TrailTrigger>();

            trailTriggerPrefab = colliderGameObject.AddComponent<BoxCollider>();
            trailTriggerPrefab.isTrigger = true;
            trailTriggerPrefab.enabled = false;
            trailTrigger.GameObject = owner.gameObject;
        }

        InitializeCollidersPool();

        CreateTrail();

        _isInitialized = true;
    }

    #endregion


    #region Private methods

    void RemoveOldPoints(List<TrailPoint> pointList)
    {
        List<TrailPoint> remove = new List<TrailPoint>();


        for (int i = 0; i < pointList.Count; i++)
        {
            TrailPoint p = pointList[i];

            if (Time.time - p.timeCreated > lifeTime)
            {
                remove.Add(p);
            }
        }

        foreach (TrailPoint p in remove)
        {
            pointList.Remove(p);
        }
    }


    void CreateTrail()
    {
        trailObject = new GameObject("Trail");
        trailObject.transform.position = Vector3.zero;
        trailObject.transform.rotation = Quaternion.identity;
        trailObject.transform.localScale = Vector3.one;
        trailObject.AddComponent(typeof(MeshFilter));
        trailObject.AddComponent(typeof(MeshRenderer));
        trailObject.layer = Layers.TRIGGERS;

        trailObject.AddComponent(typeof(TrailTrigger));
        trailTrigger = trailObject.GetComponent<TrailTrigger>();
        

        radiusVector = new Vector3(0.0f, _radius, 0.0f);
        vertexList = new List<Vector3>(dots.Count);

        float angleStep = 360.0f / (float)dots.Count;

        quaternionStep = Quaternion.Euler(0.0f, 0.0f, angleStep);

        CurrentMaterial = new Material(material);

        trailMesh = new Mesh();
        trailObject.GetComponent<MeshFilter>().mesh = trailMesh;
    }


    public void DestroyTrail()
    {
        IsActiveTrail = false;
        spawnedBoxColliders.ForEach((item) =>
        {
            Destroy(item.gameObject);
        });
        spawnedBoxColliders.Clear();

        Destroy(trailObject);
        Destroy(trailMesh);
        points.Clear();
        _isInitialized = false;
    }

    List<Vector3> MakeCircle()
    {
        vertexList.Clear();

        // vertexList.Add(radiusVector);

        for (int i = 0; i < dots.Count; i++)
        {
            vertexList.Add(dots[i].localPosition);
        }

        return vertexList;
    }

    [SerializeField] float deltaDistance;

    List<Vector3> Qwerty(List<Vector3> positionsNew)
    {
        List<Vector3> approximatedPositions = new List<Vector3>();

        Vector3 startDrowPosition = positionsNew[positionsNew.Count - 1];
        Vector3 secondDrowPosition = positionsNew[positionsNew.Count - 2];
        Vector3 lastDrowPosition = Vector3.zero;
        Vector3 preLastDrwPosition = Vector3.zero;

        approximatedPositions.Add(positionsNew[positionsNew.Count - 1]);
        for (int i = positionsNew.Count - 1 - 2; i >= 0; i--)
        {
            Vector3 currentPositon = positionsNew[i];
            Vector3 prevPosition = positionsNew[i + 1];

            preLastDrwPosition = prevPosition;
            lastDrowPosition = currentPositon;

            Vector3 directionFromStartDrowToSecondPoint = (secondDrowPosition - startDrowPosition);
            //XUI
            if (Mathf.Approximately(directionFromStartDrowToSecondPoint.magnitude, 0f))
            {
                startDrowPosition = secondDrowPosition;
                secondDrowPosition = currentPositon;
            }
            else
            {
                Vector3 directionFromStartDrowToLastPoint = (lastDrowPosition - startDrowPosition);

                float angle = Vector3.Angle(directionFromStartDrowToSecondPoint, directionFromStartDrowToLastPoint);

                if ((Mathf.Abs(angle) > deltaAngle || i == 0))
                {
                    Vector3 distance = startDrowPosition - prevPosition;
                    if (distance.magnitude > deltaDistance)
                    {
                        approximatedPositions.Add(prevPosition);
                        startDrowPosition = prevPosition;
                        secondDrowPosition = currentPositon;
                    }
                }
            }
        }

        return approximatedPositions;
    }

    #endregion


    #region IPoolCallback

    public void OnCreateFromPool()
    {
    }

    public void OnReturnToPool()
    {
        _isInitialized = false;
        DestroyTrail();
    }

    public void OnPop()
    {

    }
    public void OnPush()
    {

    }

    #endregion


}
