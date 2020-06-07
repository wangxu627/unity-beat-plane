using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleRendererRandom : MonoBehaviour
{
    public ParticleSystemRenderer particleSystemRenderer;
    public Material[] materials;
    // Start is called before the first frame update
    void Start()
    {
        particleSystemRenderer.material = materials[Random.Range(0, materials.Length)];
    }
}
