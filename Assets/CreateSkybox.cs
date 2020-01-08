using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;

//天空盒顶点数据 在单位测试的时候 会自动释放
//否则一但生成  除非硬件发生改变  不然数据会常驻在内存中
public class CreateSkybox
{
    private const int NUM_FULL_SUBDIVISIONS = 3;
    private const int NUM_HORIZON_SUBDIVISIONS = 2;

    private float[] octaVerts = new[]
    {
        0.0f, 1.0f, 0.0f,
        0.0f, 0.0f, -1.0f,
        1.0f, 0.0f, 0.0f,

        0.0f, 1.0f, 0.0f,
        1.0f, 0.0f, 0.0f,
        0.0f, 0.0f, 1.0f,

        0.0f, 1.0f, 0.0f,
        0.0f, 0.0f, 1.0f,
        -1.0f, 0.0f, 0.0f,

        0.0f, 1.0f, 0.0f,
        -1.0f, 0.0f, 0.0f,
        0.0f, 0.0f, -1.0f,

        0.0f, -1.0f, 0.0f,
        1.0f, 0.0f, 0.0f,
        0.0f, 0.0f, -1.0f,

        0.0f, -1.0f, 0.0f,
        0.0f, 0.0f, 1.0f,
        1.0f, 0.0f, 0.0f,

        0.0f, -1.0f, 0.0f,
        -1.0f, 0.0f, 0.0f,
        0.0f, 0.0f, 1.0f,

        0.0f, -1.0f, 0.0f,
        0.0f, 0.0f, -1.0f,
        -1.0f, 0.0f, 0.0f
    };

    [MenuItem("Skybox/Create")]
    public static void DoMain()
    {
        new CreateSkybox().Create();
    }

    public void Create()
    {
        //8 * 3  * 4 * 4 * 4 = 1536
        int cap = 8 * 3 * (int) Mathf.Pow(4, NUM_FULL_SUBDIVISIONS);
        List<CubemapSkyboxVertex> vData =
            new List<CubemapSkyboxVertex>(cap);

        for (int i = 0; i < 8 * 3; i++)
        {
            CubemapSkyboxVertex vert;
            Vector3 n = Vector3.Normalize(new Vector3(octaVerts[i * 3 + 0], octaVerts[i * 3 + 1],
                octaVerts[i * 3 + 2]));
            vert.x = vert.tu = n.x;
            vert.y = vert.tv = n.y;
            vert.z = vert.tw = n.z;
            vert.color = Color.white;
            vData.Add(vert);
        }

        SubdivideSkybox(ref vData, NUM_FULL_SUBDIVISIONS);

        for (int i = 0; i < NUM_HORIZON_SUBDIVISIONS; ++i)
        {
            CubemapSkyboxVertex[] srcData = vData.ToArray();

            int srcVertCount = srcData.Length;

            float horizonLimit = Mathf.Pow(0.5f, (float) i + 1.0f); //0.5 0.25 

            vData.Clear();

            for (int j = 0; j < srcVertCount; j += 3)
            {
                float maxAbsY = Mathf.Max(Mathf.Abs(srcData[j].y), Mathf.Abs(srcData[j + 1].y),
                    Mathf.Abs(srcData[j + 2].y));

                if (maxAbsY > horizonLimit)
                {
                    vData.Add(srcData[j]);
                    vData.Add(srcData[j + 1]);
                    vData.Add(srcData[j + 2]);
                }
                else
                {
                    SubdivideYOnly(ref vData, ref srcData[j], ref srcData[j + 1], ref srcData[j + 2]);
                }
            }

        }

        Debug.Log(vData.Count);
    }


    private CubemapSkyboxVertex SubDivVert(ref CubemapSkyboxVertex v1, ref CubemapSkyboxVertex v2)
    {
        CubemapSkyboxVertex res;

        Vector3 p1 = new Vector3(v1.x, v1.y, v1.z);
        Vector3 p2 = new Vector3(v2.x, v2.y, v2.z);

        Vector3 h = Vector3.Normalize(Vector3.Lerp(p1, p2, 0.5f));
        res.x = res.tu = h.x;
        res.y = res.tv = h.y;
        res.z = res.tw = h.z;
        res.color = Color.white;
        return res;
    }

    private void Subdivide(ref List<CubemapSkyboxVertex> destArray, ref CubemapSkyboxVertex v1,
        ref CubemapSkyboxVertex v2,
        ref CubemapSkyboxVertex v3)
    {
        CubemapSkyboxVertex v12 = SubDivVert(ref v1, ref v2);
        CubemapSkyboxVertex v23 = SubDivVert(ref v2, ref v3);
        CubemapSkyboxVertex v13 = SubDivVert(ref v1, ref v3);

        destArray.Add(v1);
        destArray.Add(v12);
        destArray.Add(v13);
        destArray.Add(v12);

        destArray.Add(v2);
        destArray.Add(v23);
        destArray.Add(v23);
        destArray.Add(v13);
        destArray.Add(v12);

        destArray.Add(v3);
        destArray.Add(v13);
        destArray.Add(v23);
    }

    private void SubdivideSkybox(ref List<CubemapSkyboxVertex> vertexData, int subdivisions)
    {
        for (int i = 0; i < subdivisions; ++i)
        {
            CubemapSkyboxVertex[] srcData = vertexData.ToArray();

            int srcVertCount = srcData.Length;

            vertexData.Clear();

            int endIndex = 0;

            for (int k = 0; k < srcVertCount; k += 3)
            {
                Subdivide(ref vertexData, ref srcData[k], ref srcData[k + 1], ref srcData[k + 2]);
            }
        }
    }


    private void SubdivideYOnly(ref List<CubemapSkyboxVertex> destArray, ref CubemapSkyboxVertex v1,
        ref CubemapSkyboxVertex v2,
        ref CubemapSkyboxVertex v3)
    {
        float d12 = Mathf.Abs(v2.y - v1.y);
        float d23 = Mathf.Abs(v2.y - v3.y);
        float d31 = Mathf.Abs(v3.y - v1.y);

        CubemapSkyboxVertex top, va, vb;

        if (d12 < d23 && d12 < d31)
        {
            top = v3;
            va = v1;
            vb = v2;
        }
        else if (d23 < d12 && d23 < d31)
        {
            top = v1;
            va = v2;
            vb = v3;
        }
        else
        {
            top = v2;
            va = v3;
            vb = v1;
        }

        CubemapSkyboxVertex v12 = SubDivVert(ref top, ref va);
        CubemapSkyboxVertex v13 = SubDivVert(ref top, ref vb);

        destArray.Add(top);
        destArray.Add(v12);
        destArray.Add(v13);

        Vector3 a = new Vector3(v13.x - va.x, v13.y - va.y, v13.z - va.z);
        Vector3 b = new Vector3(v12.x - vb.x, v12.y - vb.y, v12.z - vb.z);

        if (a.sqrMagnitude > b.sqrMagnitude)
        {
            destArray.Add(v12);
            destArray.Add(va);
            destArray.Add(vb);
            destArray.Add(v13);
            destArray.Add(v12);
            destArray.Add(vb);
        }
        else
        {
            destArray.Add(v13);
            destArray.Add(v12);
            destArray.Add(va);
            destArray.Add(v13);
            destArray.Add(va);
            destArray.Add(vb);
        }
    }
}