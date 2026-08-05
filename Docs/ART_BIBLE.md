# Art Bible

## Görsel kimlik

Sıcak, yumuşak kenarlı, stilize 3D animasyon filmi estetiği; canlı fakat yorucu olmayan yeşil, turkuaz, sarı ve mercan tonları kullanılır. Fotogerçekçilik ve başka oyunların ayırt edici tasarım dili kullanılmaz.

## Doğa karakteri

Yaklaşık üç yaş görünümünde, yaşına uygun oranlarda, küçük ve stilize bir çocuk karakteridir. Büyük ve yumuşak yüz formu, sarı kısa/kıvırcık saç, katmanlı mavi gözler, pembe yanaklar, turkuaz spor üst, mercan şort ve sarı ayakkabılar ana işaretlerdir. Mor sırt çantasındaki yeşil yaprak rozeti üçüncü şahıs kamerada özgün ve okunabilir bir işaret oluşturur.

PHASE 5 prosedürel modelinde 49 düşük maliyetli primitive renderer; `Body`, `Head`, `HairRoot`, `BackpackRoot`, sol/sağ kol ve sol/sağ bacak semantik eklemlerine bağlanır. Koşu, saç sekmesi, şerit eğimi, zıplama, kayma ve çocuk dostu çarpışma tepkileri bu eklemler üzerinden çalışır. Boy ve ana renk paleti `CharacterDefinition` içindedir. Bu hiyerarşi üretim modeline geçişte gameplay kodunu değiştirmeden bir prefab/rig adaptörü bağlanabilecek şekilde ayrılmıştır.

## Çevre

SunlitForest, portre ekranda güçlü derinlik veren üç sıcak toprak patikayı merkezde tutar. Doygun fakat yumuşak yeşil ağaç katmanları çerçeveyi iki yandan sarar; yaprak kemerleri, eğreltiler, çiçekler, mantarlar ve kayalar ritmik olarak tekrar eder. Şeritler ince yaprak bordürleri ve açık/koyu yüzey farkıyla okunur; ray, para, özgün karakter veya başka bir oyuna ait tanınabilir UI/öğe kullanılmaz.

Tek Directional Light, yumuşak gölge, basit düşük poligon formlar ve GPU instancing destekli paylaşılan materyaller kullanılır. Her havuzlanan parçada tek `ForestFoliageAnimator` birden çok bitki grubunu sallar; ortamda yalnızca 12 yaprak tek `ForestLeafDrift` tarafından hareket ettirilir. Particle System ve gerçek zamanlı ağır volumetric fog kullanılmaz.

## Asset kabulü

Her asset için lisans ve kaynak kaydı zorunludur. Gerçek çocuk fotoğrafları yalnızca Git dışındaki `LocalReference/` dizininde tutulabilir ve oyun asset'i olarak kullanılmaz.
