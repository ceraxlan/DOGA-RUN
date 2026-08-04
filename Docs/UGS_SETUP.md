# Unity Gaming Services kurulumu (PHASE 4)

UGS kurulumu bu ilk çalıştırmanın kapsamı değildir. PHASE 4'te:

1. Unity Cloud projesi oluşturulup Editor projesiyle bağlanacak.
2. UGS Core, Authentication, Cloud Save ve Leaderboards paket sürümleri Unity 6000.3.21f1 için doğrulanıp gerekçeleriyle eklenecek.
3. Username & Password provider etkinleştirilecek.
4. UI yalnızca `IAuthenticationService`, `ICloudSaveService` ve `ILeaderboardService` sözleşmelerine bağlanacak.
5. Token ve şifre loglanmayacak veya PlayerPrefs'e yazılmayacak.
6. Yerel/bulut koşu birleştirmesi `RunId` ile tekilleştirilecek.
7. Offline ve hesap silme akışları gerçek cihazda doğrulanacak.

PHASE 1 servis yapılandırması olmadan çalışır; gerçek UGS sonucu simüle edilmez.
