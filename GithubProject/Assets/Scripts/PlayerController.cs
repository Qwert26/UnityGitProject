using UnityEngine;
using System.Collections;
public class PlayerController : MonoBehaviour {
	public float speed=10f;
	private float lastSynchronizationTime,syncDelay,syncTime;
	private Vector3 syncStartPosition,syncEndPosition;
	void Update() {
		if (GetComponent<NetworkView>().isMine) {
			inputMovement ();
			inputColorChange();
		} else {
			syncedMovement();
		}
	}
	private void inputColorChange() {
		if(Input.GetKeyDown(KeyCode.R)) {
			changeColorTo(new Vector3(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f)));
		}
	}
	[RPC] void changeColorTo(Vector3 color) {
		GetComponent<Renderer> ().material.color = new Color (color.x,color.y,color.z,1);
		if(GetComponent<NetworkView>().isMine) {
			GetComponent<NetworkView>().RPC("changeColorTo",RPCMode.OthersBuffered,color);
		}
	}
	private void syncedMovement() {
		syncTime += Time.deltaTime;
		GetComponent<Rigidbody>().position = Vector3.Lerp(syncStartPosition,syncEndPosition,syncTime/syncDelay);
	}
	private void inputMovement() {
		GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position+Vector3.forward*speed*Input.GetAxis("Vertical")*Time.deltaTime);
		GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position+Vector3.right*speed*Input.GetAxis("Horizontal")*Time.deltaTime);
	}
	void OnSerializeNetworkView(BitStream stream,NetworkMessageInfo info) {
		Vector3 syncPosition = Vector3.zero;
		Vector3 syncVelocity = Vector3.zero;
		if (stream.isWriting) {
			syncPosition = GetComponent<Rigidbody>().position;
			stream.Serialize (ref syncPosition);

			syncVelocity=GetComponent<Rigidbody>().velocity;
			stream.Serialize(ref syncVelocity);
		} else {
			stream.Serialize(ref syncPosition);
			stream.Serialize(ref syncVelocity);

			syncTime=0;
			syncDelay=Time.time-lastSynchronizationTime;
			lastSynchronizationTime=Time.time;

			syncEndPosition=syncPosition+syncVelocity*syncDelay;
			syncStartPosition=GetComponent<Rigidbody>().position;
		}
	}
}