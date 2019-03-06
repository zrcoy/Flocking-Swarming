using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Catmul : MonoBehaviour
{

    //Use the transforms of GameObjects in 3d space as your points or define array with desired points
    public Transform[] pointsSets1;
    public Transform[] pointsSets2;
    public Transform[] pointsSets3;

    //Store points on the Catmull curve so we can visualize them
    public List<Vector3> newPoints1 = new List<Vector3>();
    public List<Vector3> leftPoints1 = new List<Vector3>();
    public List<Vector3> rightPoints1 = new List<Vector3>();

    //Store points on the Catmull curve so we can visualize them
    public List<Vector3> newPoints2 = new List<Vector3>();
    public List<Vector3> leftPoints2 = new List<Vector3>();
    public List<Vector3> rightPoints2 = new List<Vector3>();

    //Store points on the Catmull curve so we can visualize them
    public List<Vector3> newPoints3 = new List<Vector3>();
    public List<Vector3> leftPoints3 = new List<Vector3>();
    public List<Vector3> rightPoints3 = new List<Vector3>();

    //How many points you want on the curve
    public float amountOfPoints = 20.0f;

    //set from 0-1
    public float alpha = 0.5f;

    public int TrackPoints = 10;

    public float trackSize = 20.0f;
    public float gapsize = 1.0f;

    public GameObject Green;
    public GameObject Red;
    public GameObject Blue;

    /////////////////////////////
    private void Awake()
    {
        //Create 3 sets of 10 random waypoints
        GeneratePlanerWaypoints();

        newPoints1.Clear();
        leftPoints1.Clear();
        rightPoints1.Clear();

        newPoints2.Clear();
        leftPoints2.Clear();
        rightPoints2.Clear();

        newPoints3.Clear();
        leftPoints3.Clear();
        rightPoints3.Clear();

        for (int i = 0; i < pointsSets1.Length; i++)
        {
            CatmulRom(i);
        }
        GenerateParallelPaths();
    }

    private void GenerateParallelPaths()
    {
        for (int i = 0; i < newPoints1.Count;i++)
        {
            Vector3 p0 = newPoints1[(i - 1 + newPoints1.Count) % newPoints1.Count];
            Vector3 p1 = newPoints1[(i + 1)%newPoints1.Count];
            Vector3 perp = TrackCreation(gapsize, p0, p1);
            leftPoints1.Add(newPoints1[i] - perp);
            rightPoints1.Add(newPoints1[i] + perp);
        }

        for (int i = 0; i < newPoints2.Count; i++)
        {
            Vector3 p0 = newPoints2[(i - 1 + newPoints2.Count) % newPoints2.Count];
            Vector3 p1 = newPoints2[(i + 1) % newPoints2.Count];
            Vector3 perp = TrackCreation(gapsize, p0, p1);
            leftPoints2.Add(newPoints2[i] - perp);
            rightPoints2.Add(newPoints2[i] + perp);
        }

        for (int i = 0; i < newPoints3.Count; i++)
        {
            Vector3 p0 = newPoints3[(i - 1 + newPoints3.Count) % newPoints3.Count];
            Vector3 p1 = newPoints3[(i + 1) % newPoints3.Count];
            Vector3 perp = TrackCreation(gapsize, p0, p1);
            leftPoints3.Add(newPoints3[i] - perp);
            rightPoints3.Add(newPoints3[i] + perp);
        }
    }

    private void GeneratePlanerWaypoints()
    {
        // Fill pointsSets1 with gameobjects's random transform(actually just random pos)
        pointsSets1 = new Transform[TrackPoints];
        for (int i = 0; i < TrackPoints; i++)
        {
            // spawn points within trackSize radian
            Vector3 pos = UnityEngine.Random.insideUnitSphere * trackSize;
            pos.y = 0.0f;
            // Instantiate Red Object in pos
            GameObject wp = Instantiate(Red, pos, Quaternion.identity);
            // Add this wp's transform to points array
            pointsSets1[i] = wp.transform;
        }

        // Fill pointsSets2 with gameobjects's random transform(actually just random pos)
        pointsSets2 = new Transform[TrackPoints];
        for (int i = 0; i < TrackPoints; i++)
        {
            // spawn points within trackSize radian
            Vector3 pos = UnityEngine.Random.insideUnitSphere * trackSize + Vector3.right * (-50);
            pos.y = 0.0f;
            // Instantiate Blue Object in pos
            GameObject wp = Instantiate(Blue, pos, Quaternion.identity);
            // Add this wp's transform to points array
            pointsSets2[i] = wp.transform;
        }

        // Fill pointsSets3 with gameobjects's random transform(actually just random pos)
        pointsSets3 = new Transform[TrackPoints];
        for (int i = 0; i < TrackPoints; i++)
        {
            // spawn points within trackSize radian
            Vector3 pos = UnityEngine.Random.insideUnitSphere * trackSize + Vector3.right * 50;
            pos.y = 0.0f;
            // Instantiate Green Object in pos
            GameObject wp = Instantiate(Green, pos, Quaternion.identity);
            // Add this wp's transform to points array
            pointsSets3[i] = wp.transform;
        }
    }

    void Update()
    {
        newPoints1.Clear();
        leftPoints1.Clear();
        rightPoints1.Clear();

        newPoints2.Clear();
        leftPoints2.Clear();
        rightPoints2.Clear();

        newPoints3.Clear();
        leftPoints3.Clear();
        rightPoints3.Clear();

        for (int i = 0; i < pointsSets1.Length; i++)
        {
            CatmulRom(i);
        }
        GenerateParallelPaths();
    }
    int getIndex(int index)
    {
        return index % pointsSets1.Length;
    }

    void CatmulRom(int index)
    {
        //For pointsSets1
        {
            //current index of point's pos
            Vector3 p0 = pointsSets1[getIndex(index + 0)].position; // Vector3 has an implicit conversion to Vector2
                                                                    // next index of point's pos
            Vector3 p1 = pointsSets1[getIndex(index + 1)].position;
            Vector3 p2 = pointsSets1[getIndex(index + 2)].position;
            Vector3 p3 = pointsSets1[getIndex(index + 3)].position;

            float t0 = 0.0f;
            float t1 = GetT(t0, p0, p1);// which is t0 + sqrt of distance between p0 and p1
            float t2 = GetT(t1, p1, p2);
            float t3 = GetT(t2, p2, p3);


            for (float t = t1; t < t2; t += ((t2 - t1) / amountOfPoints))
            {
                Vector3 A1 = (t1 - t) / (t1 - t0) * p0 + (t - t0) / (t1 - t0) * p1;
                Vector3 A2 = (t2 - t) / (t2 - t1) * p1 + (t - t1) / (t2 - t1) * p2;
                Vector3 A3 = (t3 - t) / (t3 - t2) * p2 + (t - t2) / (t3 - t2) * p3;

                Vector3 B1 = (t2 - t) / (t2 - t0) * A1 + (t - t0) / (t2 - t0) * A2;
                Vector3 B2 = (t3 - t) / (t3 - t1) * A2 + (t - t1) / (t3 - t1) * A3;

                Vector3 C = (t2 - t) / (t2 - t1) * B1 + (t - t1) / (t2 - t1) * B2;

                if (newPoints1.Count == 0 || newPoints1[newPoints1.Count - 1] != C)
                {
                    newPoints1.Add(C);
                }
            }
        }



        //For pointsSets2
        {
            //current index of point's pos
            Vector3 p0 = pointsSets2[getIndex(index + 0)].position; // Vector3 has an implicit conversion to Vector2
                                                                   // next index of point's pos
            Vector3 p1 = pointsSets2[getIndex(index + 1)].position;
            Vector3 p2 = pointsSets2[getIndex(index + 2)].position;
            Vector3 p3 = pointsSets2[getIndex(index + 3)].position;

            float t0 = 0.0f;
            float t1 = GetT(t0, p0, p1);// which is t0 + sqrt of distance between p0 and p1
            float t2 = GetT(t1, p1, p2);
            float t3 = GetT(t2, p2, p3);


            for (float t = t1; t < t2; t += ((t2 - t1) / amountOfPoints))
            {
                Vector3 A1 = (t1 - t) / (t1 - t0) * p0 + (t - t0) / (t1 - t0) * p1;
                Vector3 A2 = (t2 - t) / (t2 - t1) * p1 + (t - t1) / (t2 - t1) * p2;
                Vector3 A3 = (t3 - t) / (t3 - t2) * p2 + (t - t2) / (t3 - t2) * p3;

                Vector3 B1 = (t2 - t) / (t2 - t0) * A1 + (t - t0) / (t2 - t0) * A2;
                Vector3 B2 = (t3 - t) / (t3 - t1) * A2 + (t - t1) / (t3 - t1) * A3;

                Vector3 C = (t2 - t) / (t2 - t1) * B1 + (t - t1) / (t2 - t1) * B2;

                if (newPoints2.Count == 0 || newPoints2[newPoints2.Count - 1] != C)
                {
                    newPoints2.Add(C);
                }
            }
        }





        //For pointsSets3
        {
            //current index of point's pos
            Vector3 p0 = pointsSets3[getIndex(index + 0)].position; // Vector3 has an implicit conversion to Vector2
            // next index of point's pos
            Vector3 p1 = pointsSets3[getIndex(index + 1)].position;
            Vector3 p2 = pointsSets3[getIndex(index + 2)].position;
            Vector3 p3 = pointsSets3[getIndex(index + 3)].position;

            float t0 = 0.0f;
            float t1 = GetT(t0, p0, p1);// which is t0 + sqrt of distance between p0 and p1
            float t2 = GetT(t1, p1, p2);
            float t3 = GetT(t2, p2, p3);


            for (float t = t1; t < t2; t += ((t2 - t1) / amountOfPoints))
            {
                Vector3 A1 = (t1 - t) / (t1 - t0) * p0 + (t - t0) / (t1 - t0) * p1;
                Vector3 A2 = (t2 - t) / (t2 - t1) * p1 + (t - t1) / (t2 - t1) * p2;
                Vector3 A3 = (t3 - t) / (t3 - t2) * p2 + (t - t2) / (t3 - t2) * p3;

                Vector3 B1 = (t2 - t) / (t2 - t0) * A1 + (t - t0) / (t2 - t0) * A2;
                Vector3 B2 = (t3 - t) / (t3 - t1) * A2 + (t - t1) / (t3 - t1) * A3;

                Vector3 C = (t2 - t) / (t2 - t1) * B1 + (t - t1) / (t2 - t1) * B2;

                if (newPoints3.Count == 0 || newPoints3[newPoints3.Count - 1] != C)
                {
                    newPoints3.Add(C);
                }
            }
        }



    }

    float GetT(float t, Vector3 p0, Vector3 p1)
    {
        // dx * dx + dy * dy + dz * dz
        float a = Mathf.Pow((p1.x - p0.x), 2.0f) + Mathf.Pow((p1.y - p0.y), 2.0f) + Mathf.Pow((p1.z - p0.z), 2.0f);

        // square root of it, which is distance between p0 and p1
        float b = Mathf.Pow(a, 0.5f);

        float c = Mathf.Pow(b, alpha);

        return (c + t);
    }

    //Visualize the points
    void OnDrawGizmos()
    {
        Color r = Color.red;
        r.a = 0.5f;
        Color g = Color.green;
        g.a = 0.5f;
        Color b = Color.blue;
        b.a = 0.5f;
        Gizmos.color = r;

        for (int i = 0; i < newPoints1.Count; i++)
        {
            Vector3 v0 = newPoints1[i];
            Vector3 v1 = newPoints1[(i + 1)%newPoints1.Count];
            Gizmos.color = r;
            Gizmos.DrawLine(v0, v1);
             v0 = leftPoints1[i];
             v1 = leftPoints1[(i + 1) % newPoints1.Count];
            Gizmos.color = g;
            Gizmos.DrawLine(v0, v1);
             v0 = rightPoints1[i];
             v1 = rightPoints1[(i + 1) % newPoints1.Count];
            Gizmos.color = b;
            Gizmos.DrawLine(v0, v1);
        }

        for (int i = 0; i < newPoints2.Count; i++)
        {
            Vector3 v0 = newPoints2[i];
            Vector3 v1 = newPoints2[(i + 1) % newPoints2.Count];
            Gizmos.color = r;
            Gizmos.DrawLine(v0, v1);
            v0 = leftPoints2[i];
            v1 = leftPoints2[(i + 1) % newPoints2.Count];
            Gizmos.color = g;
            Gizmos.DrawLine(v0, v1);
            v0 = rightPoints2[i];
            v1 = rightPoints2[(i + 1) % newPoints2.Count];
            Gizmos.color = b;
            Gizmos.DrawLine(v0, v1);
        }


        for (int i = 0; i < newPoints3.Count; i++)
        {
            Vector3 v0 = newPoints3[i];
            Vector3 v1 = newPoints3[(i + 1) % newPoints3.Count];
            Gizmos.color = r;
            Gizmos.DrawLine(v0, v1);
            v0 = leftPoints3[i];
            v1 = leftPoints3[(i + 1) % newPoints3.Count];
            Gizmos.color = g;
            Gizmos.DrawLine(v0, v1);
            v0 = rightPoints3[i];
            v1 = rightPoints3[(i + 1) % newPoints3.Count];
            Gizmos.color = b;
            Gizmos.DrawLine(v0, v1);
        }
    }

    Vector3 TrackCreation(float gap,Vector3 p0, Vector3 p1)
    {
        Vector3 dir = p0 - p1;
        Vector3 perp = Vector3.Normalize(Vector3.Cross(dir, Vector3.up))*gap;
        return perp;
    }
}