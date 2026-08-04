# Manuel adımlar

## 1. Unity ve Android desteği

Bu makinede Unity Hub ve Unity Editor tespit edilemedi.

1. Unity Hub'ı resmi Unity sitesinden kurun.
2. Unity Hub üzerinden **Unity 6000.3.21f1** kurun.
3. Kurulum modüllerinde **Android Build Support**, **Android SDK & NDK Tools** ve **OpenJDK** seçeneklerini işaretleyin.
4. Unity Hub'da `DOGA-RUN` klasörünü proje olarak ekleyip açın.
5. Paket çözümleme bittikten sonra Console'da hata olmadığını doğrulayın.
6. `Assets/DogaRun/Scenes/Game.unity` sahnesini açıp Play ile smoke test yapın.
7. `./scripts/run-tests.ps1` çalıştırın. Windows execution policy engellerse yalnızca bu çağrı için:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\run-tests.ps1
```

Unity 6000.3.21f1 resmi yayın sayfası: https://unity.com/releases/editor/whats-new/6000.3.21f1

## 2. GitHub private repository

GitHub CLI kurulu fakat `ceraxlan` hesabının token'ı geçersiz. Yeniden oturum açtıktan sonra repository kökünde:

```powershell
gh auth login -h github.com
gh repo create DOGA-RUN --private --source . --remote origin --push
```

Var olan bir `origin` görülürse komutu çalıştırmadan önce remote sahibini ve URL'yi doğrulayın; bu proje otomatik olarak hiçbir remote'u değiştirmez.

## 3. Android release imzalama

Gerçek değerleri yalnızca yerel terminal/CI secret store içinde tanımlayın:

```powershell
$env:DOGARUN_KEYSTORE_PATH = 'C:\secure\dogarun-upload.jks'
$env:DOGARUN_KEYSTORE_PASSWORD = '...'
$env:DOGARUN_KEY_ALIAS = '...'
$env:DOGARUN_KEY_ALIAS_PASSWORD = '...'
./scripts/build-android-release.ps1
```

Keystore'u, şifreleri veya `signing.properties` dosyasını repository'ye eklemeyin.

## 4. UGS (PHASE 4)

UGS paketleri ve gerçek servis adaptörleri kasıtlı olarak PHASE 4'e bırakılmıştır. Şu anki proje yerel/misafir servis sözleşmeleri ve mock genişleme noktalarıyla servis yapılandırması olmadan açılabilir.
