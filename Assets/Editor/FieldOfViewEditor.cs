using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor {

	void OnSceneGUI()
    {
        FieldOfView fow = target as FieldOfView;

        // Draw radius
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);

        // Draw angle
        Vector3 viewAngleA = fow.dirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.dirFromAngle(fow.viewAngle / 2, false);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

        // Draw line towards units
        Handles.color = Color.red;
        foreach(Transform visibleUnit in fow.visibleUnits)
        {
            Handles.DrawLine(fow.transform.position, visibleUnit.position);
        }
    }
}
