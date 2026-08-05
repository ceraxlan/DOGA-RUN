using System.Collections.Generic;
using UnityEngine;

namespace DogaRun.Gameplay.World
{
    /// <summary>
    /// Builds an original, mobile-friendly sunlit forest chunk. Visual rhythm is inspired
    /// by portrait runners, while the trail, foliage language and landmarks are DogaRun-specific.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ProceduralSunlitForestFactory : MonoBehaviour
    {
        private readonly List<Material> runtimeMaterials = new List<Material>(20);
        private readonly List<Transform> animatedFoliage = new List<Transform>(20);

        private Shader runtimeShader;
        private Material trailBase;
        private Material trailLight;
        private Material trailWarm;
        private Material trailShade;
        private Material laneMarker;
        private Material grassLight;
        private Material grassDark;
        private Material trunk;
        private Material leafLight;
        private Material leafMid;
        private Material leafDark;
        private Material stone;
        private Material flowerYellow;
        private Material flowerCoral;
        private Material flowerPurple;
        private Material mushroom;

        public int LastVisualPartCount { get; private set; }

        public WorldChunk CreateTemplate(Transform parent, float chunkLength = 24f)
        {
            EnsurePalette();
            LastVisualPartCount = 0;
            animatedFoliage.Clear();

            var root = new GameObject("SunlitForestChunk_Template");
            root.transform.SetParent(parent, false);
            var chunk = root.AddComponent<WorldChunk>();
            chunk.Configure(chunkLength);

            BuildTrail(root.transform, chunkLength);
            BuildTreeRows(root.transform, chunkLength);
            BuildGroundDetails(root.transform, chunkLength);
            BuildNaturalArch(root.transform, chunkLength - 2.5f);

            var foliageAnimator = root.AddComponent<ForestFoliageAnimator>();
            foliageAnimator.Configure(animatedFoliage.ToArray());
            return chunk;
        }

        public void CreateAmbientLeaves(Transform parent)
        {
            EnsurePalette();
            var ambientRoot = new GameObject("SunlitForest_AmbientLeaves");
            ambientRoot.transform.SetParent(parent, false);

            const int leafCount = 12;
            var leaves = new Transform[leafCount];
            for (var index = 0; index < leafCount; index++)
            {
                var material = index % 4 == 0 ? flowerYellow : leafLight;
                var leaf = CreatePart(
                    $"AmbientLeaf_{index:00}",
                    PrimitiveType.Sphere,
                    ambientRoot.transform,
                    Vector3.zero,
                    new Vector3(0.11f, 0.025f, 0.2f),
                    Vector3.zero,
                    material);
                leaves[index] = leaf.transform;
            }

            ambientRoot.AddComponent<ForestLeafDrift>().Configure(leaves);
        }

        private void BuildTrail(Transform root, float length)
        {
            var trail = CreateGroup("WarmForestTrail", root, Vector3.zero);
            CreatePart("Toprak Yol Tabanı", PrimitiveType.Cube, trail, new Vector3(0f, -0.16f, length * 0.5f), new Vector3(8.4f, 0.3f, length), Vector3.zero, trailBase);
            CreatePart("Sol Çim Bankı", PrimitiveType.Cube, trail, new Vector3(-6.25f, -0.04f, length * 0.5f), new Vector3(4.2f, 0.45f, length), new Vector3(0f, 0f, -3.5f), grassLight);
            CreatePart("Sağ Çim Bankı", PrimitiveType.Cube, trail, new Vector3(6.25f, -0.04f, length * 0.5f), new Vector3(4.2f, 0.45f, length), new Vector3(0f, 0f, 3.5f), grassLight);

            var lanePositions = new[] { -2.4f, 0f, 2.4f };
            var laneMaterials = new[] { trailShade, trailLight, trailWarm };
            var laneNames = new[] { "Sol Patika", "Orta Patika", "Sağ Patika" };
            for (var lane = 0; lane < lanePositions.Length; lane++)
                CreatePart(laneNames[lane], PrimitiveType.Cube, trail, new Vector3(lanePositions[lane], 0.015f, length * 0.5f), new Vector3(2.16f, 0.045f, length - 0.18f), Vector3.zero, laneMaterials[lane]);

            CreatePart("Sol Şerit Yaprak Bordürü", PrimitiveType.Cube, trail, new Vector3(-1.2f, 0.055f, length * 0.5f), new Vector3(0.075f, 0.035f, length), Vector3.zero, laneMarker);
            CreatePart("Sağ Şerit Yaprak Bordürü", PrimitiveType.Cube, trail, new Vector3(1.2f, 0.055f, length * 0.5f), new Vector3(0.075f, 0.035f, length), Vector3.zero, laneMarker);

            for (var row = 0; row < 4; row++)
            {
                var z = 2.4f + row * 5.6f;
                for (var lane = 0; lane < lanePositions.Length; lane++)
                {
                    var material = (row + lane) % 2 == 0 ? trailWarm : trailShade;
                    CreatePart($"Patika Taşı {row}_{lane}", PrimitiveType.Cube, trail,
                        new Vector3(lanePositions[lane], 0.07f, z), new Vector3(1.65f, 0.045f, 0.18f), Vector3.zero, material);
                }
            }
        }

        private void BuildTreeRows(Transform root, float length)
        {
            var forest = CreateGroup("LayeredForest", root, Vector3.zero);
            for (var index = 0; index < 3; index++)
            {
                var z = 3.2f + index * 7.2f;
                CreateTree(forest, $"Sol Ön Ağaç {index}", new Vector3(-5.25f - index % 2 * 0.55f, 0f, z), 0.95f + index * 0.08f, index % 2 == 0);
                CreateTree(forest, $"Sağ Ön Ağaç {index}", new Vector3(5.25f + index % 2 * 0.55f, 0f, z + 2.3f), 1.02f + index * 0.06f, index % 2 != 0);
            }

            CreateBackgroundTree(forest, "Sol Arka Ağaç A", new Vector3(-8.4f, 0f, 2f), 1.28f);
            CreateBackgroundTree(forest, "Sol Arka Ağaç B", new Vector3(-8.8f, 0f, length - 5f), 1.4f);
            CreateBackgroundTree(forest, "Sağ Arka Ağaç A", new Vector3(8.1f, 0f, 7f), 1.34f);
            CreateBackgroundTree(forest, "Sağ Arka Ağaç B", new Vector3(8.7f, 0f, length - 1f), 1.22f);
        }

        private void BuildGroundDetails(Transform root, float length)
        {
            var details = CreateGroup("ForestFloorDetails", root, Vector3.zero);
            for (var index = 0; index < 2; index++)
            {
                var z = 4.8f + index * 11f;
                CreateFern(details, $"Sol Eğrelti {index}", new Vector3(-4.25f, 0.05f, z), -12f + index * 9f);
                CreateFern(details, $"Sağ Eğrelti {index}", new Vector3(4.35f, 0.05f, z + 3.2f), 10f - index * 8f);
            }

            for (var index = 0; index < 4; index++)
            {
                var side = index % 2 == 0 ? -1f : 1f;
                var z = 2f + index * 5.4f;
                CreateFlower(details, $"Orman Çiçeği {index}", new Vector3(side * (4.15f + index * 0.12f), 0.05f, z), index);
                CreatePart($"Yumuşak Kaya {index}", PrimitiveType.Sphere, details,
                    new Vector3(-side * 4.55f, 0.13f, z + 2.1f), new Vector3(0.48f, 0.28f, 0.42f), new Vector3(0f, index * 17f, 0f), stone);
            }

            CreateMushroom(details, "Sol Mantar", new Vector3(-4.55f, 0.04f, length * 0.48f));
            CreateMushroom(details, "Sağ Mantar", new Vector3(4.65f, 0.04f, length * 0.72f));
        }

        private void BuildNaturalArch(Transform root, float z)
        {
            var arch = CreateGroup("LeafCanopyArch", root, new Vector3(0f, 0f, z));
            CreatePart("Sol Kemer Dalı", PrimitiveType.Cylinder, arch, new Vector3(-3.65f, 3.65f, 0f), new Vector3(0.22f, 2.65f, 0.22f), new Vector3(0f, 0f, -54f), trunk);
            CreatePart("Sağ Kemer Dalı", PrimitiveType.Cylinder, arch, new Vector3(3.65f, 3.65f, 0f), new Vector3(0.22f, 2.65f, 0.22f), new Vector3(0f, 0f, 54f), trunk);
            var canopy = CreateGroup("Kemer Yaprakları", arch, new Vector3(0f, 6.1f, 0f));
            CreatePart("Sol Kemer Tacı", PrimitiveType.Sphere, canopy, new Vector3(-1.8f, 0f, 0f), new Vector3(3.7f, 1.8f, 2.1f), Vector3.zero, leafMid);
            CreatePart("Sağ Kemer Tacı", PrimitiveType.Sphere, canopy, new Vector3(1.8f, 0.15f, 0.1f), new Vector3(3.7f, 1.85f, 2.2f), Vector3.zero, leafLight);
            animatedFoliage.Add(canopy);
        }

        private void CreateTree(Transform parent, string name, Vector3 position, float scale, bool lightCrown)
        {
            var tree = CreateGroup(name, parent, position);
            CreatePart("Gövde", PrimitiveType.Cylinder, tree, new Vector3(0f, 1.55f * scale, 0f), new Vector3(0.42f * scale, 1.55f * scale, 0.42f * scale), Vector3.zero, trunk);
            CreatePart("İç Dal", PrimitiveType.Cylinder, tree, new Vector3(0.45f, 2.45f * scale, 0f), new Vector3(0.14f * scale, 0.75f * scale, 0.14f * scale), new Vector3(0f, 0f, -48f), trunk);
            var canopy = CreateGroup("Sallanan Taç", tree, new Vector3(0f, 3.65f * scale, 0f));
            CreatePart("Alt Yaprak Kümesi", PrimitiveType.Sphere, canopy, new Vector3(0f, 0f, 0f), new Vector3(2.45f, 1.9f, 2.15f) * scale, Vector3.zero, lightCrown ? leafMid : leafDark);
            CreatePart("Üst Yaprak Kümesi", PrimitiveType.Sphere, canopy, new Vector3(0.35f, 1.05f * scale, 0.08f), new Vector3(1.75f, 1.55f, 1.65f) * scale, Vector3.zero, lightCrown ? leafLight : leafMid);
            animatedFoliage.Add(canopy);
        }

        private void CreateBackgroundTree(Transform parent, string name, Vector3 position, float scale)
        {
            var tree = CreateGroup(name, parent, position);
            CreatePart("Uzak Gövde", PrimitiveType.Cylinder, tree, new Vector3(0f, 1.8f * scale, 0f), new Vector3(0.36f * scale, 1.8f * scale, 0.36f * scale), Vector3.zero, trunk);
            var canopy = CreatePart("Uzak Taç", PrimitiveType.Sphere, tree, new Vector3(0f, 4.4f * scale, 0f), new Vector3(2.6f, 2.45f, 2.3f) * scale, Vector3.zero, leafDark);
            animatedFoliage.Add(canopy.transform);
        }

        private void CreateFern(Transform parent, string name, Vector3 position, float rotation)
        {
            var fern = CreateGroup(name, parent, position);
            fern.localRotation = Quaternion.Euler(0f, rotation, 0f);
            CreatePart("Orta Yaprak", PrimitiveType.Sphere, fern, new Vector3(0f, 0.25f, 0f), new Vector3(0.18f, 0.65f, 0.08f), new Vector3(0f, 0f, -8f), leafLight);
            CreatePart("Sol Yaprak", PrimitiveType.Sphere, fern, new Vector3(-0.22f, 0.22f, 0f), new Vector3(0.16f, 0.58f, 0.07f), new Vector3(0f, 0f, 42f), leafMid);
            CreatePart("Sağ Yaprak", PrimitiveType.Sphere, fern, new Vector3(0.22f, 0.22f, 0f), new Vector3(0.16f, 0.58f, 0.07f), new Vector3(0f, 0f, -42f), leafMid);
            animatedFoliage.Add(fern);
        }

        private void CreateFlower(Transform parent, string name, Vector3 position, int index)
        {
            var flower = CreateGroup(name, parent, position);
            CreatePart("Sap", PrimitiveType.Cylinder, flower, new Vector3(0f, 0.18f, 0f), new Vector3(0.035f, 0.18f, 0.035f), Vector3.zero, grassDark);
            var flowerMaterial = index % 3 == 0 ? flowerYellow : index % 3 == 1 ? flowerCoral : flowerPurple;
            CreatePart("Çiçek", PrimitiveType.Sphere, flower, new Vector3(0f, 0.42f, 0f), new Vector3(0.24f, 0.16f, 0.24f), Vector3.zero, flowerMaterial);
        }

        private void CreateMushroom(Transform parent, string name, Vector3 position)
        {
            var root = CreateGroup(name, parent, position);
            CreatePart("Mantar Sapı", PrimitiveType.Cylinder, root, new Vector3(0f, 0.16f, 0f), new Vector3(0.08f, 0.16f, 0.08f), Vector3.zero, laneMarker);
            CreatePart("Mantar Şapkası", PrimitiveType.Sphere, root, new Vector3(0f, 0.34f, 0f), new Vector3(0.38f, 0.18f, 0.38f), Vector3.zero, mushroom);
        }

        private void EnsurePalette()
        {
            if (trailBase != null) return;
            trailBase = CreateMaterial("Forest_TrailBase", new Color(0.48f, 0.25f, 0.1f), 0.08f);
            trailLight = CreateMaterial("Forest_TrailLight", new Color(0.78f, 0.49f, 0.21f), 0.08f);
            trailWarm = CreateMaterial("Forest_TrailWarm", new Color(0.9f, 0.55f, 0.2f), 0.1f);
            trailShade = CreateMaterial("Forest_TrailShade", new Color(0.66f, 0.36f, 0.14f), 0.08f);
            laneMarker = CreateMaterial("Forest_LaneMarker", new Color(1f, 0.83f, 0.43f), 0.15f);
            grassLight = CreateMaterial("Forest_GrassLight", new Color(0.33f, 0.68f, 0.21f), 0.06f);
            grassDark = CreateMaterial("Forest_GrassDark", new Color(0.12f, 0.38f, 0.16f), 0.05f);
            trunk = CreateMaterial("Forest_Trunk", new Color(0.31f, 0.16f, 0.075f), 0.08f);
            leafLight = CreateMaterial("Forest_LeafLight", new Color(0.48f, 0.78f, 0.23f), 0.06f);
            leafMid = CreateMaterial("Forest_LeafMid", new Color(0.2f, 0.58f, 0.2f), 0.06f);
            leafDark = CreateMaterial("Forest_LeafDark", new Color(0.075f, 0.31f, 0.15f), 0.06f);
            stone = CreateMaterial("Forest_Stone", new Color(0.48f, 0.52f, 0.42f), 0.12f);
            flowerYellow = CreateMaterial("Forest_FlowerYellow", new Color(1f, 0.78f, 0.18f), 0.15f);
            flowerCoral = CreateMaterial("Forest_FlowerCoral", new Color(1f, 0.31f, 0.25f), 0.16f);
            flowerPurple = CreateMaterial("Forest_FlowerPurple", new Color(0.66f, 0.37f, 0.82f), 0.16f);
            mushroom = CreateMaterial("Forest_Mushroom", new Color(0.95f, 0.27f, 0.2f), 0.16f);
        }

        private Material CreateMaterial(string name, Color color, float smoothness)
        {
            if (runtimeShader == null) runtimeShader = ResolveRuntimeShader();
            var material = new Material(runtimeShader)
            {
                name = name,
                color = color,
                enableInstancing = true,
                hideFlags = HideFlags.DontSave
            };
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", smoothness);
            runtimeMaterials.Add(material);
            return material;
        }

        private static Shader ResolveRuntimeShader()
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Universal Render Pipeline/Simple Lit") ?? Shader.Find("Standard");
            if (shader != null) return shader;
            var probe = GameObject.CreatePrimitive(PrimitiveType.Cube);
            probe.SetActive(false);
            var renderer = probe.GetComponent<Renderer>();
            shader = renderer == null || renderer.sharedMaterial == null ? null : renderer.sharedMaterial.shader;
            Destroy(probe);
            return shader;
        }

        private static Transform CreateGroup(string name, Transform parent, Vector3 localPosition)
        {
            var group = new GameObject(name).transform;
            group.SetParent(parent, false);
            group.localPosition = localPosition;
            return group;
        }

        private GameObject CreatePart(string name, PrimitiveType type, Transform parent, Vector3 position, Vector3 scale, Vector3 rotation, Material material)
        {
            var part = GameObject.CreatePrimitive(type);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = position;
            part.transform.localScale = scale;
            part.transform.localRotation = Quaternion.Euler(rotation);
            var collider = part.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
                Destroy(collider);
            }
            var renderer = part.GetComponent<Renderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            renderer.receiveShadows = true;
            LastVisualPartCount++;
            return part;
        }

        private void OnDestroy()
        {
            for (var index = 0; index < runtimeMaterials.Count; index++)
            {
                if (runtimeMaterials[index] != null) Destroy(runtimeMaterials[index]);
            }
            runtimeMaterials.Clear();
        }
    }
}
