using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] Vector3 offset;
    [SerializeField] float deltaAngle = 10f;
    [SerializeField] float deltaDistance = 0.1f;
    public List<Vector3> positions = new List<Vector3>();
    [SerializeField] List<Transform> allPositions;


    [SerializeField] List<Color> colors;



    int count = 20;
    private void Update()
    {

        List<Vector3> positionsNew = new List<Vector3>();
        Color test = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        for (int i = 0; i < allPositions.Count; i++)
        {
            positionsNew.Add(allPositions[i].position);
            DrawCross(allPositions[i].position);
        }
        
        for (int i = 0; i < positionsNew.Count - 2; i++)
        {
            Debug.DrawLine(positionsNew[i], positionsNew[i+1], test);
        }

        List<Vector3> approximatedPositions = Qwerty(positionsNew);

        for (int i = 0; i < approximatedPositions.Count - 1; i++)
        {
            Debug.DrawLine(approximatedPositions[i], approximatedPositions[i + 1], colors[i % colors.Count]);
        }   
    }



    List<Vector3> Qwerty(List<Vector3> positions)
    {
        List<Vector3> positionsNew = new List<Vector3>();
        positionsNew.Add(positions[0]);
        positionsNew.AddRange(positions);

        bool isInitedStartPosition = false;
        bool isInitedSecondDrowPosition = false;

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


    void DrawCross(Vector3 position)
    {
        Vector3 one = new Vector3(1f, 0f, 1f);
        float size = 0.5f;
        Debug.DrawLine(position - new Vector3(size, 0f, -size), position + new Vector3(size, 0f, -size), Color.red);
        Debug.DrawLine(position - new Vector3(-size, 0f, size), position + new Vector3(-size, 0f, size), Color.red);
    }
}
