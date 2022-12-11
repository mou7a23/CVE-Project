using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace WasaaMP {
	public class CursorTool : MonoBehaviourPun {
		private bool caught ;
		public Interactive interactiveObjectToInstanciate ;
		private Interactive target ;
		private MonoBehaviourPun targetParent ;
		private Transform oldParent = null ;
		void Start () {
			caught = false ;
		}
		
		public void Catch () {
			print ("Catch ?") ;
			if (target != null) {
				print ("Catch :") ;
				if ((! caught) && (transform != target.transform)) { // pour ne pas prendre 2 fois l'objet et lui faire perdre son parent
					oldParent = target.transform.parent ;
					target.transform.SetParent (transform) ;
					target.photonView.TransferOwnership (PhotonNetwork.LocalPlayer) ;
					target.photonView.RPC ("ShowCaught", RpcTarget.All) ;
					PhotonNetwork.SendAllOutgoingCommands () ;
					caught = true ;
				}
				print ("Catch !") ;
			} else {
				print ("Catch failed") ;
			}
		}

		public void Release () {
			if (caught) {
				print ("Release :") ;
				target.transform.SetParent (oldParent) ;
				target.photonView.RPC ("ShowReleased", RpcTarget.All) ;
				PhotonNetwork.SendAllOutgoingCommands () ;
				print ("Release !") ;
				caught = false ;
			}
		}

		public void CreateInteractiveCube () {
			var objectToInstanciate = PhotonNetwork.Instantiate (interactiveObjectToInstanciate.name, transform.position, transform.rotation, 0) ;
		}

		void OnTriggerEnter (Collider other) {
			if (! caught) {
				print (name + " : CursorTool OnTriggerEnter") ;
				target = other.gameObject.GetComponent<Interactive> () ;
				if (target != null) {
					target.photonView.RPC ("ShowCatchable", RpcTarget.All) ;
					PhotonNetwork.SendAllOutgoingCommands () ;
				}
			}
		}

		void OnTriggerExit (Collider other) {
			if (! caught) {
				print (name + " : CursorTool OnTriggerExit") ;
				if (target != null) {
					target.photonView.RPC ("HideCatchable", RpcTarget.All) ;
					PhotonNetwork.SendAllOutgoingCommands () ;
					target = null ;
				}
			}
		}

	}

}