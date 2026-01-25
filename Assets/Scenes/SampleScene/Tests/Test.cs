using UnityEngine;
using UnityEngine.Playables;

public class Test : MonoBehaviour
{
    public PlayableDirector director;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        director.Play();
    }
}
