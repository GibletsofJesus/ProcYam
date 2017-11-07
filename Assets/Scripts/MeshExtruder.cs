using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshExtruder : MonoBehaviour
{
    [SerializeField]
    MeshFilter m_roadMesh, m_barrierMeshA, m_barrierMeshB, m_cubeMesh;
    public GameObject m_flag;
    [SerializeField]
    MeshRenderer m_roadRenderer;
    [SerializeField]
    Material finishMaterial;

    public MeshExtruder NextSection;
    public float m_extrusionDistance;
    [HideInInspector]
    public Matrix4x4 m_roadMatrix, m_barrierAMatrix, m_barrierBMatrix, m_cubeMatrix;
    [HideInInspector]
    public Vector3[] m_roadVertsWS, m_barrierAVertsWS, m_barrierBVertsWS, m_cubeVertsWS;

    // Use this for initialization
    void Start()
    {
        m_roadMatrix = Matrix4x4.TRS(m_roadMesh.transform.position, m_roadMesh.transform.rotation, m_roadMesh.transform.lossyScale);
        m_barrierAMatrix = Matrix4x4.TRS(m_barrierMeshA.transform.position, m_barrierMeshA.transform.rotation, m_barrierMeshA.transform.lossyScale);
        m_barrierBMatrix = Matrix4x4.TRS(m_barrierMeshB.transform.position, m_barrierMeshB.transform.rotation, m_barrierMeshB.transform.lossyScale);
        m_cubeMatrix = Matrix4x4.TRS(m_cubeMesh.transform.position, m_cubeMesh.transform.rotation, m_cubeMesh.transform.lossyScale);

        GetRoadVertsInWorldSpace(m_roadMesh.mesh);
        //Left
        GetBarrierVertsInWorldSpace(m_barrierMeshA);
        //Right
        GetBarrierVertsInWorldSpace(m_barrierMeshB);
       
        m_extrusionDistance = Vector3.Distance(m_roadMesh.mesh.vertices[2], m_roadMatrix.inverse.MultiplyPoint3x4(NextSection.m_roadVertsWS[2]));

        m_roadRenderer.material.mainTextureScale = new Vector2(1, (1 / m_extrusionDistance) * 25);
    }

    public void hotFix()
    {
        m_roadRenderer.material = finishMaterial;
        m_roadRenderer.material.mainTextureScale = new Vector2(0.3f, 0.3f);
        m_roadRenderer.material.mainTextureOffset = new Vector2(0.3f, 0);
    }

    void DoTheLowerBit()
    {
        Vector3[] verts;
        verts = m_cubeMesh.mesh.vertices;
        m_cubeVertsWS = verts;

        #region -y
        {
            #region -z
            //  +x
            verts[0] = m_cubeMatrix.inverse.MultiplyPoint3x4(m_roadVertsWS[2]) + (Vector3.right * 3)
            + (Vector3.back * 10);

            //  -x
            verts[4] = m_cubeMatrix.inverse.MultiplyPoint3x4(m_roadVertsWS[3]) + (Vector3.left * 3)
            + (Vector3.back * 10);
            #endregion
        }

        {
            #region z=0
            //    +x
            verts[1] = m_cubeMatrix.inverse.MultiplyPoint3x4(m_roadVertsWS[0]) + (Vector3.right * 3)
            + (Vector3.back * 10);

            //    -x
            verts[5] = m_cubeMatrix.inverse.MultiplyPoint3x4(m_roadVertsWS[1]) + (Vector3.left * 3)
            + (Vector3.back * 10);
            #endregion
        }
        #endregion

        #region +y

        {
            #region -z
            //    +x
            verts[2] = m_cubeMatrix.inverse.MultiplyPoint3x4(m_roadVertsWS[2]) + (Vector3.right * 0.2f);

            //    -x
            verts[6] = m_cubeMatrix.inverse.MultiplyPoint3x4(m_roadVertsWS[3]) + (Vector3.left * 0.2f);
            #endregion
        }
        {
            #region z=0
            //    +x
            verts[3] = m_cubeMatrix.inverse.MultiplyPoint3x4(m_roadVertsWS[0]) + (Vector3.right * 0.2f);

            //    -x
            verts[7] = m_cubeMatrix.inverse.MultiplyPoint3x4(m_roadVertsWS[1]) + (Vector3.left * 0.2f);
            #endregion
        }
        #endregion

        verts[0].z = m_cubeMatrix.inverse.MultiplyPoint3x4(Vector3.down * 15f).z;
        verts[1].z = m_cubeMatrix.inverse.MultiplyPoint3x4(Vector3.down * 15f).z;
        verts[4].z = m_cubeMatrix.inverse.MultiplyPoint3x4(Vector3.down * 15f).z;
        verts[5].z = m_cubeMatrix.inverse.MultiplyPoint3x4(Vector3.down * 15f).z;

        m_cubeMesh.mesh.SetVertices(new List<Vector3>(verts));
        m_cubeMesh.mesh.RecalculateNormals();
        m_cubeMesh.mesh.RecalculateBounds();
        m_cubeMesh.mesh.RecalculateTangents();

        for (int i = 0; i < verts.Length; i++)
        {
            m_cubeVertsWS[i] = m_cubeMatrix.MultiplyPoint3x4(verts[i]);
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
        verts = m_cubeMesh.mesh.vertices;

        verts[1] = m_cubeMatrix.inverse.MultiplyPoint3x4(NextSection.m_cubeVertsWS[0]);
        verts[3] = m_cubeMatrix.inverse.MultiplyPoint3x4(NextSection.m_cubeVertsWS[2]);
        verts[5] = m_cubeMatrix.inverse.MultiplyPoint3x4(NextSection.m_cubeVertsWS[4]);
        verts[7] = m_cubeMatrix.inverse.MultiplyPoint3x4(NextSection.m_cubeVertsWS[6]);

        m_cubeMesh.mesh.SetVertices(new List<Vector3>(verts));
        m_cubeMesh.mesh.RecalculateNormals();
        m_cubeMesh.mesh.RecalculateBounds();
        m_cubeMesh.mesh.RecalculateTangents();
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
                DoTheLowerBit();

                m_roadMesh.gameObject.AddComponent<MeshCollider>();
                m_barrierMeshA.gameObject.AddComponent<MeshCollider>();
                m_barrierMeshB.gameObject.AddComponent<MeshCollider>();

                joined = true;
            }
        }
        if (!lowerJoin && NextSection)
        {
            if (NextSection.m_cubeVertsWS.Length > 5)
            {
                if (NextSection.m_cubeVertsWS[6] != Vector3.zero && joined)
                {
                    JoinLowerMesh();
                    lowerJoin = true;
                }
            }
        }
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
        _mesh.SetVertices(new List<Vector3>(verts));
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        _mesh.RecalculateTangents();
    }

    void GetBarrierVertsInWorldSpace(MeshFilter _mesh)
    {
        Vector3[] verts;
        verts = _mesh.mesh.vertices;

        for (int i = 0; i < verts.Length; i++)
        {
            if (_mesh.transform.localScale.x > 0)
            {
                //A
                m_barrierAVertsWS[i] = m_barrierAMatrix.MultiplyPoint3x4(verts[i]);
            }
            else
            {
                //B
                m_barrierBVertsWS[i] = m_barrierBMatrix.MultiplyPoint3x4(verts[i]);
            }
        }
    }

    void RotateRoadMesh(Mesh _mesh, Vector3 _pivot, float _angle)
    {
        Vector3[] verts;
        verts = _mesh.vertices;
        for (int i = 0; i < verts.Length; i++)
        {
            if (verts[i].y == -1)
            {
                verts[i] = Quaternion.AngleAxis(-_angle, Vector3.forward) * verts[i];
                //do a thing
            }
        }
        _mesh.SetVertices(new List<Vector3>(verts));
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        _mesh.RecalculateTangents();
    }

    #endregion
}
