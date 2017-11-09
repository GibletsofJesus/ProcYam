using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshExtruder : MonoBehaviour
{
    [Header("Mesh referneces")]
    [SerializeField]
    MeshFilter m_roadMesh;
    [SerializeField]
    MeshFilter m_barrierMeshA, m_barrierMeshB, m_lowerMesh, m_lapTriggerMesh;
    [Header("Renderer referneces")]
    public MeshRenderer m_roadRenderer;
    public MeshRenderer m_barrierRendererA, m_barrierRendererB, m_lowerRenderer;

    [Header("Other referneces")]
    [SerializeField]
    Material finishMaterial;
    public GameObject m_flag;
    public LapTrigger m_lt;
    public MeshExtruder NextSection;
    [HideInInspector]
    public Matrix4x4 m_roadMatrix, m_barrierAMatrix, m_barrierBMatrix, m_lowerMatrix, m_lapTriggerMatrix;
    [HideInInspector]
    public Vector3[] m_roadVertsWS, m_barrierAVertsWS, m_barrierBVertsWS, m_lowerVertsWS, m_lapTriggerVertWS;

    [Header("Settings")]
    public float width = 1;
    public float m_extrusionDistance;

    // Use this for initialization
    void Start()
    {
        SetupMeshes();

        m_roadMatrix = Matrix4x4.TRS(m_roadMesh.transform.position, m_roadMesh.transform.rotation, m_roadMesh.transform.lossyScale);
        m_barrierAMatrix = Matrix4x4.TRS(m_barrierMeshA.transform.position, m_barrierMeshA.transform.rotation, m_barrierMeshA.transform.lossyScale);
        m_barrierBMatrix = Matrix4x4.TRS(m_barrierMeshB.transform.position, m_barrierMeshB.transform.rotation, m_barrierMeshB.transform.lossyScale);
        m_lowerMatrix = Matrix4x4.TRS(m_lowerMesh.transform.position, m_lowerMesh.transform.rotation, m_lowerMesh.transform.lossyScale);
        m_lapTriggerMatrix = Matrix4x4.TRS(m_lapTriggerMesh.transform.position, m_lapTriggerMesh.transform.rotation, m_lapTriggerMesh.transform.lossyScale);

        StretchRoadMesh();

        GetRoadVertsInWorldSpace(m_roadMesh.mesh);
        //Left
        GetBarrierVertsInWorldSpace(m_barrierMeshA);
        //Right
        GetBarrierVertsInWorldSpace(m_barrierMeshB);
       
        m_extrusionDistance = Vector3.Distance(m_roadMesh.mesh.vertices[2], m_roadMatrix.inverse.MultiplyPoint3x4(NextSection.m_roadVertsWS[2]));

        m_roadRenderer.material.mainTextureScale = new Vector2(1, (1 / m_extrusionDistance) * 25);
    }

    void SetupMeshes()
    {
        #region Road
        Mesh new_roadMesh = new Mesh();
        new_roadMesh.vertices = new Vector3[]
        {
            new Vector3(1, 1, 0),
            new Vector3(-1, -1, 0),
            new Vector3(1, 1, 0),
            new Vector3(-1, 1, 0),
        };
        new_roadMesh.triangles = new int[]
        {
            0, 2, 3,
            0, 3, 1
        };
        new_roadMesh.uv = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        #endregion
        #region Barrier
        Mesh new_barrierMesh = new Mesh();
        new_barrierMesh.vertices = new Vector3[]
        {
            new Vector3(0.1f, -1, -.2f),
            new Vector3(0.1f, 1, -.2f),
            new Vector3(-0.1f, -1, -.2f),
            new Vector3(-0.1f, -1, .2f),
            new Vector3(-0.1f, 1, -.2f),
            new Vector3(-0.1f, 1, .2f),
            new Vector3(0.1f, -1, .1f),
            new Vector3(0.1f, 1, .1f),
            new Vector3(0.1f, 1, .2f),
            new Vector3(0.1f, -1, .2f),
            new Vector3(-0.1f, -1, .2f),
            new Vector3(-0.1f, 1, .2f),
            new Vector3(0f, 1, .2f),
            new Vector3(0f, -1, .2f),
            new Vector3(0.1f, 1, .1f),
            new Vector3(0.1f, -1, .1f)
        };
        new_barrierMesh.triangles = new int[]
        {
            8, 5, 3,
            8, 3, 9,
            10, 11, 4,
            10, 4, 2,
            12, 13, 6,
            12, 6, 7,
            14, 15, 0,
            14, 0, 1
        };
        new_barrierMesh.uv = new Vector2[]
        {
            new Vector2(1, 0),
            new Vector2(0, 0),
            new Vector2(1, 1),
            new Vector2(1, .6f),
            new Vector2(0, 1),
            new Vector2(0, .6f),
            new Vector2(1, .3f),
            new Vector2(0, .3f),
            new Vector2(0, .5f),
            new Vector2(1, .5f),
            new Vector2(1, .6f),
            new Vector2(0, .6f),
            new Vector2(0, .5f),
            new Vector2(1, .5f),
            new Vector2(0, .3f),
            new Vector2(1, .3f)
        };        
        #endregion
        #region lower
        Mesh new_lowerMesh = new Mesh();
        new_lowerMesh.vertices = new Vector3[]
        {
            new Vector3(1.2f, -1, -2),
            new Vector3(1.2f, -1, 0),
            new Vector3(1.2f, 1, -2),
            new Vector3(1.2f, 1, 0),
            new Vector3(-1.2f, -1, -2),
            new Vector3(-1.2f, -1, 0),
            new Vector3(-1.2f, 1, -2),
            new Vector3(-1.2f, 1, 0),


            new Vector3(1.2f, -2, -2),
            new Vector3(1.2f, -2, 0),
            new Vector3(1.2f, -1, -2),
            new Vector3(1.2f, -1, 0),
            new Vector3(-1.2f, -2, -2),
            new Vector3(-1.2f, -2, 0),
            new Vector3(-1.2f, -1, -2),
            new Vector3(-1.2f, -1, 0),
        };
        new_lowerMesh.triangles = new int[]
        {
            1, 0, 2,
            1, 2, 3,
            7, 6, 4,
            7, 4, 5,

            9, 8, 10,
            9, 10, 11,
            15, 14, 12,
            15, 12, 13
        };
        #endregion

        m_roadMesh.mesh = new_roadMesh;
        m_barrierMeshA.mesh = new_barrierMesh;
        m_barrierMeshB.mesh = new_barrierMesh;
        m_lowerMesh.mesh = new_lowerMesh;
    }

    public void AddFinishLineTexture()
    {
        m_roadRenderer.sharedMaterial = finishMaterial;
        m_roadRenderer.sharedMaterial.mainTextureScale = new Vector2(0.3f, 0.3f);
        m_roadRenderer.sharedMaterial.mainTextureOffset = new Vector2(0.3f, 0);
    }

    void DoTheLowerBit()
    {
        Vector3[] verts;
        verts = m_lowerMesh.mesh.vertices;
        m_lowerVertsWS = verts;

        #region -y
        {
            #region -z
            //  +x
            verts[0] = m_lowerMatrix.inverse.MultiplyPoint3x4(m_roadVertsWS[2]) + (Vector3.right * 3)
            + (Vector3.back * 10);

            //  -x
            verts[4] = m_lowerMatrix.inverse.MultiplyPoint3x4(m_roadVertsWS[3]) + (Vector3.left * 3)
            + (Vector3.back * 10);
            #endregion
        }

        {
            #region z=0
            //    +x
            verts[1] = m_lowerMatrix.inverse.MultiplyPoint3x4(m_roadVertsWS[0]) + (Vector3.right * 3)
            + (Vector3.back * 10 * width);

            //    -x
            verts[5] = m_lowerMatrix.inverse.MultiplyPoint3x4(m_roadVertsWS[1]) + (Vector3.left * 3)
            + (Vector3.back * 10 * width);
            #endregion
        }
        #endregion

        #region +y

        {
            #region -z
            //    +x
            verts[2] = m_lowerMatrix.inverse.MultiplyPoint3x4(m_roadVertsWS[2]) + (Vector3.right * 0.2f);

            //    -x
            verts[6] = m_lowerMatrix.inverse.MultiplyPoint3x4(m_roadVertsWS[3]) + (Vector3.left * 0.2f);
            #endregion
        }
        {
            #region z=0
            //    +x
            verts[3] = m_lowerMatrix.inverse.MultiplyPoint3x4(m_roadVertsWS[0]) + (Vector3.right * 0.2f);

            //    -x
            verts[7] = m_lowerMatrix.inverse.MultiplyPoint3x4(m_roadVertsWS[1]) + (Vector3.left * 0.2f);
            #endregion
        }
        #endregion

        verts[0].z = m_lowerMatrix.inverse.MultiplyPoint3x4(Vector3.down * 12f).z;
        verts[1].z = m_lowerMatrix.inverse.MultiplyPoint3x4(Vector3.down * 12f).z;
        verts[4].z = m_lowerMatrix.inverse.MultiplyPoint3x4(Vector3.down * 12f).z;
        verts[5].z = m_lowerMatrix.inverse.MultiplyPoint3x4(Vector3.down * 12f).z;

        verts[10] = verts[0];
        verts[11] = verts[1];
        verts[14] = verts[4];
        verts[15] = verts[5];

        verts[8] = verts[0] +
        (Vector3.right * 5f) +
        (Vector3.back * 3f);
        verts[9] = verts[1] +
        (Vector3.right * 5f) +
        (Vector3.back * 3f);
        verts[12] = verts[4] +
        (Vector3.left * 5f) +
        (Vector3.back * 3f);          
        verts[13] = verts[5] +
        (Vector3.left * 5f) +
        (Vector3.back * 3f);
         
        


        m_lowerMesh.mesh.SetVertices(new List<Vector3>(verts));
        m_lowerMesh.mesh.RecalculateNormals();
        m_lowerMesh.mesh.RecalculateBounds();
        m_lowerMesh.mesh.RecalculateTangents();

        for (int i = 0; i < verts.Length; i++)
        {
            m_lowerVertsWS[i] = m_lowerMatrix.MultiplyPoint3x4(verts[i]);
        }
    }

    public void JoinRoadMesh()
    {
        Vector3[] verts;
        verts = m_roadMesh.mesh.vertices;
        verts[0] = m_roadMatrix.inverse.MultiplyPoint3x4(NextSection.m_roadVertsWS[2]);
        verts[1] = m_roadMatrix.inverse.MultiplyPoint3x4(NextSection.m_roadVertsWS[3]);

        m_roadMesh.mesh.SetVertices(new List<Vector3>(verts));
        m_roadMesh.mesh.RecalculateNormals();
        m_roadMesh.mesh.RecalculateBounds();
        m_roadMesh.mesh.RecalculateTangents();
    }

    public void JoinBarrierMesh()
    {
        #region A
        Vector3[] verts;
        verts = m_barrierMeshA.mesh.vertices;

        verts[0] = m_barrierAMatrix.inverse.MultiplyPoint3x4(NextSection.m_barrierAVertsWS[1]);
        verts[2] = m_barrierAMatrix.inverse.MultiplyPoint3x4(NextSection.m_barrierAVertsWS[4]);
        verts[3] = m_barrierAMatrix.inverse.MultiplyPoint3x4(NextSection.m_barrierAVertsWS[5]);
        verts[6] = m_barrierAMatrix.inverse.MultiplyPoint3x4(NextSection.m_barrierAVertsWS[7]);
        verts[9] = m_barrierAMatrix.inverse.MultiplyPoint3x4(NextSection.m_barrierAVertsWS[8]);


        verts[10] = m_barrierAMatrix.inverse.MultiplyPoint3x4(NextSection.m_barrierAVertsWS[11]);
        verts[13] = m_barrierAMatrix.inverse.MultiplyPoint3x4(NextSection.m_barrierAVertsWS[12]);
        verts[15] = m_barrierAMatrix.inverse.MultiplyPoint3x4(NextSection.m_barrierAVertsWS[14]);

        //verts[indexB] = m_barrierAMatrix.inverse.MultiplyPoint3x4(NextSection.m_barrierAVertsWS[indexA]);

        m_barrierMeshA.mesh.SetVertices(new List<Vector3>(verts));
        m_barrierMeshA.mesh.RecalculateNormals();
        m_barrierMeshA.mesh.RecalculateBounds();
        m_barrierMeshA.mesh.RecalculateTangents();
        #endregion
        #region B
        verts = m_barrierMeshB.mesh.vertices;

        verts[0] = m_barrierBMatrix.inverse.MultiplyPoint3x4(NextSection.m_barrierBVertsWS[1]);
        verts[2] = m_barrierBMatrix.inverse.MultiplyPoint3x4(NextSection.m_barrierBVertsWS[4]);
        verts[3] = m_barrierBMatrix.inverse.MultiplyPoint3x4(NextSection.m_barrierBVertsWS[5]);
        verts[6] = m_barrierBMatrix.inverse.MultiplyPoint3x4(NextSection.m_barrierBVertsWS[7]);
        verts[9] = m_barrierBMatrix.inverse.MultiplyPoint3x4(NextSection.m_barrierBVertsWS[8]);


        verts[10] = m_barrierBMatrix.inverse.MultiplyPoint3x4(NextSection.m_barrierBVertsWS[11]);
        verts[13] = m_barrierBMatrix.inverse.MultiplyPoint3x4(NextSection.m_barrierBVertsWS[12]);
        verts[15] = m_barrierBMatrix.inverse.MultiplyPoint3x4(NextSection.m_barrierBVertsWS[14]);

        //verts[indexB] = m_barrierAMatrix.inverse.MultiplyPoint3x4(NextSection.m_barrierAVertsWS[indexA]);

        m_barrierMeshB.mesh.SetVertices(new List<Vector3>(verts));
        m_barrierMeshB.mesh.RecalculateNormals();
        m_barrierMeshB.mesh.RecalculateBounds();
        m_barrierMeshB.mesh.RecalculateTangents();
        #endregion
    }

    void JoinLowerMesh()
    {
        Vector3[] verts;
        verts = m_lowerMesh.mesh.vertices;

        verts[1] = m_lowerMatrix.inverse.MultiplyPoint3x4(NextSection.m_lowerVertsWS[0]);
        verts[3] = m_lowerMatrix.inverse.MultiplyPoint3x4(NextSection.m_lowerVertsWS[2]);
        verts[5] = m_lowerMatrix.inverse.MultiplyPoint3x4(NextSection.m_lowerVertsWS[4]);
        verts[7] = m_lowerMatrix.inverse.MultiplyPoint3x4(NextSection.m_lowerVertsWS[6]);

        verts[9] = m_lowerMatrix.inverse.MultiplyPoint3x4(NextSection.m_lowerVertsWS[8]);
        verts[11] = m_lowerMatrix.inverse.MultiplyPoint3x4(NextSection.m_lowerVertsWS[10]);
        verts[13] = m_lowerMatrix.inverse.MultiplyPoint3x4(NextSection.m_lowerVertsWS[12]);
        verts[15] = m_lowerMatrix.inverse.MultiplyPoint3x4(NextSection.m_lowerVertsWS[14]);

        m_lowerMesh.mesh.SetVertices(new List<Vector3>(verts));
        m_lowerMesh.mesh.RecalculateNormals();
        m_lowerMesh.mesh.RecalculateBounds();
        m_lowerMesh.mesh.RecalculateTangents();
    }

    bool joined, lowerJoin = false;

    void Update()
    {
        if (!joined && NextSection)
        {
            if (NextSection.m_roadVertsWS[3] != Vector3.zero)
            {
                JoinRoadMesh();
                JoinBarrierMesh();
                GetRoadVertsInWorldSpace(m_roadMesh.mesh);
                DeformLapTrigger();
                DoTheLowerBit();

                m_roadMesh.gameObject.AddComponent<MeshCollider>();
                m_barrierMeshA.gameObject.AddComponent<MeshCollider>();
                m_barrierMeshB.gameObject.AddComponent<MeshCollider>();

                joined = true;
            }
        }
        if (!lowerJoin && NextSection)
        {
            if (NextSection.m_lowerVertsWS.Length > 5)
            {
                if (NextSection.m_lowerVertsWS[6] != Vector3.zero && joined)
                {
                    JoinLowerMesh();
                    lowerJoin = true;
                }
            }
        }
    }

    void DeformLapTrigger()
    {
        Vector3[] verts = m_lapTriggerMesh.mesh.vertices;

        int[,] simpleVerts = new int[8, 3];

        //Shit and fuck
        #region Vertex index values
        simpleVerts[0, 0] = 0;
        simpleVerts[0, 1] = 13;
        simpleVerts[0, 2] = 23;

        simpleVerts[1, 0] = 1;
        simpleVerts[1, 1] = 14;
        simpleVerts[1, 2] = 16;


        simpleVerts[2, 0] = 2;
        simpleVerts[2, 1] = 8;
        simpleVerts[2, 2] = 22;

        simpleVerts[3, 0] = 3;
        simpleVerts[3, 1] = 9;
        simpleVerts[3, 2] = 17;

        simpleVerts[4, 0] = 4;
        simpleVerts[4, 1] = 10;
        simpleVerts[4, 2] = 21;

        simpleVerts[5, 0] = 5;
        simpleVerts[5, 1] = 11;
        simpleVerts[5, 2] = 18;

        simpleVerts[6, 0] = 6;
        simpleVerts[6, 1] = 12;
        simpleVerts[6, 2] = 20;

        simpleVerts[7, 0] = 7;
        simpleVerts[7, 1] = 15;
        simpleVerts[7, 2] = 19;
        #endregion

        for (int i = 0; i < simpleVerts.GetLength(0); i++)
        {
            for (int j = 0; j < simpleVerts.GetLength(1); j++)
            {
                verts[simpleVerts[i, j]] = Vector3.zero;
                if (i < 5)
                {
                    //+z
                    if (i % 2 == 0)
                    {
                        //+x
                        verts[simpleVerts[i, j]] = m_lapTriggerMatrix.inverse.MultiplyPoint3x4(m_roadVertsWS[0]);
                        //0
                    }
                    else
                    {
                        //-x
                        verts[simpleVerts[i, j]] = m_lapTriggerMatrix.inverse.MultiplyPoint3x4(m_roadVertsWS[2]);
                        //2
                    }
                }
                else
                {
                    //-z;
                    if (i % 2 == 0)
                    {
                        //+x
                        verts[simpleVerts[i, j]] = m_lapTriggerMatrix.inverse.MultiplyPoint3x4(m_roadVertsWS[1]);
                        //1
                    }
                    else
                    {
                        //-x
                        verts[simpleVerts[i, j]] = m_lapTriggerMatrix.inverse.MultiplyPoint3x4(m_roadVertsWS[3]);
                        //3
                    }
                }

                //Don't forget about these cheeky boys
                if (i > 1 && i < 6)
                {
                    verts[simpleVerts[i, j]] += Vector3.forward * 5;
                }
            }
        }

        m_lapTriggerMesh.mesh.SetVertices(new List<Vector3>(verts));
        m_lapTriggerMesh.mesh.RecalculateNormals();
        m_lapTriggerMesh.mesh.RecalculateBounds();
        m_lapTriggerMesh.mesh.RecalculateTangents();
        MeshCollider mCol = m_lapTriggerMesh.gameObject.AddComponent<MeshCollider>();
        mCol.convex = true;
        mCol.isTrigger = true;
    }

    #region Extrusion

    void GetRoadVertsInWorldSpace(Mesh _mesh)
    {
        Vector3[] verts;
        verts = _mesh.vertices;
        for (int i = 0; i < verts.Length; i++)
        {
            m_roadVertsWS[i] = m_roadMatrix.MultiplyPoint3x4(verts[i]);
        }
    }

    void StretchRoadMesh()
    {
        Vector3[] verts;
        verts = m_roadMesh.mesh.vertices;
        for (int i = 0; i < verts.Length; i++)
        {
            if (width > 1)
                verts[i].x = width * (i % 2 == 0 ? 1f : -1f);
            m_roadVertsWS[i] = m_roadMatrix.MultiplyPoint3x4(verts[i]);
        }
        m_roadMesh.mesh.SetVertices(new List<Vector3>(verts));
        m_roadMesh.mesh.RecalculateNormals();
        m_roadMesh.mesh.RecalculateBounds();
        m_roadMesh.mesh.RecalculateTangents();
    }

    void GetBarrierVertsInWorldSpace(MeshFilter _mesh)
    {
        Vector3[] verts;
        verts = _mesh.mesh.vertices;

        for (int i = 0; i < verts.Length; i++)
        {
            if (_mesh.transform.localScale.x > 0)
            {
                if (width > 1)
                    verts[i].x -= width - 1;
                //A
                m_barrierAVertsWS[i] = m_barrierAMatrix.MultiplyPoint3x4(verts[i]);
            }
            else
            {
                if (width > 1)
                    verts[i].x -= width - 1;
                //B
                m_barrierBVertsWS[i] = m_barrierBMatrix.MultiplyPoint3x4(verts[i]);
            }
        }
        _mesh.mesh.SetVertices(new List<Vector3>(verts));
        _mesh.mesh.RecalculateNormals();
        _mesh.mesh.RecalculateBounds();
        _mesh.mesh.RecalculateTangents();
    }

    #endregion
}
