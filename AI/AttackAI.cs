using Matcha.Dreadful;
using Matcha.Unity;
using System.Collections;
using UnityEngine;

public class AttackAI : CacheBehaviour
{
	public enum Style { RandomProjectile };
	public Style style;
	public float chanceOfAttack	 = 40f;
	public float attackWhenInRange = 30f;
	public bool attackPaused;

	private ProjectileManager projectile;
	private Weapon weapon;
	private Transform target;
	private float attackInterval;
	private bool levelLoading;
	private bool dead;

	void Start()
	{
		projectile		= GetComponent<ProjectileManager>();
		weapon			= GetComponentInChildren<Weapon>();
		target			= GameObject.Find(PLAYER).transform;
		attackInterval = Rand.Range(1.5f, 2.5f);
	}

	// master controller
	void OnBecameVisible()
	{
		if (!attackPaused && !dead)
		{
			if (test)
			{
				StartCoroutine(LobCompTest());
			}
			else
			{
				switch (style)
				{
					case Style.RandomProjectile:
					{
						InvokeRepeating("AttackRandomly", 2f, attackInterval);
						break;
					}
				}
			}
		}
	}

	void AttackRandomly()
	{
		if (!attackPaused && !levelLoading && !dead)
		{
			float distance = Vector3.Distance(target.position, transform.position);

			if (distance <= attackWhenInRange && !MDebug.attackDisabled)
			{
				if (Rand.Range(1, 100) <= chanceOfAttack)
				{
					// only attack if creature is facing the direction of target
					if ((target.position.x > transform.position.x && transform.localScale.x.FloatEquals(1f)) ||
							(target.position.x < transform.position.x && transform.localScale.x.FloatEquals(-1f)))
					{
						projectile.FireAtTarget(weapon, target);
					}
				}
			}
		}
	}

	void RotateTowardsTarget()
	{
		if (!attackPaused && !dead)
		{
			Vector3 vel = GetForceFrom(transform.position, target.position);
			float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
			transform.eulerAngles = new Vector3(0, 0, angle);
		}
	}

	static Vector2 GetForceFrom(Vector3 fromPos, Vector3 toPos)
	{
		const float power = 1;

		return (new Vector2(toPos.x, toPos.y) - new Vector2(fromPos.x, fromPos.y)) * power;
	}

	void CreatureDead()
	{
		dead = true;
		attackPaused = true;
		this.enabled = false;
	}

	void OnBecameInvisible()
	{
		if (!test && !dead)
		{
			CancelInvoke("AttackRandomly");
			StopCoroutine(LobCompTest());
		}
	}

	void OnDisable()
	{
		CancelInvoke();
		StopCoroutine(LobCompTest());
	}

	void OnLevelLoading(bool status)
	{
		// pause attacks and other activities while level loads
		levelLoading = true;

		StartCoroutine(Timer.Start(ENEMY_PAUSE_ON_LEVEL_LOAD, false, () =>
		{
			levelLoading = false;
		}));
	}

	void OnEnable()
	{
		EventKit.Subscribe<bool>("level loading", OnLevelLoading);
	}

	void OnDestroy()
	{
		EventKit.Unsubscribe<bool>("level loading", OnLevelLoading);
	}



	// TARGET TESTING SUITE
	// ####################

	public bool test;
	public GameObject[] targets;
	// testing only

	IEnumerator LobCompTest()
	{
		int i = 0;
		int j = i - 1;

		while (true)
		{
			if (i >= targets.Length) {
				i = 0;
			}

			if (j >= targets.Length) {
				j = 0;
			}

			if (j < 0) {
				j = 10;
			}

			targets[i].GetComponent<SpriteRenderer>().material.SetColor("_Color", MCLR.orange);
			targets[j].GetComponent<SpriteRenderer>().material.SetColor("_Color", MCLR.white);
			projectile.FireAtTarget(weapon, targets[i].transform);
			i++;
			j++;
			yield return new WaitForSeconds(2);
		}
	}
}
