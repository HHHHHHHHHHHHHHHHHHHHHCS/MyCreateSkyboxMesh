using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitySHArgb
{
    //SphericalHarmonicsL2.AddAmbientLight
    //SphericalHarmonicsL2.AddDirectionalLight  (0,1,0)
    //SphericalHarmonicsL2.AddDirectionalLight  (0,-1,0)

    // https://forum.unity.com/threads/how-is-light-probe-data-input-into-shaders.475219/
    //    Edit:
    //    Figured out the values.In case anyone else needs this the answer is
    //
    //    for each color c[red c = r or 0, green c = g or 1, blue c = b or 2]
    //    SHAc = float4(probe[c, 3], probe[c, 1], probe[c, 2], probe[c, 0] - probe[c, 6])
    //    SHBc = float4(probe[c, 4], probe[c, 5], 3 * probe[0, 6], probe[0, 7])
    //    and SHC = float3(probe[0, 8], probe[1, 8], probe[2, 8])
    //
    //    then, as in UnityCG.cginc
    //        L0.c = SHAc.w;
    //    L1.c = SHAc.xyz* normal;
    //    L2.c = float5(SHBc.xyzw, SHC.c) * float5((normal.xyzz* normal.yzzx), normal.x^2-normal.z^2);
}
