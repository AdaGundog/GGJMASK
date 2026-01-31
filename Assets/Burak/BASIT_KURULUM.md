# 🎮 AI Commander - Basit Kurulum Rehberi

## ✅ Artık Commander Her Şeyi Yapıyor!

Commander artık:
- ✅ Kaynak yönetimi yapıyor
- ✅ Birim üretiyor
- ✅ Düşmanları savaştırıyor
- ✅ Akıllı kararlar alıyor

---

## 🚀 5 Dakikada Kurulum

### ADIM 1: Yeni Sahne Oluştur (1 dakika)

```
1. File → New Scene
2. File → Save As → "AI_Battle_Test"
```

### ADIM 2: Zemin Hazırla (1 dakika)

```
1. Hierarchy → 3D Object → Plane
2. Scale: (5, 1, 5)
3. Window → AI → Navigation → Bake
```

### ADIM 3: Referans Noktaları Oluştur (1 dakika)

```
1. Hierarchy → Create Empty → "Enemy Spawn"
   Position: (10, 0, 0)

2. Hierarchy → Create Empty → "Enemy Castle"
   Position: (15, 0, 0)

3. Hierarchy → Create Empty → "Player Castle"
   Position: (-15, 0, 0)
```

### ADIM 4: AI Commander Ekle (2 dakika)

```
1. Hierarchy → Create Empty → "Enemy AI Commander"
2. Add Component → "EnemyCommanderAI"
3. Inspector'da ayarla:
```

**ZORUNLU AYARLAR:**
```
Birim Prefab'ları:
├─ Infantry Prefab: [Piyade prefab'ınızı sürükleyin]
├─ Archer Prefab: [Okçu prefab'ınızı sürükleyin]
└─ Cavalry Prefab: [Süvari prefab'ınızı sürükleyin]

Spawn Ayarları:
├─ Enemy Spawn Point: [Enemy Spawn objesini sürükleyin]
├─ Enemy Castle: [Enemy Castle objesini sürükleyin]
└─ Player Castle: [Player Castle objesini sürükleyin]
```

**İSTEĞE BAĞLI AYARLAR:**
```
Kaynak Yönetimi:
├─ Starting Resources: 2000
├─ Resources Per Second: 50
└─ Unlimited Resources: ☐ (test için ✓ yapabilirsiniz)

Üretim Ayarları:
├─ Production Interval: 5 (5 saniyede bir üretim)
└─ Max Units: 20

Debug:
├─ Show Debug Info: ✓
└─ Show Detailed Info: ✓
```

### ADIM 5: Oyuncu Birimleri Ekle (1 dakika)

```
1. PlayerUnit prefab'ınızı sahneye sürükleyin
2. Sol tarafa yerleştirin (örn: -10, 0, 0)
3. 5-10 birim ekleyin
4. Her birimin Tag'i "PlayerUnit" olmalı
```

### ADIM 6: Test Et! ▶️

```
1. Play butonuna bas
2. Console'u aç (Window → General → Console)
3. İzle!
```

---

## 🎯 Ne Göreceksiniz?

### Console'da:
```
[AI Commander] Başlatıldı!
Başlangıç Kaynakları: 2000
Kaynak Geliri: 50/saniye

=== AI Commander Analizi ===
Oyuncu Güçleri: 5 Piyade, 3 Okçu, 2 Süvari
Düşman Güçleri: 0 Piyade, 0 Okçu, 0 Süvari
Oyuncu Tehdit Seviyesi: 0.65
Önerilen Karşı Güç: 2 Piyade, 3 Süvari, 5 Okçu

[AI Commander] Üretildi: 3x Cavalry (Öncelik: 3)
[AI Commander] Üretildi: 2x Infantry (Öncelik: 2)
```

### Sahnede:
- ✅ Düşmanlar otomatik spawn oluyor
- ✅ Oyuncu birimlerine saldırıyor
- ✅ Akıllı hedef seçimi yapıyor
- ✅ Tip avantajına göre hareket ediyor

---

## ⚙️ Hızlı Ayarlar

### Daha Agresif AI İçin:
```
Production Interval: 3 (daha sık üretim)
Resources Per Second: 100 (daha fazla kaynak)
Max Units: 30 (daha fazla birim)
```

### Test İçin:
```
Unlimited Resources: ✓ (sınırsız kaynak)
Production Interval: 2 (çok hızlı üretim)
Show Detailed Info: ✓ (detaylı log)
```

### Dengeli Oyun İçin:
```
Starting Resources: 1000
Resources Per Second: 30
Production Interval: 8
Max Units: 15
```

---

## 🔧 Sorun Giderme

### "Prefab atanmamış" Hatası?
```
✓ Inspector'da Infantry/Archer/Cavalry Prefab'ları atadınız mı?
✓ Prefab'lar EnhancedEnemyUnitAI veya EnemyUnitAI içeriyor mu?
```

### Birimler Spawn Olmuyor?
```
✓ Enemy Spawn Point atandı mı?
✓ Enemy Castle ve Player Castle atandı mı?
✓ NavMesh baked mi?
✓ Console'da hata var mı?
```

### Birimler Hareket Etmiyor?
```
✓ Prefab'larda NavMeshAgent var mı?
✓ NavMesh düzgün baked mi?
✓ PlayerUnit tag'i doğru mu?
```

### Kaynak Biterse?
```
✓ Resources Per Second değerini artırın
✓ Veya Unlimited Resources'u açın
✓ Veya Starting Resources'u artırın
```

---

## 💡 İpuçları

1. **İlk Testi Unlimited Resources ile yapın** - Sistem çalışıyor mu görmek için
2. **Console'u açık tutun** - AI'nın ne yaptığını görün
3. **Production Interval'i ayarlayın** - Oyun hızınıza göre
4. **Max Units ile performansı kontrol edin** - Çok birim = yavaş oyun

---

## 🎮 Örnek Senaryo

**Hedef:** 10 oyuncu birimine karşı AI testi

```
1. 10 PlayerUnit yerleştir (5 Piyade, 3 Okçu, 2 Süvari)
2. Commander ayarları:
   - Unlimited Resources: ✓
   - Production Interval: 3
   - Max Units: 15
3. Play ▶️
4. AI otomatik olarak:
   - Okçulara karşı Süvari üretir
   - Piyadelere karşı Okçu üretir
   - Süvarilere karşı Piyade üretir
5. Savaşı izle!
```

---

## ✅ Başarı Kontrol Listesi

Test başarılı sayılır eğer:

- ✅ Console'da "Başlatıldı" mesajı görünüyor
- ✅ Her 5 saniyede analiz yapılıyor
- ✅ Düşman birimleri spawn oluyor
- ✅ Düşmanlar oyuncu birimlerine saldırıyor
- ✅ AI tip avantajına göre üretim yapıyor
- ✅ Kaynaklar artıyor/azalıyor

---

## 🎉 Hazırsınız!

Artık tam fonksiyonel bir AI Commander'ınız var!

**Keyifli testler! 🚀**

---

## 📞 Hızlı Yardım

**Prefab'ları nasıl atarım?**
→ Project'ten prefab'ı Inspector'daki alana sürükleyin

**Spawn noktası nasıl atarım?**
→ Hierarchy'den objeyi Inspector'daki alana sürükleyin

**Test için hızlı ayar?**
→ Unlimited Resources: ✓, Production Interval: 2

**Gerçek oyun için ayar?**
→ Starting Resources: 1000, Resources Per Second: 30, Production Interval: 8
