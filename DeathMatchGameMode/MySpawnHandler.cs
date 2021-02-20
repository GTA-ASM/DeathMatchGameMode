using System.Collections.Generic;
using System.Linq;
using SanAndreasUnity.Behaviours;
using SanAndreasUnity.Net;
using SanAndreasUnity.Utilities;
using UnityEngine;

namespace DeathMatchGameMode
{
    class MySpawnHandler : SpawnHandler
    {
        Vector3[] spawnLocations = new Vector3[]
        {
            new Vector3(144.7678f, 26.06416f, 1903.865f),
            new Vector3(127.387f, 17.79094f, 1879.024f),
            new Vector3(166.3635f, 17.59562f, 1842.359f),
            new Vector3(139.608f, 24.00266f, 1844.584f),
            new Vector3(105.5336f, 17.78813f, 1872.093f),
            new Vector3(107.2309f, 25.455f, 1897.732f),
            new Vector3(147.3223f, 17.71333f, 1865.13f),
            new Vector3(165.7289f, 33.85344f, 1850.303f),
        };

        public override bool GetSpawnPosition(Player player, out TransformDataStruct transformData)
        {
            // pick a random position from our list
            Vector3 position = spawnLocations.RandomElement();

            // use random rotation around Y axis
            Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            transformData = new TransformDataStruct(position, rotation);

            return true;
        }
    }
}
