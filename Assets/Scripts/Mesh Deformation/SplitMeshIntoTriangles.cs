// C#
// SplitMeshIntoTriangles.cs
using UnityEngine;
using System.Collections;

/// <summary>
/// http://answers.unity3d.com/questions/256558/Convert-mesh-to-triangles-js.html
/// </summary>
public class SplitMeshIntoTriangles : MonoBehaviour {
    private int pillOneTriangleCount = 0;
    private GameObject[] pillOneMeshes = new GameObject[1000];
    private int trianglesDestroyed = 0;

    [SerializeField]
    private GameObject pill;
    private Mesh pillMesh;
    private Renderer pillRenderer;

    void Awake() {
        pillMesh = pill.GetComponent<Mesh>();
        pillRenderer = pill.GetComponent<Renderer>();
    }

    IEnumerator SplitMesh() {
        float slowMotionSpeed = 1f;

        MeshFilter MF = GetComponent<MeshFilter>();
        MeshRenderer MR = GetComponent<MeshRenderer>();
        Mesh M = MF.mesh;
        Vector3[] verts = M.vertices;
        Vector3[] normals = M.normals;
        Vector2[] uvs = M.uv;
        for (int submesh = 0; submesh < M.subMeshCount; submesh++) {
            int[] indices = M.GetTriangles(submesh);
            for (int i = 0; i < indices.Length; i += 3) {

                Vector3[] newVerts = new Vector3[3];
                Vector3[] newNormals = new Vector3[3];
                Vector2[] newUvs = new Vector2[3];
                for (int n = 0; n < 3; n++) {
                    int index = indices[i + n];
                    newVerts[n] = verts[index];
                    newUvs[n] = uvs[index];
                    newNormals[n] = normals[index];
                }
                Mesh mesh = new Mesh();
                mesh.vertices = newVerts;
                mesh.normals = newNormals;
                mesh.uv = newUvs;

                mesh.triangles = new int[] { 0, 1, 2, 2, 1, 0 };

                GameObject GO = new GameObject("Triangle " + (i / 3));
                GO.transform.position = transform.position;
                GO.transform.rotation = transform.rotation;
                GO.AddComponent<MeshRenderer>().material = MR.materials[submesh];
                GO.AddComponent<MeshFilter>().mesh = mesh;
                GO.AddComponent<BoxCollider>();
                GO.AddComponent<Rigidbody>().AddExplosionForce(450 * slowMotionSpeed, transform.position, 50f);
                GO.GetComponent<Rigidbody>().mass = GO.GetComponent<Rigidbody>().mass / slowMotionSpeed;
                /*Rigidbody rb = GO.GetComponent<Rigidbody>();
                rb.mass = 5; //rb.mass * slowMotionSpeed;
                rb.angularDrag = 10; //rb.angularDrag * slowMotionSpeed;
                rb.useGravity = true;*/

                //pillOneMeshes[pillOneTriangleCount] = GO;
                //pillOneTriangleCount++;

                Destroy(GO, 5 + Random.Range(0.0f, 5.0f));
            }
        }
        MR.enabled = false;

        Time.timeScale = slowMotionSpeed;
        Time.fixedDeltaTime = slowMotionSpeed * 0.02f;
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
        //Time.fixedDeltaTime = 0.02f;
        //yield return new WaitForSeconds(1f);
        //Time.timeScale = 1;
        //Time.timeScale = 1.0f;
        //Destroy(gameObject);
    }
    void OnMouseDown() {
        StartCoroutine(SplitMesh());
    }

    void Update() {

      ////////////////////////////////////////////
      /// Optimization Stuff Below this line
      /// 1) Test if objects are out of the camera, and remove them immediately
      /// 2) Remove collider and set to static any shard or debris that rests on the ground
      /// 3) Mix up shards with the particle system
      ////////////////////////////////////////////
   
      //  Debug.Log(pillOneTriangleCount);

        //if(!IsVisibleFrom(pillRenderer, Camera.main)) {
        //    Debug.Log("pill is not visible");
        //}

        //if (pillOneMeshes[0] != null && !pillOneMeshes[0].GetComponent<Renderer>().isVisible) {
        //    Debug.Log("Is not visible");
        //}

            //for(int i = 0; i < pillOneMeshes.Length; i++) {
             //   if(pillOneMeshes[i] != null && !IsVisibleFrom(pillOneMeshes[i].GetComponent<Renderer>(), Camera.main)) {
              //      Destroy(pillOneMeshes[i]);
              //      trianglesDestroyed++;
              //  }
        
            //    if(pillOneMeshes[i] != null && !pillOneMeshes[i].GetComponent<Renderer>().isVisible) {
            //        Debug.Log("Not visible");
            //        Destroy(pillOneMeshes[i]);
            //        pillOneMeshes[i] = null;
            //    }
        //}

        //Debug.Log("Trianlges Destroyed: " + trianglesDestroyed);
    }

    public bool IsVisibleFrom(Renderer renderer, Camera camera) {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}