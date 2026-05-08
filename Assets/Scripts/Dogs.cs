using System.Collections;
using System.Collections.Generic;
using MainScripts.Audio;
using UnityEngine;

public class Dogs : MonoBehaviour
{
    [SerializeField] private AudioLibrary _audioLibrary;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Vector2 _range;
    
    void Start()
    {
        StartCoroutine(DogsRoutine());
    }

    private IEnumerator DogsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(_range.x, _range.y));
            _audioSource.PlayOneShot(_audioLibrary.GetRandom("Dogs"));
        }
    }
}
