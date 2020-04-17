using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public enum ControlType
{
	Turret,
	Projectile
}

public class Control : MonoBehaviour
{
	public ControlType ControlMode { get; set; }
	public Turret[] Turrets { get; set; }
	public int CurrentTurretId { get; private set; }
	public Projectil projectile { get; set; }

	public static Control instance;

	private bool projectilIsTransforming;
	
	private void Awake()
	{
		instance = this;
		
		Turrets = FindObjectsOfType<Turret>();
		IndexAllTurrets();

		CurrentTurretId = 0;
		Turrets[CurrentTurretId].SetHighLight(true);

		ControlMode = ControlType.Turret;
	}

	private void Update()
	{
		if (ControlMode == ControlType.Projectile && !projectilIsTransforming)
		{
			if (Input.GetButtonDown("Fire"))
			{
				StartCoroutine(ProjectilTransformation());
			}
		}
		else if (ControlMode == ControlType.Turret)
		{
			if (Input.GetKeyDown(KeyCode.N))
			{
				NextTurret();
			}
		}

		if (projectile != null)
		{
			if (projectile.EnterKillZone())
			{
				StopCoroutine(ProjectilTransformation());
				Destroy(projectile.gameObject);
				ControlMode = ControlType.Turret;
				projectilIsTransforming = false;
				NextTurret();
			}
		}
	}

	private float AngleRot;
	private IEnumerator ProjectilTransformation()
	{
		projectilIsTransforming = true;
		projectile.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
		projectile.Sacrifice();

		AngleRot = Mathf.Ceil(projectile.transform.eulerAngles.z);

		yield return null;

		while (!Input.GetButtonDown("Fire"))
		{
			AngleRot -= Input.GetAxis("Horizontal");

			projectile.transform.rotation = Quaternion.Euler(0, 0, AngleRot);

			yield return null;
		}

		GameManager.Instance.HasShot(projectile);

		ControlMode = ControlType.Turret;
		projectilIsTransforming = false;
		NextTurret();
	}

	private void NextTurret()
	{
		Turrets[CurrentTurretId].SetHighLight(false);

		CurrentTurretId++;
		if (CurrentTurretId >= Turrets.Length)
			CurrentTurretId = 0;

		Turrets[CurrentTurretId].SetHighLight(true);

	}

	private void IndexAllTurrets()
	{
		for (int i = 0; i < Turrets.Length; i++)
		{
			Turrets[i].ID = i;
		}
	}
}