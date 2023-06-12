using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmo : MonoBehaviour
{
    [SerializeField] Color gizmoColor = Color.white;
    [SerializeField] bool wire = true;

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = gizmoColor;

        if (wire)
            Gizmos.DrawWireSphere(Vector3.zero, 0.3F);
        else
            Gizmos.DrawSphere(Vector3.zero, 0.3F);
    }
}
