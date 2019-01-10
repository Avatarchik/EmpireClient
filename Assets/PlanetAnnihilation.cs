using UnityEngine;

public class PlanetAnnihilation : MonoBehaviour {

    public AudioSource a;

	// Use this for initialization
	void Start () {
        a.Play();
	}
	
	// Update is called once per frame
	void Update () {
        if (!a.isPlaying)
            Destroy(gameObject);
		
	}
}
