# PHASE 0–1 uygulama ve doğrulama raporu

Tarih: 2026-08-05

## Oluşturulan kapsam

- Unity repository kökü, Git/LFS kuralları, paket manifesti ve Android PlayerSettings.
- `Assets/DogaRun` klasör ağacı, beş sahne ve yedi assembly tanımı.
- Runtime composition root ile placeholder Doğa, portre kamera, üç şeritli orman yolu, havuzlanan world chunk ve köpek engeli.
- Keyboard/swipe input, yumuşak lane geçişi, jump, slide, pause, üç-hit state machine, skor/süre HUD'u, game over ve restart.
- ScriptableObject içerik/genişleme tipleri, yerel/mock servis sözleşmeleri ve Editor asset üreticisi.
- EditMode/PlayMode testleri ile Windows test ve Android development/release build scriptleri.

## Unity ve Android kurulumu

- Proje Unity `6000.3.21f1 (c02631ffc030)` ile temiz import edildi.
- Editor, yönetici izni gerektirmeyen kullanıcı dizinine kuruldu: `C:\Users\cerax\AppData\Local\Unity\Hub\Editor\6000.3.21f1`.
- Android Playback Engine, OpenJDK 17.0.18, NDK r27c, CMake 3.22.1, SDK/Build Tools 36 ve API 34/35/36 doğrulandı.
- Product/company/package `Doğa Koşusu`, `Ceraxlan Software`, `org.ceraxlan.dogarun`; version `0.1.0 (1)`; portrait-only; min API 26; target API 36; IL2CPP; ARM64.

## Düzeltilen sorunlar

- `TagManager.asset` içindeki Unity YAML parser'ının kabul etmediği boş layer değerleri açık boş string olarak düzeltildi.
- `AudioConfiguration` tipinin Unity 6 ile oluşan ad çakışması tam namespace kullanılarak giderildi.
- TextMeshPro wrapping ayarı Unity 6 API'sine taşındı.
- Test ve Android build PowerShell scriptleri Unity ana prosesinin gerçek çıkış kodunu bekleyecek şekilde düzeltildi.
- Platform başına doğru test assembly filtresi eklendi.
- Unity Editor'ı doğru sahnede Play Mode'a alan `DogaRun/Play Vertical Slice` komutu eklendi.

## Doğrulama sonuçları

- Temiz import ve tüm C# assembly derlemesi: başarılı, C# warning/error yok.
- EditMode: **15/15** test vakası geçti.
- PlayMode: **8/8** test vakası geçti.
- Toplam: **23/23** test vakası geçti; failed/skipped yok.
- Android development APK: başarılı.
  - Çıktı: `Builds/Android/DogaRun-development.apk`
  - Boyut: 69.18 MB
  - SHA-256: `D8DEBE9F90D787710AED8FA473D011D0C408E41A917D30A57FEC9D58C200DA45`
  - Package/version: `org.ceraxlan.dogarun`, `0.1.0 (1)`
  - min/target/compile SDK: 26/36/36
  - Native ABI: `arm64-v8a`
  - APK Signature Scheme v2 doğrulaması: başarılı

## Kalan manuel adımlar

1. USB debugging açık fiziksel Android cihazda APK smoke testi yapılmalı.
2. GitHub CLI `ceraxlan` hesabıyla yeniden yetkilendirilmeli ve yerel commit `origin` deposuna push edilmelidir.
3. Release AAB için keystore ortam değişkenleri sağlanmalı; development debug anahtarı yayın için kullanılmamalıdır.
