using System.Collections;
using System.Collections.Generic;
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


    public static void DoMain()
    {
    }

    public void Create()
    {
        CubemapSkyboxVertex[] vertData = new CubemapSkyboxVertex[24];

        for (int i = 0; i < 8 * 3; i++)
        {
            CubemapSkyboxVertex vert;
            Vector3 n = Vector3.Normalize(new Vector3(octaVerts[i * 3 + 0], octaVerts[i * 3 + 1],
                octaVerts[i * 3 + 2]));
            vert.x = vert.tu = n.x;
            vert.y = vert.tv = n.y;
            vert.z = vert.tw = n.z;
            vert.color = Color.white;
            vertData[i] = vert;
        }
    }
}