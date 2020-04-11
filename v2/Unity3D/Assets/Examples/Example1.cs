using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using System.Runtime.InteropServices;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using OpenCvSharp;
using System.Runtime.InteropServices;

public class Example1 : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject cadObj;

    void Start()
    {

        
        var cam = Camera.main;
        //cadObj.transform.SetParent(cam.transform);

        Debug.Log("Start");
        var mesh = cadObj.GetComponent<MeshFilter>().mesh;
        var vertices3D = mesh.vertices;
        var normal3D = mesh.triangles;
        Debug.Log("File saved 1");
        TextWriter verticesSynth = new StreamWriter("captures/verts.txt");
            
        var vLen = vertices3D.Length;
        Debug.Log("File saved {vLen}");
        for (int i = 0; i < vLen; i++) 
        {
            // Vector3 vSynth =  cadObj.transform.rotation * vertices3D[i];
            // vSynth = Vector3.Scale(vSynth , cadObj.transform.localScale); 

            Vector3 vReal = vertices3D[i];
            Vector3 vSynth = vReal;
            //Vector3 vSynth = cadObj.transform.TransformPoint(vReal); 

            verticesSynth.WriteLine(vSynth.x + ", " + vSynth.y + ", " +vSynth.z);
            
        }
        Debug.Log("File saved");
        verticesSynth.Flush();

        TextWriter faces = new StreamWriter("captures/faces.txt");
        var fLen = normal3D.Length;

        for (int i = 0; i < fLen; i=i+3) 
            {
                faces.WriteLine(normal3D[i] + ", " + normal3D[i+1] + ", " +normal3D[i+2]);
            }
        faces.Flush();


        TextWriter camMatrixFile = new StreamWriter("captures/CameraMatrix.txt");
        double[,] camMatrix = getCameraMatrix(cam);
        //Matrix4x4 camMatrix =  cam.worldToCameraMatrix ;
            
        camMatrixFile.WriteLine(camMatrix[0, 0] + ", " + camMatrix[0, 1] + ", " +camMatrix[0, 2]);
        camMatrixFile.WriteLine(camMatrix[1, 0] + ", " + camMatrix[1, 1] + ", " +camMatrix[1, 2]);
        camMatrixFile.WriteLine(camMatrix[2, 0] + ", " + camMatrix[2, 1] + ", " +camMatrix[2, 2]);
            
        camMatrixFile.Flush();

        TextWriter rotFile = new StreamWriter("captures/rot.txt");
        TextWriter transFile = new StreamWriter("captures/trans.txt");

        var objTransform = cadObj.transform;
        //objTransform.SetParent(cam.transform);

        //var r = objTransform.localRotation;
        var r = objTransform.rotation;
        
        var t = objTransform.position;
        //t.z = cam.transform.localPosition.z - t.z;
        //var t =  cam.transform.localPosition - objTransform.localPosition ;
        //var t =  cam.transform.position -  objTransform.position;
        Debug.Log($"t  {t}");

        var s = new Vector3(1.0f, 1.0f, 1.0f);
        //var s = objTransform.localScale;
        
        Matrix4x4 m = Matrix4x4.TRS(t, r, s);
        //m = m * transform.worldToLocalMatrix;
        
        Debug.Log($"euler angles  {m}");
        rotFile.WriteLine(m[0,0] + " " + m[0,1] + " " + m[0,2]);
        rotFile.WriteLine(m[1,0] + " " + m[1,1] + " " + m[1,2]);
        rotFile.WriteLine(m[2,0] + " " + m[2,1] + " " + m[2,2]);
        transFile.WriteLine(m[0,3] + " " + m[1,3] + " " + m[2,3]);

        rotFile.Flush();
        transFile.Flush();


        var height = Camera.main.pixelHeight;
        var width = Camera.main.pixelWidth;

        var depth = -1;
        var format = RenderTextureFormat.Default;
        var readWrite = RenderTextureReadWrite.Default;

        var finalRT =
            RenderTexture.GetTemporary(width, height, depth, format, readWrite, 1);

        var renderRT = finalRT;

        var prevActiveRT = RenderTexture.active;
        var prevCameraRT = cam.targetTexture;
        var rTex = renderRT;
        // render to offscreen texture (readonly from CPU side)
        RenderTexture.active = renderRT;
        cam.targetTexture = renderRT;

        cam.Render();

        var tex = rTex.toTexture2D();
        Color32[] texC = tex.GetPixels32();

        var bytes = tex.EncodeToPNG();
        File.WriteAllBytes("captures/pic.png", bytes);

        cam.targetTexture = prevCameraRT;
        RenderTexture.active = prevActiveRT;

        UnityEngine.Object.Destroy(tex);
        RenderTexture.ReleaseTemporary(finalRT);

    }

    private double[, ] getCameraMatrix(Camera cam){

		var f = cam.focalLength;
        var sensorSize = cam.sensorSize;

        var sx = sensorSize.x;
        var sy =  sensorSize.y;
        var width = cam.pixelWidth;
        var height = cam.pixelHeight;

        var l = cam.lensShift;
        var lx = l.x;
        var ly = l.y;

        //Debug.Log(" f " + f + "  sensor " +  sensorSize + " : "+sx+ " width/height " + width + "  :  " + height);

        // double[, ] cameraMatrix = new double[3, 3]
        // {
        //     { (width*f)/sx, 0, -(lx - width / 2) },
        //     { 0, (height*f)/sy, (ly - height / 2) },
        //     { 0, 0, 1}
        // };

        double[, ] cameraMatrix = new double[3, 3]
        {
            { (width*f)/sx, 0, width/2 },
            { 0, (height*f)/sy, height/2 },
            { 0, 0, 1}
        };

		return cameraMatrix;
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
