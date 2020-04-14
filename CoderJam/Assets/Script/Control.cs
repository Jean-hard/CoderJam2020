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
	public GameObject Projectile { get; set; }

	public static Control instance;
	
	private void Awake()
	{
		instance = this;
		
		Turrets = FindObjectsOfType<Turret>();
		IndexAllTurrets();

		CurrentTurretId = 1;
		Turrets[CurrentTurretId - 1].setHighLight(true);

		ControlMode = ControlType.Turret;
	}

	private void Update()
	{
		if (ControlMode == ControlType.Projectile)
		{

		}
		else if (ControlMode == ControlType.Turret)
		{
			if (Input.GetKeyDown(KeyCode.N))
			{
				NextTurret();
			}
		}
	}

	private void NextTurret()
	{
		Turrets[CurrentTurretId - 1].setHighLight(false);

		CurrentTurretId++;
		if (CurrentTurretId > Turrets.Length)
			CurrentTurretId = 1;

		Turrets[CurrentTurretId - 1].setHighLight(true);

	}

	private void IndexAllTurrets()
	{
		for (int i = 0; i < Turrets.Length; i++)
		{
			Turrets[i].ID = i + 1;
		}
	}
}