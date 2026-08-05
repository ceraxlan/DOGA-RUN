# Kalan manuel adımlar

## 1. Bu makinedeki Unity kurulumu

Unity 6000.3.21f1 ve Android araçları kullanıcı dizinine kuruldu:

```text
C:\Users\cerax\AppData\Local\Unity\Hub\Editor\6000.3.21f1
```

Test ve build scriptleri varsayılan `Program Files` yolunun dışında kalan bu kuruluma `-UnityPath` ile erişebilir:

```powershell
$unity = "$env:LOCALAPPDATA\Unity\Hub\Editor\6000.3.21f1\Editor\Unity.exe"
./scripts/run-tests.ps1 -UnityPath $unity
./scripts/build-android-development.ps1 -UnityPath $unity
```

Windows smoke doğrulaması için üretilen `Builds/Windows/DogaRun.exe` doğrudan çalıştırılabilir. Bu çıktı yalnızca yerel doğrulama içindir ve `Builds/` kuralıyla Git dışında tutulur.

## 2. GitHub durumu

`main` dalı `https://github.com/ceraxlan/DOGA-RUN.git` deposuna yüklenmiştir. Sonraki değişiklikler için mevcut `ceraxlan` GitHub CLI oturumu kullanılabilir:

```powershell
git push -u origin main
```

Yeni repository oluşturmayın; hedef repository zaten vardır.

## 3. Fiziksel Android cihaz smoke testi

USB debugging açık cihazı bağladıktan sonra Unity ile gelen ADB kullanılarak:

```powershell
$adb = "$env:LOCALAPPDATA\Unity\Hub\Editor\6000.3.21f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools\adb.exe"
& $adb devices -l
& $adb install -r .\Builds\Android\DogaRun-development.apk
```

Oyunda şerit değiştirme, jump, slide, pause, üç çarpışma, game over ve restart akışlarını cihazda kontrol edin.

## 4. Android release imzalama

Gerçek değerleri yalnızca yerel terminal/CI secret store içinde tanımlayın:

```powershell
$env:DOGARUN_KEYSTORE_PATH = 'C:\secure\dogarun-upload.jks'
$env:DOGARUN_KEYSTORE_PASSWORD = '...'
$env:DOGARUN_KEY_ALIAS = '...'
$env:DOGARUN_KEY_ALIAS_PASSWORD = '...'
./scripts/build-android-release.ps1 -UnityPath $unity
```

Keystore'u, şifreleri veya `signing.properties` dosyasını repository'ye eklemeyin.

## 5. UGS (PHASE 4)

UGS paketleri ve gerçek servis adaptörleri kasıtlı olarak PHASE 4'e bırakılmıştır. Şu anki proje yerel/misafir servis sözleşmeleri ve mock genişleme noktalarıyla servis yapılandırması olmadan açılabilir.
