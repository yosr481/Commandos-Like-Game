﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class LunchArcMesh : MonoBehaviour {

    Mesh mesh;
    [Range(0.1f,1)]
    public float meshWidth;
    [Range(7, 20)]
    public float velocity;
    [Range(20, 85)]
    public float angle;
    [Range(2, 100)]
    public int resolution = 10;

    float projectile_Velocity;
    float g;
    float radianAngle;

    ThrowSimulation throwSimulation;

    private void Awake()
    {
        throwSimulation = GetComponent<ThrowSimulation>();
        mesh = GetComponent<MeshFilter>().mesh;
        g = Mathf.Abs(Physics2D.gravity.y);
    }

    /*private void OnValidate()
    {
        if (mesh != null && Application.isPlaying)
            MakeArcMesh(CalculateArcArray());
    }*/

    // Use this for initialization
    void FixedUpdate () {
        MakeArcMesh(CalculateArcArray());
	}
	
	void MakeArcMesh(Vector3[] arcVerts)
    {
        mesh.Clear();

        Vector3[] vertices = new Vector3[(resolution + 1) * 2];
        int[] triangles = new int[resolution * 6 * 2];

        for (int i = 0; i <= resolution; i++)
        {
            vertices[i * 2] = new Vector3(meshWidth * 0.5f, arcVerts[i].y, arcVerts[i].x);
            vertices[i * 2 + 1] = new Vector3(meshWidth * -0.5f, arcVerts[i].y, arcVerts[i].x);

            if (i != resolution)
            {
                triangles[i * 12] = i * 2;
                triangles[i * 12 + 1] = triangles[i * 12 + 4] = i * 2 + 1; 
                triangles[i * 12 + 2] = triangles[i * 12 + 3] = (i + 1) * 2;
                triangles[i * 12 + 5] = (i + 1) * 2 + 1;

                triangles[i * 12 + 6] = i * 2;
                triangles[i * 12 + 7] = triangles[i * 12 + 10] = (i + 1) * 2;
                triangles[i * 12 + 8] = triangles[i * 12 + 9] = i * 2 + 1;
                triangles[i * 12 + 11] = (i + 1) * 2 + 1;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    Vector3[] CalculateArcArray()
    {
        Vector3[] arcArray = new Vector3[resolution + 1];

        float targetDistance = Vector3.Distance(transform.position, throwSimulation.Target.position);
        projectile_Velocity = targetDistance / (Mathf.Sin(2 * angle * Mathf.Deg2Rad) / g);
        radianAngle = Mathf.Deg2Rad * angle;
        //float maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / g;

        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / (float)resolution;
            arcArray[i] = CalculateArcPoint(t, targetDistance);
        }

        return arcArray;
    }

    Vector3 CalculateArcPoint(float t, float maxDistance)
    {
        float x = t * maxDistance;
        float y = x * Mathf.Tan(radianAngle) - ((g * x * x) / (2 * projectile_Velocity * projectile_Velocity * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle)));
        return new Vector3(x, y);
    }
}
