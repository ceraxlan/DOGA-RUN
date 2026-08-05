# Art Bible

## Görsel kimlik

Sıcak, yumuşak kenarlı, stilize 3D animasyon filmi estetiği; canlı fakat yorucu olmayan yeşil, turkuaz, sarı ve mercan tonları kullanılır. Fotogerçekçilik ve başka oyunların ayırt edici tasarım dili kullanılmaz.

## Doğa karakteri

Yaklaşık üç yaş görünümünde, yaşına uygun oranlarda, küçük ve stilize bir çocuk karakteridir. Büyük ve yumuşak yüz formu, sarı kısa/kıvırcık saç, katmanlı mavi gözler, pembe yanaklar, turkuaz spor üst, mercan şort ve sarı ayakkabılar ana işaretlerdir. Sırtındaki yeşil yaprak rozeti üçüncü şahıs kamerada özgün ve okunabilir bir işaret oluşturur.

PHASE 5 prosedürel modelinde 42 düşük maliyetli primitive renderer; `Body`, `Head`, sol/sağ kol ve sol/sağ bacak semantik eklemlerine bağlanır. Koşu, şerit eğimi, zıplama, kayma ve çocuk dostu çarpışma tepkileri bu eklemler üzerinden çalışır. Boy ve ana renk paleti `CharacterDefinition` içindedir. Bu hiyerarşi üretim modeline geçişte gameplay kodunu değiştirmeden bir prefab/rig adaptörü bağlanabilecek şekilde ayrılmıştır.

## Çevre

Tek Directional Light, yumuşak gölge, basit düşük poligon formlar, GPU instancing destekli materyaller ve sınırlı partikül kullanılır. Gerçek zamanlı ağır volumetric fog kullanılmaz. PHASE 1; toprak yol, çim şeritler, ağaç primitive'leri ve sıcak gökyüzü rengiyle SunlitForest prototipidir.

## Asset kabulü

Her asset için lisans ve kaynak kaydı zorunludur. Gerçek çocuk fotoğrafları yalnızca Git dışındaki `LocalReference/` dizininde tutulabilir ve oyun asset'i olarak kullanılmaz.
