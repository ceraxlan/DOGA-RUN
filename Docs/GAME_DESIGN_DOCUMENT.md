# Game Design Document — Doğa Koşusu

## Ürün özeti

Doğa Koşusu, portre ekranda tek elle oynanabilen, çocuk dostu, üç şeritli ve sonsuz dünya mantığına sahip özgün bir 3D Android koşu oyunudur. Oyuncu, Doğa adlı stilize küçük karakteri hayvanlara zarar vermeden yönlendirir.

## PHASE 1 oyun döngüsü

Oyuncu otomatik koşu hissi veren hareketli bir orman yolunda ilerler. Swipe veya klavye ile şerit değiştirir, zıplar ve kayar. Havuzdan çıkan sevimli hayvan placeholderları oyuncuya doğru gelir. İlk çarpışma düşüp kalkma, ikinci çarpışma tökezleme, üçüncü çarpışma ise çocuk dostu bir game over üretir.

## Kontroller

| Eylem | Mobil | Editor |
|---|---|---|
| Sola geç | Sola swipe | A / Sol Ok |
| Sağa geç | Sağa swipe | D / Sağ Ok |
| Zıpla | Yukarı swipe | W / Yukarı Ok / Space |
| Kay | Aşağı swipe | S / Aşağı Ok |
| Duraklat | HUD butonu | Escape |

Bir gesture yalnızca bir komut üretir. 70 pikselden kısa dokunuşlar filtrelenir. Şerit değişimi yumuşaktır ve lane index `-1..1` aralığındadır.

## Skor

`floor(distanceMeters * difficultyScoreMultiplier) + completedLoopCount * 500`

PHASE 1 tek prosedürel SunlitForest setini kullanır. Tam sekiz çevre teması, geçişleri ve hız döngüsü PHASE 2 kapsamındadır; veri sınıfları bu genişlemeye hazırdır.

## Çocuk güvenliği

Kan, yaralanma, sert savrulma, düşme korkusu veya ağır kamera sarsıntısı yoktur. Placeholder karakter fotogerçekçi değildir. Repository gerçek çocuk fotoğrafı içermez.
