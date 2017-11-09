using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCamera : MonoBehaviour
{

    [SerializeField]
    LineRenderer m_lr;
    [SerializeField]
    float m_moveSpeed;
    float lerpo;
    [SerializeField]
    int hi;
    // Update is called once per frame
    void Update()
    {
        if (m_lr.positionCount > 10)
        {
            lerpo += Time.deltaTime * m_moveSpeed;

            if (lerpo > m_lr.positionCount - 1)
                lerpo -= m_lr.positionCount;

            hi = (int)(lerpo - (lerpo % 1));

            Vector3 start = m_lr.GetPosition(CorrectPosition(hi - 1));

            Vector3 position = Vector3.Lerp(start, m_lr.GetPosition(hi), lerpo % 1) + (Vector3.up * 1.5f) +
                              // (Vector3.back * Mathf.Cos(Time.time) * 5) +
                               (Vector3.left * Mathf.Sin(Time.time) * 5);
            Vector3 lookAtMe = Vector3.Lerp(
                                   m_lr.GetPosition(CorrectPosition(hi)) + (Vector3.up * 1f), 
                                   m_lr.GetPosition(CorrectPosition(hi + 1)) + (Vector3.up * 1f),
                                   lerpo % 1) +
                               (Vector3.up * 1);
            // (Vector3.left * Mathf.Sin(Time.time) * 2); 
            lookAtMe *= m_lr.transform.lossyScale.x;

            transform.position = position * m_lr.transform.lossyScale.x;
            transform.LookAt(lookAtMe);
        }
    }

    int CorrectPosition(int _index)
    {
        if (_index > m_lr.positionCount - 1)
            return _index - m_lr.positionCount;
        if (_index < 0)
            return _index + m_lr.positionCount;

        return _index;
    }
}
