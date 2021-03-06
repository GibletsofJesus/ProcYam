﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ideas : MonoBehaviour
{
    public static int count;

    public  static bool gen;
    //Let's say we have a line renderer...
    [SerializeField]
    LineRenderer m_lr;
    public bool CamFollow = true;

    public  float[] m_mulutipliers;
    float[] m_oldMulutipliers = new float[4];

    // Use this for initialization
    void Start()
    {
        count++;

        if (!CamFollow)
        {
            m_mulutipliers[0] = Random.Range(0.5f, 2f);
            m_mulutipliers[2] = 2 *
            m_mulutipliers[0];
        }

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

    int camIndex = 0;

    IEnumerator DoALine()
    {
        if (count < 0)
        {
            Ideas copy = Instantiate(this);
            for (int i = 0; i < copy.m_mulutipliers.Length; i++)
            {
                copy.m_mulutipliers[i] *= (Random.Range(.95f, 1.05f));// * (Random.value > .5f ? 1 : -1));
            }
            copy.name = "copy";
        }
        gen = true;
        float timeTraking = 0;
        while (true)
        {
            Vector3[] points = new Vector3[m_lr.positionCount];
            m_lr.GetPositions(points);
            List<Vector3> newPoints = new List<Vector3>(points);
            if (gen)
            {
                timeTraking += Time.deltaTime;
                //For each step, decide what direction to move the line along

                if (CamFollow)
                    newPoints.Add(points[points.Length - 1] +
                        (Vector3.left * Mathf.Sin(timeTraking * m_mulutipliers[0])) +
                        (Vector3.up * Mathf.Cos(timeTraking * m_mulutipliers[1])) +
                        (Vector3.back * Mathf.Cos(timeTraking * m_mulutipliers[2])) +
                        (Vector3.right * Mathf.Sin(timeTraking * m_mulutipliers[3])));
                else
                    newPoints.Add(points[points.Length - 1] +
                        (Vector3.left * Mathf.Sin(timeTraking * m_mulutipliers[0])) +
                        (Vector3.up * timeTraking * m_mulutipliers[1]) +
                        (Vector3.back * Mathf.Cos(timeTraking * m_mulutipliers[2])));
                
                m_lr.positionCount = newPoints.Count;
                m_lr.SetPositions(newPoints.ToArray());
            }
            if (Vector3.Distance(newPoints[newPoints.Count - 1], newPoints[0]) < 3 && newPoints.Count > 200)
                gen = false;
            if (newPoints.Count > 1500)
                gen = false;

            if (name != "copy" && CamFollow)
            {
                if (gen)
                    camIndex = newPoints.Count - 1;
                else
                    camIndex += 1;

                if (camIndex > newPoints.Count - 1)
                    camIndex -= newPoints.Count;

                Camera.main.transform.position = Vector3.Lerp(
                    Camera.main.transform.position, newPoints[camIndex] +
                    (Vector3.back * Mathf.Cos(Time.time) * 5) +
                    (Vector3.left * Mathf.Sin(Time.time) * 5), 
                    Time.deltaTime * 2);

                transform.position = Camera.main.transform.position;
                if (newPoints.Count > 6)
                    transform.LookAt(newPoints[camIndex > 4 ? camIndex - 5 : camIndex - 5 + newPoints.Count]);
                Camera.main.transform.rotation = Quaternion.LerpUnclamped(Camera.main.transform.rotation, transform.rotation, Time.deltaTime * 3);
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
