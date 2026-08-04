# Android build

## Sabit ayarlar

- Product: Doğa Koşusu
- Company: Ceraxlan Software
- Identifier: `org.ceraxlan.dogarun`
- Version / code: `0.1.0` / `1`
- Minimum API: 26
- Target API: 36 (gerekli SDK Unity Hub modülünde kurulu olmalıdır)
- Backend: IL2CPP
- Architecture: ARM64
- Orientation: yalnızca Portrait
- Release çıktı biçimi: Android App Bundle (`.aab`)

## Development

```powershell
./scripts/build-android-development.ps1
```

Çıktı `Builds/Android/DogaRun-development.apk` konumuna yazılır.

## Release

`DOGARUN_KEYSTORE_PATH`, `DOGARUN_KEYSTORE_PASSWORD`, `DOGARUN_KEY_ALIAS`, `DOGARUN_KEY_ALIAS_PASSWORD` ortam değişkenlerini güvenli şekilde tanımlayıp:

```powershell
./scripts/build-android-release.ps1
```

Çıktı `Builds/Android/DogaRun-release.aab` konumuna yazılır. Eksik secret varsa script build başlamadan hata verir. Keystore ve secret dosyaları Git'e alınmaz.
