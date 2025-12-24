using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace OutwardGameSettings.Utility.Helpers.Generic
{
    public static class LocationHelpers
    {
        public static Vector3 GetRandomPositionAround(Vector3 origin, float minRadius, float maxRadius)
        {
            // Pick a random direction on the horizontal plane
            Vector2 randomDir2D = UnityEngine.Random.insideUnitCircle.normalized;

            // Random distance between min and max
            float distance = UnityEngine.Random.Range(minRadius, maxRadius);

            // Convert to 3D (keep Y same as origin for now)
            Vector3 offset = new Vector3(randomDir2D.x, 0, randomDir2D.y) * distance;

            return origin + offset;
        }

        public static Vector3 GetGroundPosition(Vector3 position)
        {
            RaycastHit hit;

            // Cast from above downwards to find the terrain
            if (Physics.Raycast(position + Vector3.up * 50f, Vector3.down, out hit, 100f, LayerMask.GetMask("Default")))
            {
                return hit.point;
            }

            // Fallback: return original if no ground found
            return position;
        }

        public static Vector3 GetRandomReachablePosition(Vector3 playerPos, float minRadius, float maxRadius)
        {
            for (int i = 0; i < 20; i++) // Try up to 20 random points
            {
                Vector3 candidate = GetRandomPositionAround(playerPos, minRadius, maxRadius);
                Vector3 groundPos = GetGroundPosition(candidate);

                // Optional: test if reachable or not inside wall
                if (IsReachable(playerPos, groundPos))
                    return groundPos;
            }

            // If all fail, just return player pos
            return playerPos;
        }

        public static bool IsReachable(Vector3 from, Vector3 to)
        {
            NavMeshPath path = new NavMeshPath();
            if (NavMesh.CalculatePath(from, to, NavMesh.AllAreas, path))
            {
                return path.status == NavMeshPathStatus.PathComplete;
            }
            return false;
        }

        public static bool IsReachableRaycast(Vector3 from, Vector3 to)
        {
            return !Physics.Linecast(from + Vector3.up * 1f, to + Vector3.up * 1f);
        }

        public static Vector3 GetRandomNavMeshPosition(Vector3 origin, float minRadius, float maxRadius, int attempts = 30)
        {
            try
            {
                NavMeshHit hit;

                for (int i = 0; i < attempts; i++)
                {
                    // 1. Random direction and distance
                    Vector2 dir2D = UnityEngine.Random.insideUnitCircle.normalized;
                    float distance = UnityEngine.Random.Range(minRadius, maxRadius);
                    Vector3 candidate = origin + new Vector3(dir2D.x * distance, 0f, dir2D.y * distance);

                    // 2. Snap to nearest NavMesh within small radius
                    if (!NavMesh.SamplePosition(candidate, out hit, 1.5f, NavMesh.AllAreas))
                        continue;

                    // 3. Check actual distance after snapping
                    float actualDistance = Vector3.Distance(origin, hit.position);
                    if (actualDistance < minRadius || actualDistance > maxRadius)
                        continue;

                    // 4. Optional: ensure reachability
                    if (IsReachable(origin, hit.position) && IsWithinMapBounds(origin, hit.position))
                    {
                        OutwardGameSettings.LogMessage($"[Spawn] Valid point at {hit.position}, distance {actualDistance:0.0}");
                        return hit.position;
                    }
                }

                // Fallback
                OutwardGameSettings.LogMessage("[Spawn] No valid NavMesh point found — returning origin.");
                return origin;
            }
            catch(Exception e) {
                OutwardGameSettings.LogMessage($"[Spawn] Error: {e.Message}");
                return origin;
            }
        }

        public static bool HitIsMapBound(RaycastHit hit)
        {
            Transform t = hit.collider.transform;
            while (t != null)
            {
                if (t.name.Equals("MapBounds", StringComparison.OrdinalIgnoreCase))
                    return true;
                t = t.parent;
            }
            return false;
        }

        public static bool IsWithinMapBounds(Vector3 origin, Vector3 target)
        {
            Vector3 dir = target - origin;
            float distance = dir.magnitude;

            if (Physics.Raycast(origin + Vector3.up, dir.normalized, out RaycastHit hit, distance))
            {
                if (HitIsMapBound(hit))
                {
                    OutwardGameSettings.LogMessage($"[Spawn] Blocked by MapBounds: {hit.collider.name}");
                    return false;
                }
            }

            return true;
        }
    }
}
