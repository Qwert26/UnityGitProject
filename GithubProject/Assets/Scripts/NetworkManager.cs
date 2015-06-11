using UnityEngine;
using System.Collections;
public class NetworkManager : MonoBehaviour {
	private const string typeName="UniqueGameName";
	private const string gameName="RoomName";
	private HostData[] hostList;
	public GameObject player;
	private void startServer() {
		Network.InitializeServer(4,25000,!Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName,gameName);
	}
	void OnServerInitialized() {
		spawnPlayer();
	}
	void OnConnectedToServer() {
		spawnPlayer();
	}
	private void spawnPlayer() {
		Network.Instantiate(player,new Vector3(0,5,0),Quaternion.identity,0);
	}
	void OnGUI() {
		if(!Network.isClient && !Network.isServer) {
			if(GUI.Button(new Rect(100,100,250,100),"Start Server")) {
				startServer();
			}
			if(GUI.Button(new Rect(100,250,250,100),"Refresh Hosts")) {
				refreshHostList();
			}
			if(hostList!=null) {
				for(int i=0;i<hostList.Length;i++) {
					if(GUI.Button(new Rect(400,100+(110*i),300,100),hostList[i].gameName)) {
						joinServer(hostList[i]);
					}
				}
			}
		}
	}
	private void refreshHostList() {
		MasterServer.RequestHostList(typeName);
	}
	void OnMasterServerEvent(MasterServerEvent mse) {
		if(mse==MasterServerEvent.HostListReceived) {
			hostList=MasterServer.PollHostList();
		}
	}
	private void joinServer(HostData hostData) {
		Network.Connect(hostData);
	}
}