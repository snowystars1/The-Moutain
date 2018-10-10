using UnityEngine;
using System.Collections;

public class JigglyBlendShapeAnimation : MonoBehaviour
{
    public float speed = 3.0f;
    private ParticleSystem splatParticles;

    SkinnedMeshRenderer smr;
    int shape1;
    int shape2;

    void Start ()
    {
        splatParticles = GetComponent<ParticleSystem>();
        smr = GetComponent<SkinnedMeshRenderer> ();

        int shapeCount = smr.sharedMesh.blendShapeCount;
        shape1 = Random.Range (0, shapeCount);
        shape2 = Random.Range (0, shapeCount);

        while (shape1 == shape2)
        {
            shape2 = Random.Range (0, shapeCount);
        }

        speed *= Random.Range (0.8f, 1.2f);
    }

    void Update()
    {
        var ratio = 50.0f * (Mathf.Sin(Time.time * speed) + 1.0f);
        smr.SetBlendShapeWeight(shape1, ratio);
        smr.SetBlendShapeWeight(shape2, 100.0f - ratio);
    }

    void OnTriggerEnter(Collider other)
    {
        if(!splatParticles.isPlaying)
            splatParticles.Play();
    }
}
