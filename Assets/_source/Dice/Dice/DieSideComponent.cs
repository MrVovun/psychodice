using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieSideComponent : MonoBehaviour
{
    [SerializeField] int _sideValue;
    public int SideValue => _sideValue;

    private void OnDrawGizmos()
    {
        Vector3 p = transform.position;
        Vector3 f = transform.forward * 0.05f;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(p, p + f);
    }
}
