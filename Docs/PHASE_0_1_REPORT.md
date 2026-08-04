# PHASE 0–1 uygulama raporu

Tarih: 2026-08-04

## Oluşturulan kapsam

- Unity repository kökü, Git/LFS kuralları, paket manifesti ve Android PlayerSettings.
- İstenen `Assets/DogaRun` klasör ağacı, beş sahne ve yedi assembly tanımı.
- Runtime composition root ile placeholder Doğa, portre kamera, üç şeritli orman yolu, havuzlanan world chunk ve köpek engeli.
- Keyboard/swipe input, yumuşak lane geçişi, jump, slide, pause, üç-hit state machine, skor/süre HUD'u, game over ve restart.
- ScriptableObject içerik/genişleme tipleri, yerel/mock servis sözleşmeleri ve Editor asset üreticisi.
- EditMode/PlayMode test kaynakları, Windows setup/test ve Android development/release build scriptleri.

## Unity ayarları

Proje 6000.3.21f1'e sabitlendi. Product/company/package `Doğa Koşusu`, `Ceraxlan Software`, `org.ceraxlan.dogarun`; version `0.1.0 (1)`; portrait-only; min API 26; target API 36; IL2CPP; ARM64. Release komutu AAB üretir. İlk Editor importunda idempotent configurator URP renderer/pipeline ve ScriptableObject assetlerini oluşturur.

## Çalıştırılan doğrulamalar

- Araç keşfi: Git 2.55.0, GitHub CLI 2.96.0, Git LFS 3.7.1 bulundu; Unity Hub/Editor bulunamadı.
- GitHub CLI auth: kayıtlı `ceraxlan` token'ı geçersiz.
- JSON/asmdef parse: başarılı.
- Dört PowerShell dosyasının parser kontrolü: başarılı.
- 36 C# dosyasının Roslyn sözdizimi taraması: sözdizimi diagnostic'i yok. Unity assembly'leri olmadığı için bu tam derleme değildir.
- `scripts/setup-windows.ps1`: execution-policy bypass ile başarılı; eksik Unity'yi doğru raporladı.
- `scripts/run-tests.ps1`: beklendiği gibi exit 1; Unity 6000.3.21f1 bulunamadığı için hiçbir EditMode/PlayMode testi çalışmadı.

## Test özeti

Başarılı Unity testi: **0**. Başarısız Unity testi: **0**. Çalıştırılamayan test assembly'si: **2** (EditMode ve PlayMode). Testlerin çalışmama nedeni Unity Editor'ın kurulu olmamasıdır; test kaynaklarının varlığı test başarısı sayılmamıştır.

## PHASE 2 öncesi engeller

1. Unity Hub + 6000.3.21f1 + Android SDK/NDK/OpenJDK kurulmalı.
2. İlk import tamamlanıp Unity Console compile error içermediği doğrulanmalı.
3. EditMode ve PlayMode batch testleri çalıştırılmalı; oluşan sorunlar kapatılmalı.
4. Game sahnesi 1080x1920 ve fiziksel Android cihazda smoke test edilmelidir.
5. API 36 development build alınmalıdır.
6. GitHub CLI yeniden yetkilendirilip private `DOGA-RUN` repository oluşturulmalıdır.
