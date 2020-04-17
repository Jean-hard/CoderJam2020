using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectil : MonoBehaviour
{
	[SerializeField] private GameObject B;
	[SerializeField] private GameObject P;

	private bool hasEnterInKillZone = false;

	public void Sacrifice()
	{
		P.SetActive(false);
		B.SetActive(true);
	}

	public void ChangeColorP(Color _color)
	{
		P.GetComponent<SpriteRenderer>().color = _color;
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		hasEnterInKillZone = true;
	}

	public bool EnterKillZone()
	{
		return hasEnterInKillZone;
	}
}