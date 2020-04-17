using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
	public int ID { get; set; }
	[SerializeField] private float AngleSpeed = 1;
	private float AngleRot = 0;
	[SerializeField] private GameObject prefab;
	[SerializeField] private Transform spawn;
	[SerializeField] private float projectilSpeed = 10.0f;
	[SerializeField] private GameObject highlight;

	private void Update()
	{
		if (ID == Control.instance.CurrentTurretId && Control.instance.ControlMode == ControlType.Turret)
		{
			AdjustTurretAngle();

			if (Input.GetButtonDown("Fire"))
			{
				Fire();
			}
		}
	}

	private void Fire()
	{
		GameObject g = Instantiate(prefab, spawn.position, transform.rotation, GameManager.Instance.bulletParent.transform);
		g.GetComponent<Rigidbody2D>().velocity = (g.transform.position - transform.parent.position).normalized * projectilSpeed;
		Control.instance.projectile = g.GetComponent<Projectil>();
		StartCoroutine(SwitchMode());
	}

	private IEnumerator SwitchMode()
	{
		yield return new WaitForEndOfFrame();
		Control.instance.ControlMode = ControlType.Projectile;
	}

	private void AdjustTurretAngle()
	{
		AngleRot -= Input.GetAxis("Horizontal");

		if (AngleRot < 0)
			AngleRot = 359;
		else if (AngleRot > 359)
			AngleRot = 0;

		transform.rotation = Quaternion.Euler(0, 0, AngleRot);
	}

	public void SetHighLight(bool b)
	{
		if (b)
			highlight.SetActive(true);
		else
			highlight.SetActive(false);
	}
}