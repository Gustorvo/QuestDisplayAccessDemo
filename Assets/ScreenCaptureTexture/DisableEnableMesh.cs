using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DisableEnableMesh : MonoBehaviour
{
    [SerializeField] bool includeChildren = true;
    private MeshRenderer meshRenderer;
    private List<MeshRenderer> meshRenderers;
    
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();  

        if (includeChildren)
        {
            meshRenderers = GetComponentsInChildren<MeshRenderer>().ToList();
            if (meshRenderers.Contains(meshRenderer))
            {
                meshRenderers.Remove(meshRenderer);
            }
        }
    }
    
    public void DisableMesh()
    {
        meshRenderer.enabled = false;
        meshRenderers.ForEach(c => c.enabled = false);
    }
    
    public void EnableMesh()
    {
        meshRenderer.enabled = true;
        meshRenderers.ForEach(c => c.enabled = true);
    }
}
