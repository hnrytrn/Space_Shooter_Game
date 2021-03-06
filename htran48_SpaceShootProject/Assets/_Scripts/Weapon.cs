﻿using UnityEngine;
using System.Collections;

//an enum of the various possible weapon types, and a shield type
public enum WeaponType {
	none, //default / no weapong
	blaster, //simple blaster
	spread, //two shots simultaneously
	shield //raise shieldLevel
}

//allows us to set the properties of a specific weapon
[System.Serializable]
public class WeaponDefinition {
	public WeaponType type = WeaponType.none;
	public string letter; // letter to show on the power-up
	public Color color = Color.white; //color of collar & power-up
	public GameObject projectilePrefab; //prefab for projectiles
	public Color projectileColor = Color.white;
	public float damageOnHit = 0; //Amount of damage caused
	public float delayBetweenShots = 0;
	public float velocity = 20; // speed of projectiles
}

public class Weapon : MonoBehaviour {
	static public Transform PROJECTILE_ANCHOR;

	public bool _______________;
	[SerializeField]
	private WeaponType _type = WeaponType.none;
	public WeaponDefinition def;
	public GameObject collar;
	public float lastShot;//Time last shot was fired

	void Awake() {
		collar = transform.Find ("Collar").gameObject;
	}

	void Start () {
		SetType (_type);

		if (PROJECTILE_ANCHOR == null) {
			GameObject go = new GameObject ("_Projectile_Anchor");
			PROJECTILE_ANCHOR = go.transform;
		}
		//find the fireDelefate of the parent
		GameObject parentGO = transform.parent.gameObject;
		if (parentGO.tag == "Hero") {
			Hero.S.fireDelegate += Fire;
		}
	}

	public WeaponType type {
		get { return(_type); }
		set { SetType (value); }
	}

	public void SetType (WeaponType wt) {
		_type = wt;
		if (type == WeaponType.none) {
			this.gameObject.SetActive (false);
			return;
		} else {
			this.gameObject.SetActive (true);
		}
		def = Main.GetWeaponDefinition (_type);
		collar.GetComponent<Renderer> ().material.color = def.color;
		lastShot = 0;
	}

	public void Fire() {
		//If this.gameObject is inactive, return
		if (!gameObject.activeInHierarchy) return;
		//if it hasn't been enough time between shots, return
		if (Time.time - lastShot < def.delayBetweenShots) {
			return;
		}
		Projectile p;
		switch (type) {
		case WeaponType.blaster:
			p = MakeProjectile ();
			p.GetComponent<Rigidbody> ().velocity = Vector3.up * def.velocity;
			break;

		case WeaponType.spread:
			p = MakeProjectile ();
			p.GetComponent<Rigidbody> ().velocity = Vector3.up * def.velocity;
			p = MakeProjectile ();
			p.GetComponent<Rigidbody> ().velocity = new Vector3 (-.2f, 0.9f, 0) * def.velocity;
			p = MakeProjectile ();
			p.GetComponent<Rigidbody> ().velocity = new Vector3 (.2f, 0.9f, 0) * def.velocity;
			break;
		}
	}

	public Projectile MakeProjectile() {
		GameObject go = Instantiate (def.projectilePrefab) as GameObject;
		if (transform.parent.gameObject.tag == "Hero") {
			go.tag = "ProjectileHero";
			go.layer = LayerMask.NameToLayer ("ProjectileHero");
		} else {
			go.tag = "ProjectileEnemy";
			go.layer = LayerMask.NameToLayer ("ProjectileEnemy");
		}
		go.transform.position = collar.transform.position;
		go.transform.parent = PROJECTILE_ANCHOR;
		Projectile p = go.GetComponent<Projectile> ();
		p.type = type;
		lastShot = Time.time;
		return(p);
	}
		
}
