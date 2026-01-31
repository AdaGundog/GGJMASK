# 🎮 Basit AI Sistemi Kurulum Rehberi (GGJ)

## ✅ YAPILAN İYİLEŞTİRMELER

### 1. EnemyUnitAI (Akıllı Hedef Seçimi)
- ✅ Tip avantajına göre hedef seçimi
- ✅ Mesafe hesaplama
- ✅ Can durumuna göre önceliklendirme
- ✅ Rank bonusları
- ✅ Gerçek hasar verme sistemi

### 2. EnemyCommander (Akıllı Strateji)
- ✅ Oyuncu kompozisyonunu analiz etme
- ✅ En baskın tipe karşı strateji
- ✅ Kaynak yönetimi
- ✅ Debug mesajları

---

## 🚀 KURULUM ADIMLARI

### ADIM 1: Düşman Prefab'larını Hazırla

Her düşman prefab'ında olması gerekenler:

#### Infantry (Piyade) Prefab
```
Bileşenler:
- Sprite Renderer
- Circle Collider 2D
- Rigidbody 2D (Kinematic)
- NavMeshAgent
  - Update Rotation: ❌ KAPALI
  - Update Up Axis: ❌ KAPALI
  - Speed: 3.5
- EnemyUnitAI (Script)
  - Unit Type: Infantry
  - Current Rank: Rookie
  - Max Health: 100
  - Attack Range: 1.5
  - Attack Cooldown: 1.0

Tag: "EnemyUnit"
```

#### Archer (Okçu) Prefab
```
Aynı bileşenler, sadece:
- EnemyUnitAI:
  - Unit Type: Archer
  - Attack Range: 4.0
  - Attack Cooldown: 1.5
```

#### Cavalry (Atlı) Prefab
```
Aynı bileşenler, sadece:
- NavMeshAgent:
  - Speed: 5.0
- EnemyUnitAI:
  - Unit Type: Cavalry
  - Max Health: 120
  - Attack Range: 1.8
  - Attack Cooldown: 0.8
```

---

### ADIM 2: Commander GameObject Oluştur

1. **Hierarchy → Create Empty**
   - İsim: "Enemy Commander"
   - Position: (0, 0, 0)

2. **Add Component → Enemy Commander**

3. **Inspector Ayarları:**

```
Ayarlar:
- Spawn Point: [Düşman spawn noktası GameObject'i]
- My Castle Transform: [Düşman kalesi]
- Enemy Castle Transform: [Oyuncu kalesi]
- Decision Interval: 2 (2 saniyede bir karar)
- Current Level: 1

Ekonomi:
- Current Gold: 100
- Income Rate: 10 (saniyede 10 altın)

Prefablar:
- Infantry Prefab: [Infantry prefab'ını sürükle]
- Archer Prefab: [Archer prefab'ını sürükle]
- Cavalry Prefab: [Cavalry prefab'ını sürükle]

Maliyetler:
- Infantry Cost: 30
- Archer Cost: 50
- Cavalry Cost: 80
```

---

### ADIM 3: Referans Noktalarını Oluştur

Hierarchy'de boş GameObject'ler:

```
1. "Enemy Spawn"
   Position: (8, 0, 0) // Haritanın sağ tarafı

2. "Enemy Castle"
   Position: (10, 3, 0) // Sağ üst köşe

3. "Player Castle"
   Position: (-10, 3, 0) // Sol üst köşe
```

**ÖNEMLİ:** Tüm pozisyonlarda Z=0 olmalı!

---

### ADIM 4: NavMesh Kurulumu

1. **Grid GameObject'ini seç** (veya zemin objesi)
2. **Add Component → NavMeshSurface**
3. **Ayarlar:**
   ```
   Agent Type: Humanoid
   Collect Objects: All
   ```
4. **"Bake" butonuna tıkla**
5. Scene view'da mavi NavMesh alanı görünmeli

---

### ADIM 5: Tag'leri Ayarla

1. **Edit → Project Settings → Tags and Layers**
2. **Tags bölümünde "+" tıkla**
3. **Ekle:**
   - "PlayerUnit"
   - "EnemyUnit"

4. **Oyuncu birimlerine "PlayerUnit" tag'i ata**
5. **Düşman birimlerine "EnemyUnit" tag'i ata**

---

## 🎮 TEST ETME

### 1. Play Butonuna Bas

### 2. Beklenen Davranışlar

