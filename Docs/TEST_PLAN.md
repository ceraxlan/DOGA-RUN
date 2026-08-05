# Test planı

## Otomasyon

`scripts/run-tests.ps1`, Unity batch mode ile EditMode ve PlayMode testlerini ayrı XML sonuçlarına yazar.

EditMode kapsamı: zorluk değerleri, lane sınırları, skor, hit geçişleri, üçüncü hit game over, dünya sırası/döngüsü, hız çarpanı, top-five ve duplicate RunId, süre biçimi.

PlayMode kapsamı: lane hareketleri/sınırlar, jump, slide, obstacle tek-hit, recovery ve final hit, game over görünümü, restart sıfırlama, chunk recycling, pause sırasında süre, prosedürel karakter rig/koşu hareketi ve SunlitForest görsel hiyerarşisi.

## Manuel smoke test

- 1080x1920 Game görünümünde safe area ve okunabilirlik.
- Klavye ve cihazda her swipe yönü.
- Üç farklı collider çarpışması ve dokunulmazlık aralığı.
- Pause/resume, game over ve tekrar koş.
- 10 dakikalık koşuda aktif world/obstacle sayısının sabit kalması.
- Düşük/orta Android cihazda 30/60 FPS profili.

## Mevcut doğrulama durumu

2026-08-05 tarihinde Unity 6000.3.21f1 ile EditMode **15/15**, PlayMode **11/11** geçti; failed/skipped yok. Windows standalone runtime smoke günlüğünde exception/crash bulunmadı. Android ARM64 development APK başarıyla üretildi ve APK Signature Scheme v2 ile doğrulandı.
