using System.Collections.Generic;
using DogaRun.Configuration;
using UnityEngine;

namespace DogaRun.Gameplay.Runner
{
    /// <summary>
    /// Mobile-friendly, original placeholder rig for Doğa. The hierarchy deliberately
    /// mirrors a small humanoid rig so a production prefab can replace it later.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ProceduralDogaCharacter : MonoBehaviour
    {
        private const float ReferenceHeight = 1.72f;

        private readonly List<Renderer> characterRenderers = new List<Renderer>(40);
        private readonly List<Material> runtimeMaterials = new List<Material>(12);
        private Shader runtimeShader;

        public Transform BodyRoot { get; private set; }
        public Transform HeadRoot { get; private set; }
        public Transform LeftArmRoot { get; private set; }
        public Transform RightArmRoot { get; private set; }
        public Transform LeftLegRoot { get; private set; }
        public Transform RightLegRoot { get; private set; }
        public int PartCount => characterRenderers.Count;
        public bool IsBuilt { get; private set; }

        public void Build(CharacterDefinition definition = null)
        {
            if (IsBuilt) return;

            var skin = CreateMaterial("Doga_Skin", definition == null ? new Color(1f, 0.76f, 0.58f) : definition.SkinTone, 0.28f);
            var hair = CreateMaterial("Doga_Hair", definition == null ? new Color(1f, 0.76f, 0.2f) : definition.HairColor, 0.2f);
            var eyes = CreateMaterial("Doga_Eyes", definition == null ? new Color(0.12f, 0.46f, 0.9f) : definition.EyeColor, 0.5f);
            var shirt = CreateMaterial("Doga_Shirt", definition == null ? new Color(0.08f, 0.68f, 0.72f) : definition.OutfitPrimary, 0.18f);
            var accent = CreateMaterial("Doga_Accent", definition == null ? new Color(0.95f, 0.35f, 0.27f) : definition.OutfitAccent, 0.2f);
            var shoes = CreateMaterial("Doga_Shoes", definition == null ? new Color(1f, 0.72f, 0.16f) : definition.ShoeColor, 0.3f);
            var white = CreateMaterial("Doga_White", new Color(0.97f, 0.98f, 0.94f), 0.35f);
            var dark = CreateMaterial("Doga_Dark", new Color(0.055f, 0.075f, 0.09f), 0.45f);
            var blush = CreateMaterial("Doga_Blush", new Color(1f, 0.42f, 0.46f), 0.22f);
            var leaf = CreateMaterial("Doga_Leaf", new Color(0.38f, 0.72f, 0.27f), 0.18f);

            var rigRoot = CreateJoint("DogaRig", transform, Vector3.zero);
            var height = definition == null ? 1.65f : definition.Height;
            rigRoot.localScale = Vector3.one * Mathf.Max(0.5f, height) / ReferenceHeight;

            BodyRoot = CreateJoint("Body", rigRoot, new Vector3(0f, 0.79f, 0f));
            CreatePart("Turkuaz Spor Üst", PrimitiveType.Sphere, BodyRoot, new Vector3(0f, 0.22f, 0f), new Vector3(0.62f, 0.68f, 0.44f), Vector3.zero, shirt);
            CreatePart("Mercan Şort", PrimitiveType.Sphere, BodyRoot, new Vector3(0f, -0.05f, 0f), new Vector3(0.57f, 0.25f, 0.42f), Vector3.zero, accent);
            CreatePart("Sarı Bel Şeridi", PrimitiveType.Cylinder, BodyRoot, new Vector3(0f, 0.02f, 0f), new Vector3(0.3f, 0.035f, 0.22f), Vector3.zero, shoes);

            // The small leaf badge is visible from the third-person camera and gives Doğa an original silhouette.
            CreatePart("Yaprak Rozeti", PrimitiveType.Sphere, BodyRoot, new Vector3(0f, 0.25f, -0.225f), new Vector3(0.15f, 0.25f, 0.035f), new Vector3(0f, 0f, -28f), leaf);
            CreatePart("Yaprak Damarı", PrimitiveType.Cube, BodyRoot, new Vector3(0f, 0.25f, -0.248f), new Vector3(0.025f, 0.18f, 0.018f), new Vector3(0f, 0f, -28f), white);

            HeadRoot = CreateJoint("Head", rigRoot, new Vector3(0f, 1.18f, 0f));
            CreatePart("Kafa", PrimitiveType.Sphere, HeadRoot, new Vector3(0f, 0.28f, 0f), new Vector3(0.61f, 0.67f, 0.59f), Vector3.zero, skin);
            CreatePart("Sol Kulak", PrimitiveType.Sphere, HeadRoot, new Vector3(-0.31f, 0.29f, 0f), new Vector3(0.12f, 0.17f, 0.09f), Vector3.zero, skin);
            CreatePart("Sağ Kulak", PrimitiveType.Sphere, HeadRoot, new Vector3(0.31f, 0.29f, 0f), new Vector3(0.12f, 0.17f, 0.09f), Vector3.zero, skin);

            BuildFace(HeadRoot, skin, eyes, white, dark, blush);
            BuildHair(HeadRoot, hair);

            LeftArmRoot = BuildArm("Sol Kol", BodyRoot, -1f, skin, shirt);
            RightArmRoot = BuildArm("Sağ Kol", BodyRoot, 1f, skin, shirt);
            LeftLegRoot = BuildLeg("Sol Bacak", rigRoot, -1f, skin, white, shoes, accent);
            RightLegRoot = BuildLeg("Sağ Bacak", rigRoot, 1f, skin, white, shoes, accent);

            ResetPose();
            IsBuilt = true;
        }

        public void ResetPose()
        {
            if (BodyRoot != null) BodyRoot.localRotation = Quaternion.identity;
            if (HeadRoot != null) HeadRoot.localRotation = Quaternion.identity;
            if (LeftArmRoot != null) LeftArmRoot.localRotation = Quaternion.Euler(0f, 0f, -8f);
            if (RightArmRoot != null) RightArmRoot.localRotation = Quaternion.Euler(0f, 0f, 8f);
            if (LeftLegRoot != null) LeftLegRoot.localRotation = Quaternion.identity;
            if (RightLegRoot != null) RightLegRoot.localRotation = Quaternion.identity;
            SetRenderersEnabled(true);
        }

        public void SetRenderersEnabled(bool enabled)
        {
            for (var index = 0; index < characterRenderers.Count; index++)
            {
                if (characterRenderers[index] != null) characterRenderers[index].enabled = enabled;
            }
        }

        private void BuildFace(Transform head, Material skin, Material eyes, Material white, Material dark, Material blush)
        {
            CreatePart("Sol Göz Beyazı", PrimitiveType.Sphere, head, new Vector3(-0.135f, 0.32f, 0.285f), new Vector3(0.145f, 0.16f, 0.075f), Vector3.zero, white);
            CreatePart("Sağ Göz Beyazı", PrimitiveType.Sphere, head, new Vector3(0.135f, 0.32f, 0.285f), new Vector3(0.145f, 0.16f, 0.075f), Vector3.zero, white);
            CreatePart("Sol Mavi İris", PrimitiveType.Sphere, head, new Vector3(-0.135f, 0.32f, 0.334f), new Vector3(0.082f, 0.096f, 0.038f), Vector3.zero, eyes);
            CreatePart("Sağ Mavi İris", PrimitiveType.Sphere, head, new Vector3(0.135f, 0.32f, 0.334f), new Vector3(0.082f, 0.096f, 0.038f), Vector3.zero, eyes);
            CreatePart("Sol Göz Bebeği", PrimitiveType.Sphere, head, new Vector3(-0.135f, 0.32f, 0.357f), new Vector3(0.036f, 0.05f, 0.02f), Vector3.zero, dark);
            CreatePart("Sağ Göz Bebeği", PrimitiveType.Sphere, head, new Vector3(0.135f, 0.32f, 0.357f), new Vector3(0.036f, 0.05f, 0.02f), Vector3.zero, dark);
            CreatePart("Burun", PrimitiveType.Sphere, head, new Vector3(0f, 0.22f, 0.325f), new Vector3(0.075f, 0.07f, 0.065f), Vector3.zero, skin);
            CreatePart("Gülümseme", PrimitiveType.Sphere, head, new Vector3(0f, 0.11f, 0.325f), new Vector3(0.13f, 0.035f, 0.027f), Vector3.zero, blush);
            CreatePart("Sol Yanak", PrimitiveType.Sphere, head, new Vector3(-0.225f, 0.18f, 0.285f), new Vector3(0.095f, 0.055f, 0.025f), Vector3.zero, blush);
            CreatePart("Sağ Yanak", PrimitiveType.Sphere, head, new Vector3(0.225f, 0.18f, 0.285f), new Vector3(0.095f, 0.055f, 0.025f), Vector3.zero, blush);
        }

        private void BuildHair(Transform head, Material hair)
        {
            CreatePart("Sarı Saç Başlığı", PrimitiveType.Sphere, head, new Vector3(0f, 0.48f, -0.015f), new Vector3(0.66f, 0.48f, 0.62f), Vector3.zero, hair);
            CreatePart("Sol Perçem", PrimitiveType.Sphere, head, new Vector3(-0.16f, 0.47f, 0.275f), new Vector3(0.25f, 0.23f, 0.16f), new Vector3(0f, 0f, -18f), hair);
            CreatePart("Orta Perçem", PrimitiveType.Sphere, head, new Vector3(0f, 0.5f, 0.3f), new Vector3(0.22f, 0.2f, 0.14f), Vector3.zero, hair);
            CreatePart("Sağ Perçem", PrimitiveType.Sphere, head, new Vector3(0.16f, 0.47f, 0.275f), new Vector3(0.25f, 0.23f, 0.16f), new Vector3(0f, 0f, 18f), hair);

            for (var side = -1; side <= 1; side += 2)
            {
                CreatePart(side < 0 ? "Sol Saç Buklesi Üst" : "Sağ Saç Buklesi Üst", PrimitiveType.Sphere, head,
                    new Vector3(0.29f * side, 0.31f, -0.03f), new Vector3(0.22f, 0.24f, 0.22f), Vector3.zero, hair);
                CreatePart(side < 0 ? "Sol Saç Buklesi Alt" : "Sağ Saç Buklesi Alt", PrimitiveType.Sphere, head,
                    new Vector3(0.3f * side, 0.12f, -0.055f), new Vector3(0.18f, 0.21f, 0.18f), Vector3.zero, hair);
            }
        }

        private Transform BuildArm(string name, Transform parent, float side, Material skin, Material shirt)
        {
            var joint = CreateJoint(name, parent, new Vector3(0.34f * side, 0.38f, 0f));
            CreatePart(name + " Kısa Kol", PrimitiveType.Sphere, joint, new Vector3(0.015f * side, -0.07f, 0f), new Vector3(0.22f, 0.25f, 0.23f), Vector3.zero, shirt);
            CreatePart(name + " Kol", PrimitiveType.Capsule, joint, new Vector3(0f, -0.3f, 0f), new Vector3(0.13f, 0.28f, 0.13f), Vector3.zero, skin);
            CreatePart(name + " El", PrimitiveType.Sphere, joint, new Vector3(0f, -0.59f, 0f), new Vector3(0.17f, 0.18f, 0.16f), Vector3.zero, skin);
            return joint;
        }

        private Transform BuildLeg(string name, Transform parent, float side, Material skin, Material socks, Material shoes, Material shorts)
        {
            var joint = CreateJoint(name, parent, new Vector3(0.18f * side, 0.71f, 0f));
            CreatePart(name + " Şort Paçası", PrimitiveType.Sphere, joint, new Vector3(0f, -0.05f, 0f), new Vector3(0.31f, 0.27f, 0.31f), Vector3.zero, shorts);
            CreatePart(name + " Bacak", PrimitiveType.Capsule, joint, new Vector3(0f, -0.31f, 0f), new Vector3(0.16f, 0.31f, 0.17f), Vector3.zero, skin);
            CreatePart(name + " Çorap", PrimitiveType.Cylinder, joint, new Vector3(0f, -0.54f, 0f), new Vector3(0.17f, 0.11f, 0.18f), Vector3.zero, socks);
            CreatePart(name + " Ayakkabı", PrimitiveType.Sphere, joint, new Vector3(0f, -0.66f, 0.09f), new Vector3(0.25f, 0.17f, 0.39f), Vector3.zero, shoes);
            CreatePart(name + " Ayakkabı Tabanı", PrimitiveType.Cube, joint, new Vector3(0f, -0.735f, 0.1f), new Vector3(0.24f, 0.035f, 0.38f), Vector3.zero, socks);
            return joint;
        }

        private Material CreateMaterial(string materialName, Color color, float smoothness)
        {
            if (runtimeShader == null) runtimeShader = ResolveRuntimeShader();
            var material = new Material(runtimeShader)
            {
                name = materialName,
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
            var shader = Shader.Find("Universal Render Pipeline/Lit") ??
                         Shader.Find("Universal Render Pipeline/Simple Lit") ??
                         Shader.Find("Standard");
            if (shader != null) return shader;

            // Shader.Find can return null after player shader stripping. Unity's primitive
            // default material is always part of the player, so its shader is a safe fallback.
            var probe = GameObject.CreatePrimitive(PrimitiveType.Cube);
            probe.name = "DogaMaterialShaderProbe";
            probe.SetActive(false);
            var probeRenderer = probe.GetComponent<Renderer>();
            shader = probeRenderer == null || probeRenderer.sharedMaterial == null
                ? null
                : probeRenderer.sharedMaterial.shader;
            Destroy(probe);
            return shader;
        }

        private static Transform CreateJoint(string name, Transform parent, Vector3 localPosition)
        {
            var joint = new GameObject(name).transform;
            joint.SetParent(parent, false);
            joint.localPosition = localPosition;
            return joint;
        }

        private GameObject CreatePart(
            string name,
            PrimitiveType primitiveType,
            Transform parent,
            Vector3 localPosition,
            Vector3 localScale,
            Vector3 localEulerAngles,
            Material material)
        {
            var part = GameObject.CreatePrimitive(primitiveType);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localScale = localScale;
            part.transform.localRotation = Quaternion.Euler(localEulerAngles);

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
            characterRenderers.Add(renderer);
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
