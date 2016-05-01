using UnityEngine;
using System.Collections;

public class DeleteAnim : MonoBehaviour {

	void Start()
	{
		Play ();
	}

	//play delete animation
	void Play()
	{
		TweenPosition tp = gameObject.AddComponent<TweenPosition> ();
		tp.from = gameObject.transform.localPosition ;
		tp.to = tp.from + new Vector3 (-1500f, 0);

		Debug.Log (gameObject.name + " Play anim.");

		gameObject.GetComponent<TweenPosition> ().enabled = true;
		gameObject.GetComponent<TweenPosition> ().PlayForward ();
	}
}
