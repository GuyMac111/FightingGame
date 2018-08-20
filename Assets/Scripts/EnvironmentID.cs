using UnityEngine;
using System.Collections;

public class EnvironmentID : MonoBehaviour {
	
	[SerializeField]
	public EnvironmentObject envType;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public enum EnvironmentObject {
		WALL,FLOOR
	}
}
