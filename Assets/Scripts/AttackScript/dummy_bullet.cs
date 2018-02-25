using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class dummy_bullet : NetworkBehaviour {
	public float speed;
	public float distanceToLive = 2.0F;
	public float damage;
	private Vector3 startingPoint;
	private GameObject originGameObject;

	public void setOriginGameObject(GameObject obj)
	{
		originGameObject = obj;
	}

	public GameObject getOriginGameObject()
	{
		return originGameObject;
	}

	void Start()
	{
		startingPoint = transform.position;
		gameObject.GetComponent<Rigidbody>().velocity = transform.forward * speed;


		//l'idée est ici de dissocier la bullet du particle system puisque celui ci doit continuer a briller meme quand
		//la bullet a été destroyed
		var particle_system = transform.GetChild(0).gameObject as GameObject;
		if (particle_system)
		{
			//on applique la meme vitesse au particle system, pour que celui ci suivent la trajectoire de la bullet
			particle_system.GetComponent<Rigidbody>().velocity = transform.forward * speed;
			ParticleSystem ps = particle_system.GetComponent<ParticleSystem>();
			ps.Stop();
			var main = ps.main;
			main.duration = distanceToLive / speed; //la duration doit etre précisément la meme que celle de la bullet
			//sinon le joueur aura l'impression que la balle va plus ou moins loin qu'elle ne va réellement!
			ps.Play();

			Destroy(particle_system, main.duration + 8); //on estime que de base, la majorité des particles se seront
			//éteinte 8 secondes apres le temps de la période d'emission.
			//On détruit donc le particle system apres ce délai
		}

		transform.DetachChildren();
	}

	// Update is called once per frame
	void Update () 
	{
		if(Vector3.Distance(startingPoint, transform.position) > distanceToLive)
			Destroy(gameObject);
	}

	void OnTriggerEnter(Collider other)
	{
		var statSystem = other.GetComponent<StatSystem>();
		if (statSystem != null && other.gameObject != originGameObject)
		{
			statSystem.substract("health", damage);
			Destroy(gameObject);
		}
	}
}
