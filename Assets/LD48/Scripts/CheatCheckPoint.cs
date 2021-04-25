using System;
using UnityEngine;

namespace Assets.LD48.Scripts
{
    public class CheatCheckPoint : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(this.transform.position, 0.1f);
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.transform.parent.position, this.transform.position);
        }
    }
}