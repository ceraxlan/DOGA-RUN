# Test planı

## Otomasyon

`scripts/run-tests.ps1`, Unity batch mode ile EditMode ve PlayMode testlerini ayrı XML sonuçlarına yazar.

EditMode kapsamı: zorluk değerleri, lane sınırları, skor, hit geçişleri, üçüncü hit game over, dünya sırası/döngüsü, hız çarpanı, top-five ve duplicate RunId, süre biçimi.

PlayMode kapsamı: lane hareketleri/sınırlar, jump, slide, obstacle tek-hit, recovery ve final hit, game over görünümü, restart sıfırlama, chunk recycling, pause sırasında süre.

## Manuel smoke test

- 1080x1920 Game görünümünde safe area ve okunabilirlik.
- Klavye ve cihazda her swipe yönü.
- Üç farklı collider çarpışması ve dokunulmazlık aralığı.
- Pause/resume, game over ve tekrar koş.
- 10 dakikalık koşuda aktif world/obstacle sayısının sabit kalması.
- Düşük/orta Android cihazda 30/60 FPS profili.

## Mevcut doğrulama durumu

Unity Editor tespit edilmediği için 2026-08-04 tarihinde test assembly'leri çalıştırılamadı. Bu durum test başarısı olarak raporlanmaz.
