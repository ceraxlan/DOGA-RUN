using DogaRun.Configuration;
using DogaRun.Core;
using DogaRun.Gameplay.Collision;
using DogaRun.Gameplay.Input;
using DogaRun.Gameplay.Obstacles;
using DogaRun.Gameplay.Runner;
using DogaRun.Gameplay.Scoring;
using DogaRun.Gameplay.World;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace DogaRun.UI
{
    public sealed class GameSceneInstaller : MonoBehaviour
    {
        private static readonly Color ForestGreen = new Color(0.18f, 0.48f, 0.25f);
        private static readonly Color DeepGreen = new Color(0.08f, 0.28f, 0.15f);
        private static readonly Color DirtBrown = new Color(0.42f, 0.25f, 0.12f);
        private static readonly Color WarmYellow = new Color(1f, 0.76f, 0.2f);
        private static readonly Color SkyBlue = new Color(0.42f, 0.76f, 0.9f);
        private static readonly Color Coral = new Color(0.95f, 0.35f, 0.27f);
        private KeyboardInputReader keyboardInput;
        private GameLoopController gameLoop;

        private void Awake()
        {
            BuildVerticalSlice();
        }

        private void BuildVerticalSlice()
        {
            Application.targetFrameRate = 60;
            Screen.orientation = ScreenOrientation.Portrait;

            var stateMachine = new GameStateMachine(GameState.Running);
            var session = new RunSession();
            var difficulty = DifficultyPresetFactory.CreateRuntime("Normal");
            var hitStateMachine = new HitStateMachine();
            session.Start(difficulty.DifficultyId);

            var systemsRoot = new GameObject("Systems").transform;
            systemsRoot.SetParent(transform, false);

            var runner = CreateRunner(stateMachine, hitStateMachine, systemsRoot, out var characterAnimation);
            CreateCamera(runner.transform, systemsRoot);
            CreateLighting(systemsRoot);
            var world = CreateWorld(difficulty, stateMachine, systemsRoot);
            var obstacles = CreateObstacles(difficulty, stateMachine, hitStateMachine, world, systemsRoot);
            CreateInput(runner, systemsRoot);
            CreateUi(session, hitStateMachine, out var hud, out var gameOver);

            gameLoop = systemsRoot.gameObject.AddComponent<GameLoopController>();
            gameLoop.Initialize(stateMachine, session, difficulty, hitStateMachine, runner, characterAnimation, world, obstacles, new ScoreCalculator(), hud, gameOver);
            keyboardInput.PauseRequested += gameLoop.TogglePause;
        }

        private RunnerController CreateRunner(
            GameStateMachine stateMachine,
            HitStateMachine hitStateMachine,
            Transform parent,
            out CharacterAnimationController animationController)
        {
            var runnerObject = new GameObject("DogaRunner");
            runnerObject.transform.SetParent(parent, false);
            runnerObject.transform.position = Vector3.zero;

            var controller = runnerObject.AddComponent<CharacterController>();
            controller.height = 1.7f;
            controller.radius = 0.34f;
            controller.center = new Vector3(0f, 0.85f, 0f);
            controller.stepOffset = 0.2f;
            controller.skinWidth = 0.04f;

            var visualRoot = new GameObject("DogaVisual_Placeholder").transform;
            visualRoot.SetParent(runnerObject.transform, false);

            var shirt = CreatePrimitive("Turkuaz Spor Üst", PrimitiveType.Capsule, visualRoot, new Vector3(0f, 0.83f, 0f), new Vector3(0.52f, 0.58f, 0.4f), new Color(0.08f, 0.68f, 0.72f));
            shirt.transform.localRotation = Quaternion.identity;
            CreatePrimitive("Kafa", PrimitiveType.Sphere, visualRoot, new Vector3(0f, 1.62f, 0f), Vector3.one * 0.68f, new Color(1f, 0.76f, 0.58f));
            CreatePrimitive("Sarı Saç", PrimitiveType.Sphere, visualRoot, new Vector3(0f, 1.83f, -0.02f), new Vector3(0.72f, 0.38f, 0.7f), WarmYellow);
            CreatePrimitive("Sol Saç Buklesi", PrimitiveType.Sphere, visualRoot, new Vector3(-0.28f, 1.62f, 0f), Vector3.one * 0.22f, WarmYellow);
            CreatePrimitive("Sağ Saç Buklesi", PrimitiveType.Sphere, visualRoot, new Vector3(0.28f, 1.62f, 0f), Vector3.one * 0.22f, WarmYellow);
            CreatePrimitive("Sol Göz", PrimitiveType.Sphere, visualRoot, new Vector3(-0.13f, 1.68f, 0.31f), Vector3.one * 0.09f, new Color(0.12f, 0.46f, 0.9f));
            CreatePrimitive("Sağ Göz", PrimitiveType.Sphere, visualRoot, new Vector3(0.13f, 1.68f, 0.31f), Vector3.one * 0.09f, new Color(0.12f, 0.46f, 0.9f));
            CreatePrimitive("Sol Bacak", PrimitiveType.Capsule, visualRoot, new Vector3(-0.16f, 0.27f, 0f), new Vector3(0.18f, 0.28f, 0.2f), Coral);
            CreatePrimitive("Sağ Bacak", PrimitiveType.Capsule, visualRoot, new Vector3(0.16f, 0.27f, 0f), new Vector3(0.18f, 0.28f, 0.2f), Coral);

            var runner = runnerObject.AddComponent<RunnerController>();
            runner.Initialize(stateMachine);
            runner.Configure(2.4f, 2f, 0.75f);
            animationController = runnerObject.AddComponent<CharacterAnimationController>();
            animationController.Initialize(visualRoot, hitStateMachine);
            return runner;
        }

        private void CreateCamera(Transform runner, Transform parent)
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(parent, false);
            var camera = cameraObject.AddComponent<Camera>();
            camera.fieldOfView = 58f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 160f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = SkyBlue;
            cameraObject.AddComponent<AudioListener>();
            cameraObject.AddComponent<RunnerCameraRig>().Initialize(runner);
        }

        private static void CreateLighting(Transform parent)
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.68f, 0.79f, 0.74f);
            RenderSettings.ambientEquatorColor = new Color(0.44f, 0.57f, 0.42f);
            RenderSettings.ambientGroundColor = new Color(0.2f, 0.25f, 0.18f);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = new Color(0.56f, 0.72f, 0.66f);
            RenderSettings.fogStartDistance = 45f;
            RenderSettings.fogEndDistance = 120f;

            var lightObject = new GameObject("Warm Sun");
            lightObject.transform.SetParent(parent, false);
            lightObject.transform.rotation = Quaternion.Euler(42f, -28f, 0f);
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.9f, 0.72f);
            light.intensity = 1.15f;
            light.shadows = LightShadows.Soft;
        }

        private WorldSequenceController CreateWorld(DifficultyConfig difficulty, GameStateMachine stateMachine, Transform parent)
        {
            var worldRoot = new GameObject("World").transform;
            worldRoot.SetParent(parent, false);
            var pool = worldRoot.gameObject.AddComponent<WorldChunkPool>();
            var template = CreateForestChunkTemplate(worldRoot);
            pool.Initialize(template, 7);
            var sequence = worldRoot.gameObject.AddComponent<WorldSequenceController>();
            sequence.Initialize(pool, difficulty, stateMachine, 6, 8);
            worldRoot.gameObject.AddComponent<EnvironmentTransitionController>();
            return sequence;
        }

        private WorldChunk CreateForestChunkTemplate(Transform parent)
        {
            const float chunkLength = 18f;
            var root = new GameObject("SunlitForestChunk_Template");
            root.transform.SetParent(parent, false);
            var chunk = root.AddComponent<WorldChunk>();
            chunk.Configure(chunkLength);

            var road = GameObject.CreatePrimitive(PrimitiveType.Cube);
            road.name = "Toprak Yol";
            road.transform.SetParent(root.transform, false);
            road.transform.localPosition = new Vector3(0f, -0.16f, chunkLength * 0.5f);
            road.transform.localScale = new Vector3(7.7f, 0.3f, chunkLength);
            SetMaterial(road, DirtBrown);

            CreatePrimitive("Sol Çim", PrimitiveType.Cube, root.transform, new Vector3(-5.9f, -0.21f, chunkLength * 0.5f), new Vector3(4f, 0.22f, chunkLength), ForestGreen);
            CreatePrimitive("Sağ Çim", PrimitiveType.Cube, root.transform, new Vector3(5.9f, -0.21f, chunkLength * 0.5f), new Vector3(4f, 0.22f, chunkLength), ForestGreen);
            CreatePrimitive("Sol Şerit İzi", PrimitiveType.Cube, root.transform, new Vector3(-1.2f, 0.005f, chunkLength * 0.5f), new Vector3(0.05f, 0.02f, chunkLength), new Color(0.65f, 0.47f, 0.24f));
            CreatePrimitive("Sağ Şerit İzi", PrimitiveType.Cube, root.transform, new Vector3(1.2f, 0.005f, chunkLength * 0.5f), new Vector3(0.05f, 0.02f, chunkLength), new Color(0.65f, 0.47f, 0.24f));

            for (var index = 0; index < 3; index++)
            {
                var z = 2.5f + index * 6f;
                CreateTree(root.transform, -5.1f - index % 2, z, 1f + index * 0.06f);
                CreateTree(root.transform, 5.1f + index % 2, z + 2f, 0.94f + index * 0.08f);
                CreatePrimitive($"Çiçek Sol {index}", PrimitiveType.Sphere, root.transform, new Vector3(-4.1f, 0.16f, z + 1f), Vector3.one * 0.22f, index % 2 == 0 ? WarmYellow : Coral);
                CreatePrimitive($"Çiçek Sağ {index}", PrimitiveType.Sphere, root.transform, new Vector3(4.2f, 0.16f, z + 4f), Vector3.one * 0.2f, index % 2 == 0 ? Coral : WarmYellow);
            }
            return chunk;
        }

        private static void CreateTree(Transform parent, float x, float z, float scale)
        {
            CreatePrimitive("Ağaç Gövdesi", PrimitiveType.Cylinder, parent, new Vector3(x, 1.3f * scale, z), new Vector3(0.42f * scale, 1.3f * scale, 0.42f * scale), new Color(0.36f, 0.2f, 0.09f));
            CreatePrimitive("Ağaç Tacı", PrimitiveType.Sphere, parent, new Vector3(x, 3.45f * scale, z), new Vector3(2.1f, 2.3f, 2.1f) * scale, DeepGreen);
        }

        private AnimalObstacleSpawner CreateObstacles(
            DifficultyConfig difficulty,
            GameStateMachine stateMachine,
            HitStateMachine hitStateMachine,
            WorldSequenceController world,
            Transform parent)
        {
            var root = new GameObject("AnimalObstaclePool").transform;
            root.SetParent(parent, false);
            var templateRoot = new GameObject("Sevimli Köpek Placeholder_Template");
            templateRoot.transform.SetParent(root, false);
            var collider = templateRoot.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.center = new Vector3(0f, 0.52f, 0f);
            collider.size = new Vector3(1.15f, 1.05f, 0.85f);
            var body = templateRoot.AddComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            var animal = templateRoot.AddComponent<AnimalObstacle>();

            CreatePrimitive("Gövde", PrimitiveType.Capsule, templateRoot.transform, new Vector3(0f, 0.52f, 0f), new Vector3(0.65f, 0.48f, 0.55f), new Color(0.72f, 0.47f, 0.25f));
            CreatePrimitive("Baş", PrimitiveType.Sphere, templateRoot.transform, new Vector3(0f, 0.9f, 0.25f), Vector3.one * 0.62f, new Color(0.83f, 0.58f, 0.32f));
            CreatePrimitive("Sol Kulak", PrimitiveType.Cube, templateRoot.transform, new Vector3(-0.26f, 1.12f, 0.22f), new Vector3(0.18f, 0.42f, 0.12f), new Color(0.48f, 0.27f, 0.12f));
            CreatePrimitive("Sağ Kulak", PrimitiveType.Cube, templateRoot.transform, new Vector3(0.26f, 1.12f, 0.22f), new Vector3(0.18f, 0.42f, 0.12f), new Color(0.48f, 0.27f, 0.12f));

            var spawner = root.gameObject.AddComponent<AnimalObstacleSpawner>();
            spawner.Initialize(animal, 8, difficulty, stateMachine, hitStateMachine, world, 1207);
            return spawner;
        }

        private void CreateInput(RunnerController runner, Transform parent)
        {
            var inputObject = new GameObject("InputReaders");
            inputObject.transform.SetParent(parent, false);
            keyboardInput = inputObject.AddComponent<KeyboardInputReader>();
            var swipe = inputObject.AddComponent<SwipeInputReader>();
            runner.Bind(keyboardInput, swipe);
        }

        private void CreateUi(RunSession session, HitStateMachine hits, out GameHudPresenter hud, out GameOverPresenter gameOver)
        {
            var canvasObject = new GameObject("GameCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            var safeObject = new GameObject("SafeArea", typeof(RectTransform), typeof(SafeAreaFitter));
            safeObject.transform.SetParent(canvasObject.transform, false);
            Stretch(safeObject.GetComponent<RectTransform>());

            var topBar = CreatePanel("TopBar", safeObject.transform, new Color(0.04f, 0.18f, 0.13f, 0.82f));
            var topRect = topBar.GetComponent<RectTransform>();
            topRect.anchorMin = new Vector2(0.03f, 0.88f);
            topRect.anchorMax = new Vector2(0.97f, 0.98f);
            topRect.offsetMin = Vector2.zero;
            topRect.offsetMax = Vector2.zero;

            var timer = CreateText("Timer", topBar.transform, "00:00", 62f, TextAlignmentOptions.MidlineLeft, Color.white);
            Anchor(timer.rectTransform, new Vector2(0.04f, 0.12f), new Vector2(0.32f, 0.88f));
            var score = CreateText("Score", topBar.transform, "SKOR 000000", 52f, TextAlignmentOptions.Center, Color.white);
            Anchor(score.rectTransform, new Vector2(0.29f, 0.12f), new Vector2(0.71f, 0.88f));
            var hitsText = CreateText("Hits", topBar.transform, "HAK ●●●", 42f, TextAlignmentOptions.MidlineRight, WarmYellow);
            Anchor(hitsText.rectTransform, new Vector2(0.67f, 0.12f), new Vector2(0.94f, 0.88f));

            var pauseButton = CreateButton("PauseButton", safeObject.transform, "II", new Color(0.04f, 0.18f, 0.13f, 0.9f));
            Anchor(pauseButton.GetComponent<RectTransform>(), new Vector2(0.82f, 0.79f), new Vector2(0.97f, 0.87f));
            var debug = CreateText("SpeedDebug", safeObject.transform, "1.00x", 30f, TextAlignmentOptions.Center, new Color(1f, 1f, 1f, 0.55f));
            Anchor(debug.rectTransform, new Vector2(0.82f, 0.755f), new Vector2(0.97f, 0.79f));

            var pausePanel = CreatePanel("PauseOverlay", safeObject.transform, new Color(0.02f, 0.08f, 0.06f, 0.9f));
            Stretch(pausePanel.GetComponent<RectTransform>());
            var pauseTitle = CreateText("PauseTitle", pausePanel.transform, "MOLA", 88f, TextAlignmentOptions.Center, WarmYellow);
            Anchor(pauseTitle.rectTransform, new Vector2(0.15f, 0.58f), new Vector2(0.85f, 0.75f));
            var resumeButton = CreateButton("ResumeButton", pausePanel.transform, "DEVAM ET", ForestGreen);
            Anchor(resumeButton.GetComponent<RectTransform>(), new Vector2(0.18f, 0.44f), new Vector2(0.82f, 0.54f));
            var pauseRestartButton = CreateButton("PauseRestartButton", pausePanel.transform, "YENİDEN BAŞLAT", Coral);
            Anchor(pauseRestartButton.GetComponent<RectTransform>(), new Vector2(0.18f, 0.31f), new Vector2(0.82f, 0.41f));

            var gameOverPanel = CreatePanel("GameOverOverlay", safeObject.transform, new Color(0.02f, 0.07f, 0.06f, 0.92f));
            Stretch(gameOverPanel.GetComponent<RectTransform>());
            var title = CreateText("GameOverTitle", gameOverPanel.transform, "DOĞA KOŞUSU", 86f, TextAlignmentOptions.Center, WarmYellow);
            Anchor(title.rectTransform, new Vector2(0.08f, 0.73f), new Vector2(0.92f, 0.86f));
            var subtitle = CreateText("GameOverSubtitle", gameOverPanel.transform, "HARİKA KOŞU!", 48f, TextAlignmentOptions.Center, Color.white);
            Anchor(subtitle.rectTransform, new Vector2(0.12f, 0.66f), new Vector2(0.88f, 0.73f));
            var summary = CreateText("Summary", gameOverPanel.transform, string.Empty, 48f, TextAlignmentOptions.Center, Color.white);
            summary.lineSpacing = 16f;
            Anchor(summary.rectTransform, new Vector2(0.12f, 0.38f), new Vector2(0.88f, 0.64f));
            var restartButton = CreateButton("RestartButton", gameOverPanel.transform, "TEKRAR KOŞ", ForestGreen);
            Anchor(restartButton.GetComponent<RectTransform>(), new Vector2(0.16f, 0.24f), new Vector2(0.84f, 0.34f));
            var menuButton = CreateButton("MenuButton", gameOverPanel.transform, "ANA MENÜ", new Color(0.16f, 0.34f, 0.48f));
            Anchor(menuButton.GetComponent<RectTransform>(), new Vector2(0.16f, 0.11f), new Vector2(0.84f, 0.21f));

            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            eventSystem.transform.SetParent(canvasObject.transform, false);

            hud = canvasObject.AddComponent<GameHudPresenter>();
            gameOver = canvasObject.AddComponent<GameOverPresenter>();
            gameOver.Initialize(gameOverPanel, summary, restartButton, menuButton, RestartFromUi, ReturnToMenuFromUi);
            hud.Initialize(timer, score, hitsText, debug, pauseButton, pausePanel, session, hits, TogglePauseFromUi);
            resumeButton.onClick.AddListener(TogglePauseFromUi);
            pauseRestartButton.onClick.AddListener(RestartFromUi);
        }

        private void TogglePauseFromUi() => gameLoop?.TogglePause();
        private void RestartFromUi() => gameLoop?.Restart();
        private void ReturnToMenuFromUi() => gameLoop?.ReturnToMenuPlaceholder();

        private static GameObject CreatePanel(string name, Transform parent, Color color)
        {
            var panel = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panel.transform.SetParent(parent, false);
            panel.GetComponent<Image>().color = color;
            return panel;
        }

        private static TextMeshProUGUI CreateText(string name, Transform parent, string value, float size, TextAlignmentOptions alignment, Color color)
        {
            var textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(parent, false);
            var text = textObject.GetComponent<TextMeshProUGUI>();
            text.text = value;
            text.fontSize = size;
            text.alignment = alignment;
            text.color = color;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            var font = TMP_Settings.defaultFontAsset ?? Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            if (font != null) text.font = font;
            return text;
        }

        private static Button CreateButton(string name, Transform parent, string label, Color color)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            var image = buttonObject.GetComponent<Image>();
            image.color = color;
            var button = buttonObject.GetComponent<Button>();
            var colors = button.colors;
            colors.highlightedColor = Color.Lerp(color, Color.white, 0.16f);
            colors.pressedColor = Color.Lerp(color, Color.black, 0.16f);
            button.colors = colors;
            var text = CreateText("Label", buttonObject.transform, label, 48f, TextAlignmentOptions.Center, Color.white);
            Stretch(text.rectTransform);
            return button;
        }

        private static GameObject CreatePrimitive(string name, PrimitiveType type, Transform parent, Vector3 position, Vector3 scale, Color color)
        {
            var result = GameObject.CreatePrimitive(type);
            result.name = name;
            result.transform.SetParent(parent, false);
            result.transform.localPosition = position;
            result.transform.localScale = scale;
            var collider = result.GetComponent<Collider>();
            if (collider != null) Object.DestroyImmediate(collider);
            SetMaterial(result, color);
            return result;
        }

        private static void SetMaterial(GameObject target, Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (shader == null) return;
            var material = new Material(shader) { color = color, enableInstancing = true };
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            target.GetComponent<Renderer>().sharedMaterial = material;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void Anchor(RectTransform rect, Vector2 minimum, Vector2 maximum)
        {
            rect.anchorMin = minimum;
            rect.anchorMax = maximum;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private void OnDestroy()
        {
            if (keyboardInput != null && gameLoop != null) keyboardInput.PauseRequested -= gameLoop.TogglePause;
        }
    }
}
