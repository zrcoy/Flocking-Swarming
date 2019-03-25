using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Catmul : MonoBehaviour
{
    public GameObject wp;
    public GameObject wp2;
    public GameObject wp3;
    

    public GameObject CommonGenerator;
    private Common commonScript;

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
    public float amountOfPoints = 10.0f;
    public float trackwidth = 0.5f;
    //set from 0-1
    public float alpha = 0.5f;

    public float GapBetweenTrack = 50.0f;

    /////////////////////////////
    private void Awake()
    {
        
        // get the common script component
        commonScript = CommonGenerator.GetComponent<Common>();


        //setup pointSets1
        pointsSets1 = new Transform[14];
        //fill the ten first
        for (int i = 0; i < 10; i++)
        {
            Vector3 pos = UnityEngine.Random.insideUnitSphere * 20;
            pos.y = 0;
            GameObject w = Instantiate(wp, pos, Quaternion.identity);
            pointsSets1[i] = w.transform;
        }

        //fill in pointSets1 with common wayPoint
        for (int i = 0; i < 4; i++)
        {
            pointsSets1[10 + i] = commonScript.commonWP[i];
        }
       

        newPoints1.Clear();
        leftPoints1.Clear();
        rightPoints1.Clear();
        for (int i = 0; i < pointsSets1.Length; i++)
        {
            CatmulRom(i);
        }
        GenerateParallelLanes();




        //Setup pointsSets
        pointsSets2 = new Transform[14];
        //fill the ten first
        for (int i = 0; i < 10; i++)
        {
            Vector3 pos = UnityEngine.Random.insideUnitSphere * 20 + Vector3.right * (-GapBetweenTrack);
            pos.y = 0;
            GameObject w = Instantiate(wp2, pos, Quaternion.identity);
            pointsSets2[i] = w.transform;
        }

        //fill in pointSets2 with common wayPoint
        for (int i = 0; i < 4; i++)
        {
            pointsSets2[10 + i] = commonScript.commonWP[i];
        }

        newPoints2.Clear();
        leftPoints2.Clear();
        rightPoints2.Clear();
        for (int i = 0; i < pointsSets2.Length; i++)
        {
            CatmulRom2(i);
        }
        GenerateParallelLanes2();




        //Setup pointSets
        pointsSets3 = new Transform[14];
        //fill the ten first
        for (int i = 0; i < 10; i++)
        {
            Vector3 pos = UnityEngine.Random.insideUnitSphere * 20 + Vector3.right * (GapBetweenTrack);
            pos.y = 0;
            GameObject w = Instantiate(wp3, pos, Quaternion.identity);
            pointsSets3[i] = w.transform;
        }

        //then fill in three pointSets with 4 common wayPoint
        for (int i = 0; i < 4; i++)
        {
            pointsSets3[10 + i] = commonScript.commonWP[i];
        }

        newPoints3.Clear();
        leftPoints3.Clear();
        rightPoints3.Clear();
        for (int i = 0; i < pointsSets3.Length; i++)
        {
            CatmulRom3(i);
        }
        GenerateParallelLanes3();



        
    }

    private void GenerateParallelLanes()
    {
        for (int i = 0; i < newPoints1.Count;i++)
        {
            Vector3 tangent = newPoints1[(i+1)%newPoints1.Count]-newPoints1[(i - 1 + newPoints1.Count) % newPoints1.Count];
            Vector3 perp = Vector3.Cross(tangent, Vector3.up);
            Vector3 Nperp = Vector3.Normalize(perp);
            Vector3 left = newPoints1[i] - Nperp * trackwidth;
            Vector3 right = newPoints1[i] + Nperp * trackwidth;
            leftPoints1.Add(left);
            rightPoints1.Add(right);
        }
    }

    private void GenerateParallelLanes2()
    {
        for (int i = 0; i < newPoints2.Count; i++)
        {
            Vector3 tangent = newPoints2[(i + 1) % newPoints2.Count] - newPoints2[(i - 1 + newPoints2.Count) % newPoints2.Count];
            Vector3 perp = Vector3.Cross(tangent, Vector3.up);
            Vector3 Nperp = Vector3.Normalize(perp);
            Vector3 left = newPoints2[i] - Nperp * trackwidth;
            Vector3 right = newPoints2[i] + Nperp * trackwidth;
            leftPoints2.Add(left);
            rightPoints2.Add(right);
        }
    }


    private void GenerateParallelLanes3()
    {
        for (int i = 0; i < newPoints3.Count; i++)
        {
            Vector3 tangent = newPoints3[(i + 1) % newPoints3.Count] - newPoints3[(i - 1 + newPoints3.Count) % newPoints3.Count];
            Vector3 perp = Vector3.Cross(tangent, Vector3.up);
            Vector3 Nperp = Vector3.Normalize(perp);
            Vector3 left = newPoints3[i] - Nperp * trackwidth;
            Vector3 right = newPoints3[i] + Nperp * trackwidth;
            leftPoints3.Add(left);
            rightPoints3.Add(right);
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
        GenerateParallelLanes();

        for (int i = 0; i < pointsSets2.Length; i++)
        {
            CatmulRom2(i);
        }
        GenerateParallelLanes2();

        for (int i = 0; i < pointsSets3.Length; i++)
        {
            CatmulRom3(i);
        }
        GenerateParallelLanes3();
    }
    int getIndex(int index)
    {
        return index % pointsSets1.Length;
    }

    void CatmulRom(int index)
    {


        Vector3 p0 = pointsSets1[getIndex(index+0)].position; // Vector3 has an implicit conversion to Vector2
        Vector3 p1 = pointsSets1[getIndex(index + 1)].position;
        Vector3 p2 = pointsSets1[getIndex(index + 2)].position;
        Vector3 p3 = pointsSets1[getIndex(index + 3)].position;

        float t0 = 0.0f;
        float t1 = GetT(t0, p0, p1);
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

    void CatmulRom2(int index)
    {


        Vector3 p0 = pointsSets2[getIndex(index + 0)].position; // Vector3 has an implicit conversion to Vector2
        Vector3 p1 = pointsSets2[getIndex(index + 1)].position;
        Vector3 p2 = pointsSets2[getIndex(index + 2)].position;
        Vector3 p3 = pointsSets2[getIndex(index + 3)].position;

        float t0 = 0.0f;
        float t1 = GetT(t0, p0, p1);
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

    void CatmulRom3(int index)
    {


        Vector3 p0 = pointsSets3[getIndex(index + 0)].position; // Vector3 has an implicit conversion to Vector2
        Vector3 p1 = pointsSets3[getIndex(index + 1)].position;
        Vector3 p2 = pointsSets3[getIndex(index + 2)].position;
        Vector3 p3 = pointsSets3[getIndex(index + 3)].position;

        float t0 = 0.0f;
        float t1 = GetT(t0, p0, p1);
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

    float GetT(float t, Vector3 p0, Vector3 p1)
    {
        float a = Mathf.Pow((p1.x - p0.x), 2.0f) + Mathf.Pow((p1.y - p0.y), 2.0f)+ Mathf.Pow((p1.z-p0.z),2.0f);
        float b = Mathf.Pow(a, 0.5f);
        float c = Mathf.Pow(b, alpha);

        return (c + t);
    }

    //Visualize the points
    void OnDrawGizmos()
    {
        //Color r = Color.red;
        //r.a = 0.5f;
        //Gizmos.color = r;
        //for  (int i = 0; i < newPoints1.Count; i++)
        //{
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawLine(newPoints1[i], newPoints1[(i+1)%newPoints1.Count]);
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawLine(leftPoints1[i], leftPoints1[(i + 1) % newPoints1.Count]);
        //    Gizmos.color = Color.blue;
        //    Gizmos.DrawLine(rightPoints1[i], rightPoints1[(i + 1) % newPoints1.Count]);
        //}

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
            Vector3 v1 = newPoints1[(i + 1) % newPoints1.Count];
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
            Gizmos.color = Color.black;
            Gizmos.DrawLine(v0, v1);
            v0 = leftPoints2[i];
            v1 = leftPoints2[(i + 1) % newPoints2.Count];
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(v0, v1);
            v0 = rightPoints2[i];
            v1 = rightPoints2[(i + 1) % newPoints2.Count];
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(v0, v1);
        }


        for (int i = 0; i < newPoints3.Count; i++)
        {
            Vector3 v0 = newPoints3[i];
            Vector3 v1 = newPoints3[(i + 1) % newPoints3.Count];
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(v0, v1);
            v0 = leftPoints3[i];
            v1 = leftPoints3[(i + 1) % newPoints3.Count];
            Gizmos.color = Color.white;
            Gizmos.DrawLine(v0, v1);
            v0 = rightPoints3[i];
            v1 = rightPoints3[(i + 1) % newPoints3.Count];
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(v0, v1);
        }

    }


    public Vector3 getPoint(int spline, int lane, int index)
    {
        if (spline == 1)
        {
            if (lane == 1)
            {
                return newPoints1[(index + newPoints1.Count) % newPoints1.Count];
            }
            else if (lane == 2)
            {
                return leftPoints1[(index + newPoints1.Count) % newPoints1.Count];
            }
            else
            {
                return rightPoints1[(index + newPoints1.Count) % newPoints1.Count];
            }
        }
        else if (spline == 2)
        {
            if (lane == 1)
            {
                return newPoints2[(index + newPoints2.Count) % newPoints2.Count];
            }
            else if (lane == 2)
            {
                return leftPoints2[(index + newPoints2.Count) % newPoints1.Count];
            }
            else
            {
                return rightPoints2[(index + newPoints2.Count) % newPoints2.Count];
            }
        }
        else
        {
            if (lane == 1)
            {
                return newPoints3[(index + newPoints3.Count) % newPoints3.Count];
            }
            else if (lane == 2)
            {
                return leftPoints3[(index + newPoints3.Count) % newPoints3.Count];
            }
            else
            {
                return rightPoints3[(index + newPoints3.Count) % newPoints3.Count];
            }
        }
    }


    public int getCount(int spline)
    {
        if (spline == 1)
        {
            return newPoints1.Count;
        }
        else if (spline == 2)
        {
            return newPoints2.Count;
        }
        else
        {
            return newPoints3.Count;
        }
    }



    public int FindCrossingSpline(int currentSpline, Vector3 pos, out int spline)
    {
        spline = -1;
        for (int i = 1; i < 3; i++)
        {
            // i returns current target checking spline index
            if (i == currentSpline)
            {
                continue; // skip to the next one
            }
            for (int j = 0; j < getCount(i); j++)
            {
                // pos = next way point vec3 on the current spline, 
                //getPoint(i, 1, j) returns current target checking spline's middle lane's every point vec3
                // j returns target spline's crossing waypoint index
                if (Vector3.Distance(getPoint(i, 1, j), pos) < 0.5f) // very close
                {
                    spline = i;
                    return j;
                }
            }
        }
        return -1;
    }
}