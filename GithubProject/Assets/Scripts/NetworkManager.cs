using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class NetworkManager : MonoBehaviour {
	private const string typeName="UniqueGameName";
	private const string gameName="Great Test";
	private HostData[] hostList;
	public GameObject player;
	public GameObject buttonJoinRoom;
	public GameObject buttonStartServer,buttonRefreshHosts,buttonDisconnect;
	public void startServer() {
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
			if(hostList!=null) {
				for(int i=0;i<hostList.Length;i++) {
					if(GUI.Button(new Rect(400,100+(110*i),300,100),hostList[i].gameName)) {
						joinServer(hostList[i]);
					}
				}
			}
		}
	}
	public void refreshHostList() {
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
	void OnPlayerDisconnected(NetworkPlayer player) {
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
	void OnDisconnectedFromServer(NetworkDisconnection info) {
		if(Network.isClient) {
			Network.RemoveRPCsInGroup(0);
		}
	}
}