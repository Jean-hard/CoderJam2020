using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectil : MonoBehaviour
{
	[SerializeField] private GameObject B;
	[SerializeField] private GameObject P;

	public void Sacrifice()
	{
		P.SetActive(false);
		B.SetActive(true);
	}
}