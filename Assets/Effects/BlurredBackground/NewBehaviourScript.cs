using UnityEditor;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    MeshRenderer meshRenderer;
    void Awake()
    {
    }
    private void OnBecameVisible()
    {
        Debug.Log("OnBecameVisible");
    }
    private void OnBecameInvisible()
    {
        Debug.Log("OnBecameInvisible");
    }
    void SetMesh()
    {
        //GetComponent<MeshRenderer>().bounds = new Bounds(transform.position, Vector3.one);
    }

    void OnDrawGizmos()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.bounds = new Bounds(transform.position, Vector3.one);
        var bounds = meshRenderer.bounds;
        Gizmos.DrawCube(bounds.center, bounds.size);
        //Debug.Log(bounds.center+""+ bounds.size);
    }
}
