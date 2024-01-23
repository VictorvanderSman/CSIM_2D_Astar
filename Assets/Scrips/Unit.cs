using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {


	public Transform target;
	public GameObject[] targets;

	float speed = 20;
	Vector2[] path;
	int targetIndex;
	public int PreviousOption = 999;

	void Start() {

		targets = GameObject.FindGameObjectsWithTag("Targets");
		target = FindTarget();
		PathfindingManager.RequestPath(transform.position,target.position, OnPathFound);
	}
	

	public void OnPathFound(Vector2[] newPath, bool pathSuccessful) {
		if (pathSuccessful) {
			path = newPath;
			targetIndex = 0;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}
	public Transform FindTarget()
	{
		int RandomOption = Random.Range(0, targets.Length);
		
		while(RandomOption == PreviousOption)
			RandomOption = Random.Range(0, targets.Length);
		
		PreviousOption = RandomOption;
		
		return targets[RandomOption].transform;
	}

	IEnumerator FollowPath() {

		Vector3 currentWaypoint = path[0];

		while (true) {
			if (transform.position == currentWaypoint) {
				targetIndex ++;
				if (targetIndex >= path.Length) {

					// at the end of the path, find a new target
					target = FindTarget();
					PathfindingManager.RequestPath(transform.position,target.position, OnPathFound);

					yield break;
				}
				currentWaypoint = path[targetIndex];
			}

			transform.position = Vector2.MoveTowards(transform.position,currentWaypoint,speed * Time.deltaTime);
			yield return null;

		}
	}

	public void OnDrawGizmos() {
		if (path != null) {
			for (int i = targetIndex; i < path.Length; i ++) {
				Gizmos.color = Color.black;
				Gizmos.DrawCube(path[i], Vector2.one);

				if (i == targetIndex) {
					Gizmos.DrawLine(transform.position, path[i]);
				}
				else {
					Gizmos.DrawLine(path[i-1],path[i]);
				}
			}
		}
	}
}
