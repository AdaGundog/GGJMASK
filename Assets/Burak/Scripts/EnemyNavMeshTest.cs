using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Dusman NavMeshAgent ayarlarini kontrol eden script
/// Herhangi bir dusman objesine ekle ve Play'e bas
/// </summary>
public class EnemyNavMeshTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== DUSMAN NAVMESH TESTI ===");
        
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        
        if (agent == null)
        {
            Debug.LogError($"❌ {gameObject.name} uzerinde NavMeshAgent YOK!");
            return;
        }

        Debug.Log($"✅ {gameObject.name} NavMeshAgent var");
        Debug.Log($"   Agent Type ID: {agent.agentTypeID}");
        Debug.Log($"   Area Mask: {agent.areaMask}");
        Debug.Log($"   Obstacle Avoidance: {agent.obstacleAvoidanceType}");
        Debug.Log($"   Auto Braking: {agent.autoBraking}");
        Debug.Log($"   Auto Repath: {agent.autoRepath}");
        Debug.Log($"   Is On NavMesh: {agent.isOnNavMesh}");
        
        if (!agent.isOnNavMesh)
        {
            Debug.LogError("❌ KRITIK: Dusman NavMesh uzerinde DEGIL!");
            Debug.LogError("   Cozum: Dusmani NavMesh'in uzerinde bir yere koy!");
        }

        // NavMesh path testi
        if (agent.isOnNavMesh && agent.hasPath)
        {
            Debug.Log($"✅ Dusman bir hedefe gidiyor");
            Debug.Log($"   Path Status: {agent.pathStatus}");
            Debug.Log($"   Remaining Distance: {agent.remainingDistance}");
        }
    }

    void Update()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null && agent.isOnNavMesh)
        {
            // Her 2 saniyede bir path durumunu kontrol et
            if (Time.frameCount % 120 == 0)
            {
                if (agent.pathStatus == NavMeshPathStatus.PathPartial)
                {
                    Debug.LogWarning($"⚠️ {gameObject.name} kısmi path buldu (engel var olabilir)");
                }
                else if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
                {
                    Debug.LogError($"❌ {gameObject.name} gecersiz path! NavMesh guncel degil olabilir.");
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null && agent.hasPath)
        {
            // Path'i gizmos ile ciz
            Gizmos.color = Color.red;
            Vector3[] corners = agent.path.corners;
            for (int i = 0; i < corners.Length - 1; i++)
            {
                Gizmos.DrawLine(corners[i], corners[i + 1]);
            }
        }
    }
}
