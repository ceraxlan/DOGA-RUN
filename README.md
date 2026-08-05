# Doğa Koşusu

Doğa Koşusu, Android telefonlar için portre modunda çalışan, özgün ve çocuk dostu üç şeritli bir 3D sonsuz koşu oyunudur. Unity proje kökü bu dizindir.

## Mevcut kapsam

PHASE 0 ve PHASE 1 dikey dilimine ek olarak PHASE 5'in karakter ve SunlitForest görsel alt-kapsamı uygulanmıştır: üç şerit, klavye/swipe kontrolü, zıplama, kayma, havuzlanan orman parçaları ve hayvan engelleri, üç çarpışma akışı, süre/skor HUD'u, pause, game over ve yeniden başlatmanın yanında özgün prosedürel Doğa karakteri, koşu animasyonları, sıcak üç patikalı orman, yaprak kemerleri ve düşük maliyetli çevre hareketi bulunur.

Proje Unity 6000.3.21f1 ile doğrulanmıştır. EditMode ve PlayMode testleri geçer; Android ARM64 development APK ve Windows runtime smoke build'i başarıyla üretilir. Yerel kurulum ve kalan yayın adımları için `MANUAL_STEPS.md` dosyasına bakın.

## Gereksinimler

- Unity 6000.3.21f1 (Unity 6.3 LTS)
- Android Build Support: SDK/NDK/OpenJDK
- Git ve Git LFS
- PowerShell 5.1 veya daha yeni

## İlk açılış

1. Unity Hub ile bu klasörü açın.
2. Paketlerin çözülmesini ve script importunun tamamlanmasını bekleyin.
3. `Assets/DogaRun/Scenes/Game.unity` sahnesini açın.
4. Play düğmesine basın. Sahne, prosedürel Doğa karakterini ve SunlitForest görsellerini çalışma anında oluşturur.
5. Testleri `./scripts/run-tests.ps1` ile çalıştırın.

Editör kontrolleri: `A/Sol Ok`, `D/Sağ Ok`, `W/Yukarı Ok/Space`, `S/Aşağı Ok`, `Escape`.

## Mimari

Oynanış, UI ve servis assembly'leri ayrıdır. `GameSceneInstaller`, yalnızca composition root olarak çalışma zamanı bağımlılıklarını bağlar. Dünya ve engeller gameplay sırasında sürekli Instantiate/Destroy yapmaz; başlangıçta oluşturulan sabit boyutlu havuzları geri dönüştürür.

## Android

Development build için `./scripts/build-android-development.ps1`; imzalı release AAB için gerekli ortam değişkenlerini ayarladıktan sonra `./scripts/build-android-release.ps1` kullanılır. Ayrıntılar `Docs/ANDROID_BUILD.md` içindedir.

## Gizlilik

İlk sürüm reklam, takip SDK'sı, konum, kamera, mikrofon, rehber veya kullanıcılar arası mesajlaşma içermez. `LocalReference/` ve tüm imzalama/secret dosyaları Git dışında tutulur.
