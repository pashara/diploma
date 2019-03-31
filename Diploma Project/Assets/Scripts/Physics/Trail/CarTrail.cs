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

    GameObject trailObject;
    Mesh trailMesh;
    Material trailMaterial;

    Vector3 radiusVector;
    List<Vector3> vertexList;
    Quaternion quaternionStep;


    float _radius;
    bool _isInitialized;
    Gradient _gradient;

    Transform _baseAnchor;

    Vector3[] newVertices = new Vector3[5];

    Vector2[] newUV = new Vector2[5];
    int[] newTriangles = new int[5];
    Color[] colors = new Color[5];
    MeshCollider meshCollider;
    TrailTrigger trailTrigger;

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

    void OnEnable()
    {
        IsActiveTrail = true;
    }

    void OnDisable()
    {
        IsActiveTrail = false;
    }

    // [SerializeField] bool outPlayer = false;
    int n = 10;
    void LateUpdate()
    {
        if (!_isInitialized)
        {
            return;
        }
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
            newVertices = new Vector3[points.Count * dots.Count];
            newUV = new Vector2[points.Count * dots.Count];
            newTriangles = new int[points.Count * (dots.Count * 6 + 6)]; //+6
            colors = new Color[points.Count * dots.Count];           

            int trianglesCounter = 0;

            float uRatio = 1.0f / (float)(points.Count);
            float vRatio = 1.0f / (points.Count);
            float colorPart = 1.0f / points.Count;

            float coneDecresCoef = 1f;//(1.0f - coneCoef) / points.Count;

            Vector3[] prevPos = null;
            
                trianglesCounter = (numPoints ) * (points.Count) * 6 - 1;
                for (int i = points.Count - 1; i >= 0; i--)
                {
                    TrailPoint p = points[i];

                    Color curColor = _gradient.Evaluate((points.Count - i) * colorPart);

                    for (int j = numPoints - 1; j >= 0; j--)
                    {
                        Vector3 currentPoint = p.circlePoints[j];
                        Vector3 newPoint = Quaternion.Euler(p.rotationVector) * currentPoint + p.basePosition;// ((currentPoint) * (coneCoef + i * coneDecresCoef))  + p.basePosition;
                        // Vector3 newPoint = ((currentPoint - p.basePosition) * (coneCoef + i * coneDecresCoef)) + p.basePosition;
                        newVertices[i * numPoints + j] = newPoint;
                        newUV[i * numPoints + j] = new Vector2((numPoints - j) * uRatio, (points.Count - i) * vRatio);

                        colors[i * numPoints + j] = curColor;
                        
                        if (j%n == 0)
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


            // for(int i = 0; i < newTriangles.Length - 1; i++)
            // {
            //     Debug.DrawLine(newVertices[newTriangles[i]],newVertices[newTriangles[i + 1]], Color.red);
            // }

            trailMesh.Clear();

            trailMesh.vertices = newVertices;
            trailMesh.uv = newUV;
            trailMesh.colors = colors;
            trailMesh.triangles = newTriangles;

            if (meshCollider != null)
            {
                meshCollider.sharedMesh = trailMesh; 
            }

            trailMesh.RecalculateNormals();
        }
    }



    #endregion

    #region Public methods

    public void Initialize(Transform anchor)
    {
        _baseAnchor = anchor;
        _gradient = new Gradient();
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
        trailObject.AddComponent(typeof(MeshCollider));
        meshCollider = trailObject.GetComponent<MeshCollider>();
        trailObject.layer = Layers.TRIGGERS;

        trailObject.AddComponent(typeof(TrailTrigger));
        trailTrigger = trailObject.GetComponent<TrailTrigger>();
        
        // meshCollider.convex = true;
        // meshCollider.isTrigger = true;

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
