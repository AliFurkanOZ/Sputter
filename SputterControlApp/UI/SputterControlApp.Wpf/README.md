# Sputter Control — WPF arayüzü

## Çalıştırma

Solution'da SputterControlApp.Wpf projesini başlangıç projesi seçin veya:

```powershell
dotnet run --project UI/SputterControlApp.Wpf/SputterControlApp.Wpf.csproj
```

Debug çıktısı: bin/Debug/net10.0-windows/SputterControlApp.Wpf.exe.

## Sabitlenen ekran düzeni

- Üst sekmeler: Process, Graph, QCM, Log Event, Debug, Sputter Şeması (F1–F6).
- Process, WinForms sırasını korur: solda manuel kontroller; yanında DC, RF1, RF2.
  Step 1/2, ramp, dwell, off ramp, kalan süre, target, readback ve MFC ayarları
  aynı ekrandadır. Ayrı kaynak ayar penceresi kaldırılmıştır.
- Alt bölüm: vakum bilgisi ve RF1/RF2/DC sensör özeti.
- QCM: Material, Process, Layer listeleri yan yana; detay ve layer kontrolleri sağda.
  Layer seçimi detay alanlarını günceller. Edit seçili satırın değerlerini açar.
- Graph: sol üst vakum, sağ üst kalınlık, sol alt rate, sağ alt sıcaklık.
- Açıklama panelleri, süreç adımları, sahte alarm bandı ve sahte bağlantı durumu kaldırıldı.
- 1920×1080 hedeflenir. Küçük pencerelerde okunur yazı boyutu korunur,
  gerekiyorsa kaydırma kullanılır; tüm ekran küçültülmez.

## Arka plan bağlantısına geçiş

Bu aşama yalnızca UI'dır. Core / WinForms proje referansı, Modbus, seri port,
polling, veri kaydı veya gerçek cihaz komutu yoktur. Pasif cihaz düğmeleri
bağlantı ve durum kuralları uygulanınca etkinleştirilmelidir.

Models/WorkspaceState.cs, UI değerleri için INotifyPropertyChanged sunar:

| UI alanı | Bağlantı noktası |
| --- | --- |
| DC/RF1/RF2 | State.Dc / State.Rf1 / State.Rf2 |
| İki adım | Step1/Step2.Setpoint, Ramp, Dwell, Remaining |
| DC limitleri | VoltageSet, CurrentSet, PowerSet, OffRamp |
| Gaz | Argon/Oxygen.Setpoint ve Actual |
| Kaynak readback | Read1/Read2/Read3 (DC: W/V/mA, RF: forward/reflected/set W) |
| Target | Target ve UsedWh |
| Vakum / turbo | State.Pressure, Capacitance, TurboFrequency |
| Valf konumu | State.GateValve/RoughingValve/BypassValve/VentValve/TransferValve.IsOpen |
| MFC valf konumu | State.Dc/Rf1/Rf2.Argon/Oxygen.Valve.IsOpen |
| Manuel setler | State.RotationSet, HeaterSet, GateSet |
| Manuel cihaz durumu | State.Devices[].Status |
| QCM | Preview.Sensors[].Rate, Thickness, Life |
| Seçili katman | Preview.SelectedLayer, SelectedMaterial, SelectedProcess |

Sayısal girişler decimal özelliklere bağlanır. Geçersiz sayı WPF doğrulama
işaretiyle gösterilir. Fiziksel limitler, birimler, register dönüşümleri ve
yazma onayı entegrasyon sırasında mevcut servis sözleşmeleriyle doğrulanmalıdır.
UI modeli her tuşta PLC'ye yazmaz. Yeni readback değerleri UI thread'inde
güncellenmelidir. Komutların etkinlik koşulları ve yürütülmesi henüz eklenmemiştir.

Material/process/layer listeleri ve target ömrü görsel yerleşim için örnektir.
Kaynak ayarları bellekte kalır; hiçbir ayar veya reçete dosyası okunmaz/yazılmaz.
Gerçek ölçümler bağlantı yokken “—” gösterilir. Grafik eğrileri sabit örnektir.
Düzenleme formlarında kaydetme, silme, dışa aktarma ve cihaz işlemleri pasiftir.
Şema temsili bir yerleşimdir; gerçek P&ID veya doğrulanmış valf eşlemesi değildir.

Valf göstergeleri komut düğmesi değildir: true = AÇIK (dolu simge/koyu zemin),
false = KAPALI (boş simge/açık zemin), null = BİLİNMİYOR (soru işareti/gri zemin).
Başlangıçta konum bilinmez; bağlantı kaybında entegrasyon katmanı null atamalıdır.
Setpoint veya gönderilen komut, doğrulanmış konum yerine kullanılmamalıdır.
RF1 ve RF2 üst ayar alanları ortak satır yüksekliği kullanır; MFC başlıkları hizalı kalır.

## Doğrulama

```powershell
dotnet build UI/SputterControlApp.Wpf/SputterControlApp.Wpf.csproj
UI/SputterControlApp.Wpf/bin/Debug/net10.0-windows/SputterControlApp.Wpf.exe --render-preview UI/SputterControlApp.Wpf/bin/Debug/layout-review
```

Render modu gerçek WPF görünümlerini 1920 ve 1366 piksel genişlikte kontrol
etmek için PNG üretir. Normal kullanımda dosya üretmez.
Render sırasında iki çözünürlükte MFC hizası ve valf durum geçişleri de doğrulanır;
sonuç readability-checks.txt dosyasına yazılır.
