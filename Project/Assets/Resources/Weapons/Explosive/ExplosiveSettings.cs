using UnityEngine;
using System.Collections;

public class ExplosiveSettings : MonoBehaviour {

	public float explosiveDamage;
	public float explosionRadius;
	public GameObject owner;

	void Start()
	{
		Invoke("ForgetOwner", 5);
	}

	public void Detonate()
	{
		GameObject BombAudio = new GameObject();
		BombAudio.transform.position = transform.position;
		BombAudio.AddComponent<AudioSource>();
		BombAudio.GetComponent<AudioSource>().clip = (AudioClip)Resources.Load("Weapons/Defensive/Bomb/Assets/BombExplosion", typeof(AudioClip));
		BombAudio.GetComponent<AudioSource>().minDistance = 20;
		BombAudio.GetComponent<AudioSource>().Play();

		GameObject bombObject = (GameObject)Instantiate(Resources.Load("Weapons/Defensive/Bomb/BombExplosion", typeof(GameObject)));
		bombObject.transform.position = transform.position;

		Destroy(this.gameObject);
	}

	void ForgetOwner()
	{
		owner = null;
	}

}
