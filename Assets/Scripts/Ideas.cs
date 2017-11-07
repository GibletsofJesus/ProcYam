using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ideas : MonoBehaviour
{
    public static int count;

    //Let's say we have a line renderer...
    [SerializeField]
    LineRenderer m_lr;

    public  float[] m_mulutipliers;
    float[] m_oldMulutipliers = new float[4];

    // Use this for initialization
    void Start()
    {
        count++;
        StartCoroutine(DoALine());
        for (int i = 0; i < m_mulutipliers.Length; i++)
        {
            m_oldMulutipliers[i] = m_mulutipliers[i];
        }
    }

    void Updlate()
    {
        for (int i = 0; i < m_mulutipliers.Length; i++)
        {
            if (m_mulutipliers[i] != m_oldMulutipliers[i])
            {
                m_oldMulutipliers[i] = m_mulutipliers[i];
                DoALineQuickly();
            }
        }
    }

    void DoALineQuickly()
    {
        m_lr.positionCount = 1;
        bool gen = true;
        float timeTraking = 0;
        while (gen)
        {
            timeTraking += 0.05f;
            //For each step, decide what direction to move the line along
            Vector3[] points = new Vector3[m_lr.positionCount];
            m_lr.GetPositions(points);
            List<Vector3> newPoints = new List<Vector3>(points);

            newPoints.Add(points[points.Length - 1] +
                (Vector3.left * Mathf.Sin(timeTraking * m_mulutipliers[0])) +
                (Vector3.up * Mathf.Cos(timeTraking * m_mulutipliers[1])) +
                (Vector3.back * Mathf.Cos(timeTraking * m_mulutipliers[2])) +
                (Vector3.right * Mathf.Sin(timeTraking * m_mulutipliers[3])));

            if (Vector3.Distance(newPoints[newPoints.Count - 1], newPoints[0]) < 5 && newPoints.Count > 20)
                gen = false;
            if (newPoints.Count > 1000)
                gen = false;

            m_lr.positionCount = newPoints.Count;
            m_lr.SetPositions(newPoints.ToArray());

        }
    }

    IEnumerator DoALine()
    {
        if (count < 10)
        {
            Ideas copy = Instantiate(this);
            for (int i = 0; i < copy.m_mulutipliers.Length; i++)
            {
                copy.m_mulutipliers[i] *= (Random.Range(.95f, 1.05f));// * (Random.value > .5f ? 1 : -1));
            }
            copy.name = "copy";
        }

        bool gen = true;
        float timeTraking = 0;
        while (gen)
        {
            timeTraking += Time.deltaTime * 0.5f;
            //For each step, decide what direction to move the line along
            Vector3[] points = new Vector3[m_lr.positionCount];
            m_lr.GetPositions(points);
            List<Vector3> newPoints = new List<Vector3>(points);

            newPoints.Add(points[points.Length - 1] +
                (Vector3.left * Mathf.Sin(timeTraking * m_mulutipliers[0])) +
                (Vector3.up * Mathf.Cos(timeTraking * m_mulutipliers[1])) +
                (Vector3.back * Mathf.Cos(timeTraking * m_mulutipliers[2])) +
                (Vector3.right * Mathf.Sin(timeTraking * m_mulutipliers[3])));

            if (Vector3.Distance(newPoints[newPoints.Count - 1], newPoints[0]) < 1 && newPoints.Count > 200)
                gen = false;
            /*if (newPoints.Count > 2500)
                gen = false;*/

            if (name != "copy")
            {
                Camera.main.transform.position = Vector3.Lerp(
                    Camera.main.transform.position, newPoints[newPoints.Count - 1] +
                    (Vector3.back * Mathf.Cos(Time.time) * 5) +
                    (Vector3.left * Mathf.Sin(Time.time) * 5), 
                    Time.deltaTime * 5);

                //Camera.main.transform.position = newPoints[newPoints.Count - 1];
                transform.position = Camera.main.transform.position;
                if (newPoints.Count > 6)
                    transform.LookAt(newPoints[newPoints.Count - 5]);
                Camera.main.transform.rotation = Quaternion.LerpUnclamped(Camera.main.transform.rotation, transform.rotation, Time.deltaTime * 3);
            }
            m_lr.positionCount = newPoints.Count;
            m_lr.SetPositions(newPoints.ToArray());

            yield return new WaitForEndOfFrame();
        }
    }
}
