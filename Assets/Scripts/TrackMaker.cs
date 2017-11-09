using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TrackMaker : MonoBehaviour
{
    //Let's say we have a line renderer...
    [SerializeField]
    LineRenderer m_lr, m_lr2;
    [SerializeField]
    float noise = 1;
    [SerializeField]
    [Range(0, 7)]
    int noiseIterations;
    [SerializeField]
    [Range(0, 25)]
    int smoothness = 1;
    [SerializeField]
    float m_verticalSinFrequency;
    [SerializeField]
    float m_verticalComp;

    [SerializeField]
    Transform car;

    [SerializeField]
    MeshExtruder m_roadPrefab;

    void Start()
    {
        GenerateTrack();
        StartCoroutine(CreateTrackMesh());
    }

    List<MeshExtruder> roadSections = new List<MeshExtruder>();

    public IEnumerator  CreateTrackMesh()
    {
        transform.localScale = Vector3.one;
        foreach (MeshExtruder i in roadSections)
        {
            Destroy(i.gameObject);
        }
        roadSections.Clear();

        Vector3[] points = new Vector3[ m_lr.positionCount];
        m_lr.GetPositions(points);

        //Get line distance and such
        float distance = 0;
        float[] distanceAtIndex = new float[points.Length];
        distanceAtIndex[0] = 0;
        for (int i = 0; i < points.Length - 1; i++)
        {
            distance += Vector3.Distance(points[i], points[i + 1]);
            distanceAtIndex[i + 1] = distance;
        }

        for (int i = 1; i < m_lr.positionCount; i++)
        {
            Vector3 pointVector = points[i] - points[GetCorrectedPoint(i - 1, points)];
            float angle = Mathf.Atan2(pointVector.x, pointVector.z) * Mathf.Rad2Deg;


            MeshExtruder newME = Instantiate(m_roadPrefab, points[i], Quaternion.Euler(-90, 0, angle), transform);
            newME.width = 2 + (Mathf.Sin(
                (2f * 5f * Mathf.PI)
                * (distanceAtIndex[i] / distance)));
            
            newME.m_lt.index = i - 1;

            if (roadSections.Count > 0)
                roadSections[roadSections.Count - 1].NextSection = newME;
            
            roadSections.Add(newME);
            if (i > m_lr.positionCount - 2)
                roadSections[roadSections.Count - 1].NextSection = roadSections[0];                
        }

        RandomiseColours(Random.value);

        yield return new WaitForSeconds(.2f);
        roadSections[roadSections.Count - 1].AddFinishLineTexture();

        roadSections[0].m_flag.transform.localScale = Vector3.one * roadSections[0].width;
        roadSections[0].m_flag.SetActive(true);

        transform.localScale = Vector3.one * 5;
        car.gameObject.SetActive(true);
        car.position = roadSections[0].transform.position + (Vector3.up * 3);
        car.LookAt(roadSections[1].transform.position);
    }

    #region colours

    public  void RandomiseColours(float r)
    {
        floor.val = r;
        for (int i = 0; i < roadSections.Count - 2; i++)
        {
            MeshExtruder m = roadSections[i];
            //RandomiseRendererColour(m.m_roadRenderer, r);
            RandomiseRendererColour(m.m_barrierRendererA, r);
            RandomiseRendererColour(m.m_barrierRendererB, r);
            RandomiseRendererColour(m.m_lowerRenderer, r);
        }
    }

    void RandomiseRendererColour(MeshRenderer _renderer, float r)
    {
        Texture2D tex = _renderer.material.mainTexture as Texture2D;
        //Texture2D tex = new Texture2D(original.width, original.height);
        //tex.filterMode = FilterMode.Point;

        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                Color col = tex.GetPixel(x, y);
                float hue, sat, val;
                Color.RGBToHSV(col, out hue, out sat, out val);

                hue = r;
                tex.SetPixel(x, y, Color.HSVToRGB(hue, sat, val));
            } 
        }
        tex.Apply();
        _renderer.material.mainTexture = tex;
    }

    #endregion

    #region line generation

    public void GenerateTrack()
    {
        #region Generate points
        //How many points?
        int pointCount = Random.Range(10, 15);

        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < pointCount * 2; i++)
        {
            points.Add(new Vector3(
                    Random.Range(0, 125) - 66,
                    0,//  Random.Range(0, 10) - 5,
                    Random.Range(0, 150) - 75
                ));
            Debug.DrawLine(points[i], points[i] + Vector3.up, Color.red, 3);
        }
        #endregion

        //Define the most basic of tracks
        List<Vector3> newPoints = ConvexHull.GenerateConvexHull(points);

        //Add sum nois
        for (int i = 0; i < noiseIterations; i++)
        {
            newPoints = ScatterMidPoints(newPoints);
        }

        Vector3[] curvedPoints = newPoints.ToArray();
        #region Smooth out curve
        m_lr2.positionCount = curvedPoints.Length;// + 1;
        m_lr2.SetPositions(curvedPoints);
        for (int i = 0; i < smoothness; i++)
        {
            curvedPoints = CurveLine(new List<Vector3>(curvedPoints));
        }
        List<Vector3> scum = new List<Vector3>(curvedPoints);
        scum.Add(curvedPoints[0]);
        curvedPoints = scum.ToArray();
        #endregion

        //Get line distance and such
        float distance = 0;
        float[] distanceAtIndex = new float[curvedPoints.Length];
        distanceAtIndex[0] = 0;
        for (int i = 0; i < curvedPoints.Length - 1; i++)
        {
            distance += Vector3.Distance(curvedPoints[i], curvedPoints[i + 1]);
            distanceAtIndex[i + 1] = distance;
        }


        //Add a bit of height to this mf
        for (int i = 0; i < curvedPoints.Length; i++)
        {
            curvedPoints[i].y = Mathf.Sin(
                (2 * m_verticalSinFrequency * Mathf.PI)
                * (distanceAtIndex[i] / distance))
            * m_verticalComp;
        }

        m_lr.positionCount = curvedPoints.Length;// + 1;
        m_lr.SetPositions(curvedPoints);
    }

    List<Vector3> ScatterMidPoints(List<Vector3> newPoints)
    {
        List<Vector3> DoublePoints = new List<Vector3>();
        for (int i = 0; i < newPoints.Count - 1; i++)
        {
            DoublePoints.Add(newPoints[i]);

            //Do this based on normal instead
            float r = Random.Range(0, 2) - 1;

            Vector3 vec = newPoints[i + 1] - newPoints[i];
            if (vec.magnitude > noise)
            {
                vec.Normalize();
                vec = new Vector3(-vec.z, 0, vec.x);

                DoublePoints.Add(Vector3.Lerp(newPoints[i], newPoints[i + 1], 0.5f)
                    + (r * vec * noise) 
                );
            }
        } 
        DoublePoints.Add(newPoints[0]);
        return DoublePoints;
    }

    Vector3[] CurveLine(List<Vector3> _OGpoints)
    {
        List<Vector3> returnMe = new List<Vector3>();

        for (int i = 0; i < _OGpoints.Count + 1; i++)
        {
            Vector3 pAminus1 = _OGpoints[GetCorrectedPoint(i - 1, _OGpoints)];
            Vector3 pA = _OGpoints[GetCorrectedPoint(i, _OGpoints)];                //Really this is first point
            Vector3 pB = _OGpoints[GetCorrectedPoint(i + 1, _OGpoints)];            //And this is the second point

            if (i > _OGpoints.Count - 1)
            {
                pB = returnMe[0]; 
            }
            //returnMe.Add(pA + ((pAminus1 - pA) * 0.25f));
            returnMe.Add(pA + ((pB - pA) * 0.45f));
        }
        return returnMe.ToArray();
    }

    Vector3[] InterpolatePoints(Vector3[] _input)
    {
        List<Vector3> newPoints = new List<Vector3>();
        List<Vector3> tempList;
        int curvedLength = (_input.Length * smoothness);
        float t = 0;

        for (int i = 0; i < curvedLength + 1; i++)
        {
            t = Mathf.InverseLerp(0, curvedLength, i);

            tempList = new List<Vector3>(_input);

            for (int j = _input.Length - 1; j > 0; j--)
            {
                for (int k = 0; k < j; k++)
                {
                    Vector3 p0 = tempList[GetCorrectedPoint(k - 1, tempList)];
                    Vector3 p1 = tempList[GetCorrectedPoint(k, tempList)];       //Really this is first point
                    Vector3 p2 = tempList[GetCorrectedPoint(k + 1, tempList)];   //And this is the seocnd point
                    Vector3 p3 = tempList[GetCorrectedPoint(k + 2, tempList)];

                    Vector3 vecP0P1 = p1 - p0;
                    Vector3 vecP2P3 = p3 - p2;

                    Vector3 p0a = p1 + (vecP0P1 * .4f);
                    Vector3 p1a = p2 + (vecP2P3 * .4f);

                    tempList[k] = (1 - t) * p1 + t * p2;
                }
            }
            newPoints.Add(tempList[0]);
        }
        return newPoints.ToArray();
    }

    int GetCorrectedPoint(int _index, List<Vector3> _array)
    {
        if (_index < 0)
            return _array.Count + _index;
        if (_index > _array.Count - 1)
            return _index - _array.Count;
        
        return _index;
    }

    int GetCorrectedPoint(int _index, Vector3[] _array)
    {
        if (_index < 0)
            return _array.Length + _index;
        if (_index > _array.Length - 1)
            return _index - _array.Length;

        return _index;
    }

    #endregion
}

[CustomEditor(typeof(TrackMaker))]
public class TrackMakerEditorButtons :Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TrackMaker it = (TrackMaker)target;
        if (GUILayout.Button("Gen track"))
        {
            it.GenerateTrack();
            it.StartCoroutine(it.CreateTrackMesh());
        }
        if (GUILayout.Button("Randomise colours"))
        {
            it.RandomiseColours(Random.value);
        }
    }
}