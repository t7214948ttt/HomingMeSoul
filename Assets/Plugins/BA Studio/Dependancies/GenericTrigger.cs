using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BA_Studio.UnityLib.General
{
	[RequireComponent(typeof(Collider))]
	public class GenericTrigger : MonoBehaviour
	{
		public GameObjectFilter filter;

		public Collider triggerZone;

		public CommonEvents.ColliderEvent triggerEnter, triggerStay, triggerExit;

		void Awake ()
		{
			if (triggerZone == null) triggerZone = this.GetComponent<Collider>();
		}

		private void OnTriggerEnter (Collider other)
		{
//			Debug.Log("other.tag:" + other.tag);
//			Debug.Log("tagFilter.Contains(other.tag): " +tagFilter.Contains(other.tag));
			if (!this.enabled) return;
			if (filter.Match(other.gameObject)) triggerEnter?.Invoke(other);
		}

		private void OnTriggerStay(Collider other)
		{
			if (!this.enabled) return;
			if (filter.Match(other.gameObject)) triggerStay?.Invoke(other);
		}
		
		private void OnTriggerExit(Collider other)
		{
			
			if (!this.enabled) return;
			if (filter.Match(other.gameObject)) triggerExit?.Invoke(other);
		}
	}

}
