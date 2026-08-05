# Mimari

## İlkeler

- Runtime bağımlılıkları `GameSceneInstaller` composition root'unda bağlanır.
- Oynanış, arayüz ve servis assembly'leri birbirinden ayrıdır.
- `ScriptableObject` tipleri denge ve içerik ayarlarını koddan ayırır.
- Input okuyucuları `RunnerController`'dan ayrıdır.
- Dünya parçaları ve engeller sabit kapasiteli havuzlarda geri dönüştürülür.
- UGS/UI arasında doğrudan bağımlılık kurulmaz; servis arayüzleri kullanılır.

## Assembly'ler

| Assembly | Sorumluluk |
|---|---|
| DogaRun.Core | Durum, oturum, konfigürasyon, veri ve yardımcılar |
| DogaRun.Gameplay | Runner, input, world, obstacle, collision ve scoring |
| DogaRun.UI | Runtime composition root, safe area, HUD ve game over |
| DogaRun.Services | Yerel/mock servis sözleşmeleri ve adaptörleri |
| DogaRun.Editor | Proje/Android build otomasyonu |
| DogaRun.Tests.EditMode | Saf mantık ve veri testleri |
| DogaRun.Tests.PlayMode | MonoBehaviour entegrasyon testleri |

## Runtime akışı

`GameSceneInstaller` → `GameStateMachine` + `RunSession` → input okuyucuları → `RunnerController`; aynı anda `WorldSequenceController` ve `AnimalObstacleSpawner` dünyayı oyuncuya taşır. `ProceduralSunlitForestFactory` havuza girecek görsel şablonu bir kez üretir. `HitStateMachine` çarpışma sayısını ve dokunulmazlığı yönetir. `GameHudPresenter`, oturum ve hit durumunu yalnızca sunar.

## Genişleme noktaları

- `CharacterDefinition`: placeholder renderer yerine gerçek model/prefab.
- `WorldSegmentDefinition` / `WorldSequenceDefinition`: PHASE 2'nin sekiz teması.
- `AnimalObstacleDefinition`: hayvan prefabı ve davranış türü.
- `DifficultyConfig`: beş zorluk profilinin bütün denge değerleri.
- `IAuthenticationService`, `ICloudSaveService`, `ILeaderboardService`: PHASE 4 UGS adaptörleri.

## Performans bütçesi

PHASE 1'de altı kapasiteli world chunk havuzu (beşi aktif) ve sekiz obstacle örneği başlangıçta oluşturulur; oyun döngüsünde Destroy yoktur. SunlitForest parçası 84 primitive renderer ve paylaşılan GPU-instancing materyalleri kullanır. Parça başına tek bitki animatörü, ortam için tek yaprak animatörü vardır; Update içindeki koleksiyonlar yeniden kullanılmaktadır. Hedef orta cihazda 60 FPS, düşük cihazda yapılandırılabilir 30 FPS'tir.
