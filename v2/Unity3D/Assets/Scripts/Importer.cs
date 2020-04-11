using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Importer : MonoBehaviour
{
    // Start is called before the first frame update

    public string fileLoc = "/home/sourabh/Documents/TU-Berlin/Thesis/Sytheticdata/ml-imagesynthesis/Assets/CADPictures/linmod/duck/duck.obj";

    public string meshname;

    void Start()
    {
        ObjImporter newMesh = new ObjImporter();
        Mesh holderMesh = newMesh.ImportFile(fileLoc);

        // Mesh holderMesh  = FastObjImporter.Instance.ImportFile(fileLoc);

        MeshSaver.SaveMesh(holderMesh, meshname);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