✅ **Commander:**
- Her 2 saniyede oyuncu kompozisyonunu analiz eder
- Console'da şöyle mesajlar görünür:
  ```
  [AI Commander] Oyuncu Güçleri: 3 Piyade, 2 Okçu, 1 Atlı
  [AI Commander] Archer üretildi! Kalan Altın: 50
  ```
- En çok olan tipe karşı birim üretir

✅ **Düşman Birimleri:**
- Tip avantajı olan hedefleri seçer
- Yakın hedeflere öncelik verir
- Düşük canlı hedefleri bitirmeye çalışır
- Gerçek hasar verir

### 3. Console Mesajları

```
[AI Commander] Oyuncu Güçleri: 2 Piyade, 2 Okçu, 1 Atlı
[AI Commander] Cavalry üretildi! Kalan Altın: 20
Enemy_Infantry (Infantry) vurdu: Player_Archer - Hasar: 10
```

---

## 🐛 Sorun Giderme

### Düşmanlar Spawn Olmuyor

**Çözüm:**
1. ✅ Prefab'lar atandı mı?
2. ✅ Commander'da yeterli altın var mı?
3. ✅ Spawn Point atandı mı?
4. ✅ Console'da hata var mı?

### Düşmanlar Hareket Etmiyor

**Çözüm:**
1. ✅ NavMesh bake edildi mi?
2. ✅ NavMeshAgent Update Rotation/Up Axis KAPALI mı?
3. ✅ Spawn pozisyonu NavMesh üzerinde mi?
4. ✅ Enemy Castle ve Player Castle atandı mı?

### "PlayerUnit" Tag Bulunamıyor

**Çözüm:**
1. Edit → Project Settings → Tags
2. "PlayerUnit" ve "EnemyUnit" ekle
3. Birimlere tag'leri ata

### Düşmanlar Saldırmıyor

**Çözüm:**
1. ✅ Attack Range ayarlandı mı?
2. ✅ PlayerUnit scripti var mı?
3. ✅ TakeDamage fonksiyonu çalışıyor mu?

---

## ⚙️ AYARLAMA İPUÇLARI

### Kolay Mod
```
Commander:
- Income Rate: 15 (hızlı altın)
- Infantry Cost: 20
- Archer Cost: 30
- Cavalry Cost: 50
- Decision Interval: 3 (yavaş üretim)
```

### Normal Mod
```
Commander:
- Income Rate: 10
- Infantry Cost: 30
- Archer Cost: 50
- Cavalry Cost: 80
- Decision Interval: 2
```

### Zor Mod
```
Commander:
- Income Rate: 20 (çok hızlı altın)
- Infantry Cost: 25
- Archer Cost: 40
- Cavalry Cost: 60
- Decision Interval: 1 (çok hızlı üretim)
```

### Level Sistemi

Commander'da `currentLevel` değişkenini artırarak:
- Level 1: Rookie düşmanlar
- Level 2: Veteran düşmanlar (%20 daha güçlü)
- Level 3: Elite düşmanlar (%50 daha güçlü, geri çekilip iyileşir)

---

## 📊 AI NASIL ÇALIŞIYOR?

### Hedef Seçimi (EnemyUnitAI)

Öncelik hesaplama:
```
Öncelik = 
  + Yakınlık Puanı (40%)
  + Tip Avantajı Puanı (40%)
  + Can Durumu Puanı (20%)
  + Rank Bonusu
```

**Örnek:**
- Piyade düşman, 5 birim uzakta Atlı görür → +40 puan (avantajlı)
- Piyade düşman, 3 birim uzakta Okçu görür → -30 puan (dezavantajlı)
- Düşük canlı hedef → +20 puan

### Strateji (EnemyCommander)

1. Oyuncu birimlerini say
2. En çok olan tipi bul
3. Ona karşı etkili birim üret

**Örnek:**
- Oyuncu: 5 Okçu, 2 Piyade, 1 Atlı
- En çok: Okçu
- Karşı strateji: Atlı üret (Okçulara karşı güçlü)

---

## ✅ Kurulum Tamamlandı!

Artık basit ama etkili bir AI sisteminiz var! 🎉

**Sonraki Adımlar:**
1. Test et ve ayarları optimize et
2. Farklı level'larda dene
3. Zorluk seviyelerini ayarla
4. Gerekirse ince ayar yap

İyi oyunlar! 🚀
