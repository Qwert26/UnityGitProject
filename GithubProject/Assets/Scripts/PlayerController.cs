using UnityEngine;
using System.Collections;
public class PlayerController : MonoBehaviour {
	public float speed=10f;
	private Rigidbody rigidbody;
	private float lastSynchronizationTime,syncDelay,syncTime;
	private Vector3 syncStartPosition,syncEndPosition;
	void Start() {
		rigidbody=GetComponent<Rigidbody>();
	}
	void Update() {
		if (GetComponent<NetworkView> ().isMine) {
			inputMovement ();
		} else {
			syncedMovement();
		}
	}
	private void syncedMovement() {
		syncTime += Time.deltaTime;
		rigidbody.position = Vector3.Lerp(syncStartPosition,syncEndPosition,syncTime/syncDelay);
	}
	private void inputMovement() {
		rigidbody.MovePosition(rigidbody.position+Vector3.forward*speed*Input.GetAxis("Vertical")*Time.deltaTime);
		rigidbody.MovePosition(rigidbody.position+Vector3.right*speed*Input.GetAxis("Horizontal")*Time.deltaTime);
	}
	void OnSerializeNetworkView(BitStream stream,NetworkMessageInfo info) {
		Vector3 syncPosition = Vector3.zero;
		Vector3 syncVelocity = Vector3.zero;
		if (stream.isWriting) {
			syncPosition = rigidbody.position;
			stream.Serialize (ref syncPosition);

			syncVelocity=rigidbody.velocity;
			stream.Serialize(ref syncVelocity);
		} else {
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncVelocity);

			syncTime=0;
			syncDelay=Time.time-lastSynchronizationTime;
			lastSynchronizationTime=Time.time;

			syncEndPosition=syncPosition+syncVelocity*syncDelay;
			syncStartPosition=rigidbody.position;
		}
	}
}