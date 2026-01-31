# 🎮 SÜPER DETAYLI AI SİSTEMİ KURULUM REHBERİ

## 📋 İçindekiler
1. [Ön Hazırlık](#ön-hazırlık)
2. [NavMesh Kurulumu](#navmesh-kurulumu)
3. [Düşman Prefab Hazırlama](#düşman-prefab-hazırlama)
4. [Commander Kurulumu](#commander-kurulumu)
5. [Test ve Çalıştırma](#test-ve-çalıştırma)
6. [Sorun Giderme](#sorun-giderme)

---

## 🔧 ÖN HAZIRLIK

### Adım 1: Tag'leri Oluştur

**1.1. Unity Editor'ü Aç**

**1.2. Menüden Tag Ayarlarını Aç:**
- Üst menüden **Edit** → **Project Settings** tıkla
- Sol taraftan **Tags and Layers** seç

**1.3. PlayerUnit Tag'i Ekle:**
- **Tags** bölümünde **"+"** butonuna tıkla
- Açılan kutucuğa **"PlayerUnit"** yaz (tırnak işareti olmadan)
- **Enter** tuşuna bas

**1.4. EnemyUnit Tag'i Ekle:**
- Tekrar **"+"** butonuna tıkla
- **"EnemyUnit"** yaz
- **Enter** tuşuna bas

**1.5. Pencereyi Kapat**

✅ **Kontrol:** Tags bölümünde "PlayerUnit" ve "EnemyUnit" görünmeli

---

### Adım 2: Referans Noktalarını Oluştur

**2.1. Enemy Spawn Noktası:**

1. **Hierarchy** penceresinde boş alana **sağ tık**
2. **Create Empty** seç
3. Oluşan GameObject'e **isim ver:**
   - Hierarchy'de GameObject'e **tek tık**
   - **F2** tuşuna bas (veya tekrar tek tık)
   - **"Enemy Spawn"** yaz (tırnak olmadan)
   - **Enter** tuşuna bas

4. **Pozisyonu ayarla:**
   - Hierarchy'de **"Enemy Spawn"** seçili olsun
   - **Inspector** penceresinde **Transform** bölümünü bul
   - **Position** değerlerini yaz:
     ```
     X: 8
     Y: 0
     Z: 0
     ```
   - Her değeri yazdıktan sonra **Enter** tuşuna bas

**2.2. Enemy Castle:**

1. Hierarchy'de boş alana **sağ tık**
2. **Create Empty** seç
3. İsim: **"Enemy Castle"**
4. Position:
   ```
   X: 10
   Y: 3
   Z: 0
   ```

**2.3. Player Castle:**

1. Hierarchy'de boş alana **sağ tık**
2. **Create Empty** seç
3. İsim: **"Player Castle"**
4. Position:
   ```
   X: -10
   Y: 3
   Z: 0
   ```

✅ **Kontrol:** Hierarchy'de 3 boş GameObject görünmeli:
- Enemy Spawn (8, 0, 0)
- Enemy Castle (10, 3, 0)
- Player Castle (-10, 3, 0)

---

## 🗺️ NAVMESH KURULUMU

### Adım 3: NavMesh Surface Ekle

**3.1. Zemin GameObject'ini Bul:**
- Hierarchy'de **"Ground"** veya **"Grid"** isimli GameObject'i bul
- Eğer yoksa, zemin sprite'ının olduğu GameObject'i seç

**3.2. NavMeshSurface Bileşeni Ekle:**

1. Zemin GameObject'i **seçili** olsun
2. **Inspector** penceresinde en altta **"Add Component"** butonuna tıkla
3. Arama kutusuna **"NavMeshSurface"** yaz
4. Çıkan sonuçtan **"NavMeshSurface"** seç ve tıkla

**3.3. NavMeshSurface Ayarları:**

Inspector'da NavMeshSurface bileşenini bul ve şu ayarları yap:

```
Agent Type: Humanoid
Default Area: Walkable
Use Geometry: Render Meshes
Collect Objects: All
```

**Detaylı Ayarlama:**
- **Agent Type:** Dropdown'dan **"Humanoid"** seç
- **Default Area:** Dropdown'dan **"Walkable"** seç
- **Use Geometry:** Dropdown'dan **"Render Meshes"** seç
- **Collect Objects:** Dropdown'dan **"All"** seç

**3.4. NavMesh'i Bake Et:**

1. NavMeshSurface bileşeninde en altta **"Bake"** butonunu bul
2. **"Bake"** butonuna tıkla
3. Birkaç saniye bekle

**3.5. NavMesh'i Görselleştir:**

1. **Scene** view'a geç (Game view değil!)
2. Scene view'ın **sağ üst köşesinde** "Gizmos" düğmesi var
3. Gizmos düğmesine tıkla
4. Açılan listede **"NavMesh"** seçeneğini bul
5. NavMesh'in yanındaki **checkbox'ı işaretle**

✅ **Kontrol:** Scene view'da zemin üzerinde **mavi renkli alan** görünmeli

**SORUN:** Mavi alan görünmüyorsa?
- Bake butonuna tekrar tıkla
- Zemin GameObject'inin Layer'ı "Default" olmalı
- Zemin GameObject'inin Scale değerleri çok küçük olmamalı

---

## 🎭 DÜŞMAN PREFAB HAZIRLAMA

### Adım 4: Infantry (Piyade) Prefab'ı Hazırla

**4.1. Mevcut Prefab'ı Aç:**

1. **Project** penceresinde **Assets/Burak/Prefabs** klasörüne git
2. Eğer düşman prefab'ın varsa onu bul (örn: "Enemy_Infantry")
3. Prefab'a **çift tıkla** (Prefab edit moduna girer)

**VEYA Yeni Prefab Oluştur:**

1. Hierarchy'de **sağ tık** → **2D Object** → **Sprite**
2. İsim: **"Enemy_Infantry"**
3. Inspector'da **Sprite Renderer** bileşeninde sprite'ını seç

**4.2. Gerekli Bileşenleri Ekle:**

Şimdi sırayla bileşenleri ekleyeceğiz:

---

#### A) Circle Collider 2D Ekle

1. Inspector'da **"Add Component"** tıkla
2. **"Circle Collider 2D"** yaz ve seç
3. Ayarlar:
   ```
   Radius: 0.3
   Is Trigger: ❌ KAPALI (checkbox boş olmalı)
   ```

---

#### B) Rigidbody 2D Ekle

1. **"Add Component"** tıkla
2. **"Rigidbody 2D"** yaz ve seç
3. Ayarlar:
   ```
   Body Type: Kinematic (dropdown'dan seç)
   Simulated: ✅ AÇIK (checkbox işaretli)
   Constraints:
     - Freeze Position Z: ✅ AÇIK
   ```

**Detaylı:**
- **Body Type:** Dropdown'ı aç, **"Kinematic"** seç
- **Simulated:** Checkbox'ı **işaretle**
- **Constraints:** Açılır menüyü aç
  - **Freeze Position** bölümünde **Z** checkbox'ını işaretle

---

#### C) Nav Mesh Agent Ekle

1. **"Add Component"** tıkla
2. **"Nav Mesh Agent"** yaz ve seç
3. **ÇOK ÖNEMLİ AYARLAR:**

```
Agent Type: Humanoid
Base Offset: 0
Speed: 3.5
Angular Speed: 120
Acceleration: 8
Stopping Distance: 0.5
Auto Braking: ✅ AÇIK
Radius: 0.5
Height: 2

🔴 KRİTİK 2D AYARLARI:
Update Rotation: ❌ KAPALI
Update Up Axis: ❌ KAPALI
```

**Detaylı Ayarlama:**

1. **Agent Type:** Dropdown'dan **"Humanoid"** seç

2. **Steering** bölümü:
   - **Speed:** Kutucuğa **3.5** yaz
   - **Angular Speed:** **120** yaz
   - **Acceleration:** **8** yaz
   - **Stopping Distance:** **0.5** yaz
   - **Auto Braking:** Checkbox'ı **işaretle**

3. **Obstacle Avoidance** bölümü:
   - **Radius:** **0.5** yaz
   - **Height:** **2** yaz

4. **Path Finding** bölümü:
   - **Auto Traverse Off Mesh Link:** ✅ İşaretli
   - **Auto Repath:** ✅ İşaretli

5. **🔴 EN ÖNEMLİ KISIM - 2D Ayarları:**
   - **Update Rotation:** Checkbox'ı **KALDIR** (boş olmalı)
   - **Update Up Axis:** Checkbox'ı **KALDIR** (boş olmalı)

✅ **Kontrol:** Update Rotation ve Update Up Axis checkbox'ları **BOŞ** olmalı!

---

#### D) Enemy Unit AI Scripti Ekle

1. **"Add Component"** tıkla
2. **"Enemy Unit AI"** yaz ve seç
3. Ayarlar:

```
Kimlik:
- Unit Type: Infantry (dropdown'dan seç)
- Current Rank: Rookie (dropdown'dan seç)

Bileşenler:
- Agent: [NavMeshAgent bileşenini sürükle]

Durum:
- Max Health: 100
- Current Health: 100
- Attack Range: 1.5
- Attack Cooldown: 1.0
```

**Detaylı:**

1. **Unit Type:** Dropdown'ı aç, **"Infantry"** seç
2. **Current Rank:** Dropdown'ı aç, **"Rookie"** seç
3. **Agent:** 
   - Aynı GameObject'teki **NavMeshAgent** bileşenini bul
   - NavMeshAgent'ın başlığını **Agent** alanına sürükle
   - VEYA: Agent alanının sağındaki **hedef simgesine** tıkla, açılan pencereden **NavMeshAgent** seç
4. **Max Health:** **100** yaz
5. **Current Health:** **100** yaz
6. **Attack Range:** **1.5** yaz
7. **Attack Cooldown:** **1.0** yaz

---

#### E) Tag Ayarla

1. Inspector'ın **en üstünde** "Tag" dropdown'ı var
2. **"Untagged"** yazıyor, ona tıkla
3. Açılan listeden **"EnemyUnit"** seç

✅ **Kontrol:** Tag kısmında "EnemyUnit" yazmalı

---

**4.3. Prefab'ı Kaydet:**

1. Eğer prefab edit modundaysan:
   - Scene view'ın üstünde **"< "** (geri ok) butonu var
   - Ona tıkla veya **Ctrl+S** bas

2. Eğer Hierarchy'de GameObject oluşturmuşsan:
   - GameObject'i **Project** penceresine **sürükle**
   - **Assets/Burak/Prefabs** klasörüne bırak
   - Prefab oluştu!

---

### Adım 5: Archer (Okçu) Prefab'ı Hazırla

**5.1. Infantry Prefab'ını Kopyala:**

1. Project'te **Enemy_Infantry** prefab'ına **sağ tık**
2. **Duplicate** seç
3. Yeni prefab'ın ismini **"Enemy_Archer"** yap

**5.2. Prefab'ı Aç ve Değiştir:**

1. **Enemy_Archer** prefab'ına **çift tıkla**
2. Inspector'da **Enemy Unit AI** bileşenini bul
3. Sadece şu değerleri değiştir:

```
Unit Type: Archer (dropdown'dan seç)
Attack Range: 4.0 (uzun menzil)
Attack Cooldown: 1.5
```

4. **Sprite Renderer** bileşeninde sprite'ı okçu sprite'ına değiştir
5. **Ctrl+S** ile kaydet

---

### Adım 6: Cavalry (Atlı) Prefab'ı Hazırla

**6.1. Infantry Prefab'ını Kopyala:**

1. **Enemy_Infantry** prefab'ına **sağ tık**
2. **Duplicate** seç
3. İsim: **"Enemy_Cavalry"**

**6.2. Prefab'ı Aç ve Değiştir:**

1. **Enemy_Cavalry** prefab'ına **çift tıkla**
2. Değiştirilecekler:

**NavMeshAgent:**
```
Speed: 5.0 (daha hızlı)
```

**Enemy Unit AI:**
```
Unit Type: Cavalry
Max Health: 120 (daha dayanıklı)
Attack Range: 1.8
Attack Cooldown: 0.8 (daha hızlı saldırı)
```

**Sprite Renderer:**
- Sprite'ı atlı sprite'ına değiştir

3. **Ctrl+S** ile kaydet

✅ **Kontrol:** 3 prefab'ın olmalı:
- Enemy_Infantry
- Enemy_Archer
- Enemy_Cavalry

---

## 👑 COMMANDER KURULUMU

### Adım 7: Commander GameObject Oluştur

**7.1. Boş GameObject Oluştur:**

1. **Hierarchy** penceresinde boş alana **sağ tık**
2. **Create Empty** seç
3. İsim: **"AI Commander"**
4. Position: **(0, 0, 0)**

**7.2. Enemy Commander Scripti Ekle:**

1. **AI Commander** GameObject'i **seçili** olsun
2. Inspector'da **"Add Component"** tıkla
3. **"Enemy Commander"** yaz ve seç

**7.3. Commander Ayarlarını Yap:**

Şimdi Inspector'da **Enemy Commander** bileşenini bulup ayarlayacağız:

---

#### Ayarlar Bölümü:

**Spawn Point:**
1. Alanın sağındaki **hedef simgesine** tıkla
2. Açılan pencereden **"Enemy Spawn"** GameObject'ini seç
3. VEYA: Hierarchy'den **"Enemy Spawn"** GameObject'ini bu alana **sürükle**

**My Castle Transform:**
1. Hedef simgesine tıkla
2. **"Enemy Castle"** seç

**Enemy Castle Transform:**
1. Hedef simgesine tıkla
2. **"Player Castle"** seç

**Decision Interval:**
- **2** yaz (2 saniyede bir karar verir)

**Current Level:**
- **1** yaz (başlangıç seviyesi)

---

#### Ekonomi Bölümü:

```
Current Gold: 100
Income Rate: 10
```

**Detaylı:**
- **Current Gold:** **100** yaz (başlangıç altını)
- **Income Rate:** **10** yaz (saniyede 10 altın kazanır)

---

#### Prefablar Bölümü:

**Infantry Prefab:**
1. Alanın sağındaki **hedef simgesine** tıkla
2. Açılan pencereden **"Enemy_Infantry"** prefab'ını seç
3. VEYA: Project'ten **Enemy_Infantry** prefab'ını bu alana **sürükle**

**Archer Prefab:**
1. **"Enemy_Archer"** prefab'ını seç/sürükle

**Cavalry Prefab:**
1. **"Enemy_Cavalry"** prefab'ını seç/sürükle

---

#### Maliyetler Bölümü:

```
Infantry Cost: 30
Archer Cost: 50
Cavalry Cost: 80
```

**Detaylı:**
- **Infantry Cost:** **30** yaz
- **Archer Cost:** **50** yaz
- **Cavalry Cost:** **80** yaz

---

**7.4. Ayarları Kontrol Et:**

Inspector'da Enemy Commander bileşeninde şunlar dolu olmalı:

✅ **Ayarlar:**
- Spawn Point: Enemy Spawn
- My Castle Transform: Enemy Castle
- Enemy Castle Transform: Player Castle
- Decision Interval: 2
- Current Level: 1

✅ **Ekonomi:**
- Current Gold: 100
- Income Rate: 10

✅ **Prefablar:**
- Infantry Prefab: Enemy_Infantry
- Archer Prefab: Enemy_Archer
- Cavalry Prefab: Enemy_Cavalry

✅ **Maliyetler:**
- Infantry Cost: 30
- Archer Cost: 50
- Cavalry Cost: 80

**SORUN:** Prefablar "None" gösteriyorsa?
- Project'ten prefab'ları tekrar sürükle
- Prefab'ların doğru klasörde olduğundan emin ol

---

## 🎮 TEST VE ÇALIŞTIRMA

### Adım 8: Oyuncu Birimlerini Hazırla

**8.1. Oyuncu Prefab'larına Tag Ata:**

1. **Project** penceresinde oyuncu prefab'larını bul
2. Her birine **tek tek:**
   - Prefab'a **çift tıkla** (edit modu)
   - Inspector'ın üstünde **Tag** dropdown'ı
   - **"PlayerUnit"** seç
   - **Ctrl+S** ile kaydet

✅ **Kontrol:** Tüm oyuncu prefab'larının tag'i "PlayerUnit" olmalı

---

**8.2. Test İçin Oyuncu Birimleri Ekle:**

1. Hierarchy'de **sağ tık** → **Create Empty**
2. İsim: **"Test Units"**
3. Bu GameObject'in altına oyuncu prefab'larından 3-5 tane ekle:
   - Project'ten oyuncu prefab'ını Hierarchy'deki **"Test Units"** üzerine sürükle
   - Position'larını haritanın sol tarafına ayarla (X: -5 ile -8 arası)

---

### Adım 9: Play ve Test

**9.1. Sahneyi Kaydet:**
- **Ctrl+S** bas
- Sahneyi kaydet

**9.2. Play Butonuna Bas:**
- Unity Editor'ün üstünde **Play** (▶️) butonuna tıkla

**9.3. Console'u Aç:**
- Üst menüden **Window** → **General** → **Console**
- Console penceresini aç

---

### Adım 10: Beklenen Davranışlar

✅ **Console Mesajları:**

Şöyle mesajlar görmelisin:

```
[AI Commander] Oyuncu Güçleri: 2 Piyade, 2 Okçu, 1 Atlı
[AI Commander] Cavalry üretildi! Kalan Altın: 20
[AI Commander] Yetersiz altın! İhtiyaç: 80, Mevcut: 20
Enemy_Infantry (Infantry) vurdu: Player_Archer - Hasar: 10
```

✅ **Görsel Davranışlar:**

1. **Commander:**
   - Her 2 saniyede bir birim üretir
   - Altın birikir (saniyede 10)
   - Oyuncu kompozisyonuna göre strateji geliştirir

2. **Düşman Birimleri:**
   - Enemy Spawn noktasında oluşur
   - Oyuncu birimlerine doğru hareket eder
   - Tip avantajı olan hedefleri seçer
   - Yaklaşınca saldırır

3. **NavMesh:**
   - Düşmanlar mavi NavMesh alanı üzerinde hareket eder
   - Engellerin etrafından dolaşır

---

## 🐛 SORUN GİDERME

### SORUN 1: Düşmanlar Spawn Olmuyor

**Kontrol Listesi:**

1. ✅ **Prefab'lar atandı mı?**
   - AI Commander → Inspector → Prefablar bölümü
   - 3 prefab da dolu olmalı

2. ✅ **Spawn Point atandı mı?**
   - AI Commander → Spawn Point
   - "Enemy Spawn" GameObject'i atanmış olmalı

3. ✅ **Yeterli altın var mı?**
   - AI Commander → Current Gold
   - En az 30 olmalı (Infantry için)
   - Veya Income Rate'i artır (örn: 50)

4. ✅ **Console'da hata var mı?**
   - Console penceresini aç
   - Kırmızı hata mesajları varsa oku

**Çözüm:**
- Prefab'ları tekrar ata
- Current Gold'u 500 yap (test için)
- Income Rate'i 50 yap (hızlı altın)

---

### SORUN 2: Düşmanlar Hareket Etmiyor

**Kontrol Listesi:**

1. ✅ **NavMesh bake edildi mi?**
   - Scene view'da mavi alan görünüyor mu?
   - Görünmüyorsa: Zemin GameObject → NavMeshSurface → Bake

2. ✅ **NavMeshAgent ayarları doğru mu?**
   - Prefab'ı aç
   - NavMeshAgent bileşeni
   - **Update Rotation: ❌ KAPALI**
   - **Update Up Axis: ❌ KAPALI**

3. ✅ **Agent referansı atandı mı?**
   - Prefab'ı aç
   - Enemy Unit AI → Agent
   - NavMeshAgent atanmış olmalı

4. ✅ **Castle referansları atandı mı?**
   - AI Commander → My Castle Transform
   - AI Commander → Enemy Castle Transform
   - İkisi de atanmış olmalı

**Çözüm:**
- NavMesh'i tekrar bake et
- NavMeshAgent ayarlarını kontrol et
- Prefab'ları tekrar düzenle

---

### SORUN 3: "PlayerUnit" Tag Bulunamıyor

**Hata Mesajı:**
```
Tag 'PlayerUnit' is not defined
```

**Çözüm:**

1. **Edit** → **Project Settings** → **Tags and Layers**
2. **Tags** bölümünde **"+"** tıkla
3. **"PlayerUnit"** ekle
4. **"EnemyUnit"** ekle
5. Oyuncu prefab'larına **"PlayerUnit"** tag'ini ata
6. Düşman prefab'larına **"EnemyUnit"** tag'ini ata

---

### SORUN 4: Düşmanlar Saldırmıyor

**Kontrol Listesi:**

1. ✅ **Attack Range ayarlandı mı?**
   - Prefab → Enemy Unit AI → Attack Range
   - Infantry: 1.5, Archer: 4.0, Cavalry: 1.8

2. ✅ **PlayerUnit scripti var mı?**
   - Oyuncu prefab'ını aç
   - PlayerUnit scripti olmalı
   - TakeDamage fonksiyonu olmalı

3. ✅ **Hedef bulunuyor mu?**
   - Console'da "vurdu" mesajı görünüyor mu?
   - Görünmüyorsa: Oyuncu birimlerinin tag'i "PlayerUnit" olmalı

**Çözüm:**
- Attack Range'i artır (test için 10 yap)
- Oyuncu birimlerinin tag'ini kontrol et
- Console'daki hata mesajlarını oku

---

### SORUN 5: Z Koordinatı Sorunları

**Belirti:** Birimler görünmüyor veya yanlış yerde

**Çözüm:**

1. **Tüm GameObject'lerin Z pozisyonu 0 olmalı:**
   - Enemy Spawn: Z=0
   - Enemy Castle: Z=0
   - Player Castle: Z=0
   - Prefab'lar: Z=0

2. **NavMeshAgent ayarları:**
   - Update Rotation: ❌ KAPALI
   - Update Up Axis: ❌ KAPALI

3. **Rigidbody2D:**
   - Constraints → Freeze Position Z: ✅ AÇIK

---

### SORUN 6: NavMesh Görünmüyor

**Çözüm:**

1. **Scene view'a geç** (Game view değil!)
2. **Scene view'ın sağ üst köşesinde "Gizmos" düğmesi**
3. **Gizmos'a tıkla**
4. **"NavMesh" seçeneğini bul ve işaretle**
5. **Zemin GameObject → NavMeshSurface → Bake**

---

## ⚙️ İNCE AYAR

### Zorluk Seviyeleri

**Kolay Mod:**
```
AI Commander:
- Current Gold: 200
- Income Rate: 20
- Infantry Cost: 20
- Archer Cost: 30
- Cavalry Cost: 50
- Decision Interval: 3
```

**Normal Mod:**
```
AI Commander:
- Current Gold: 100
- Income Rate: 10
- Infantry Cost: 30
- Archer Cost: 50
- Cavalry Cost: 80
- Decision Interval: 2
```

**Zor Mod:**
```
AI Commander:
- Current Gold: 300
- Income Rate: 30
- Infantry Cost: 25
- Archer Cost: 40
- Cavalry Cost: 60
- Decision Interval: 1
```

---

### Level Sistemi

**Level 1 (Rookie):**
- Current Level: 1
- Düşmanlar normal güçte

**Level 2 (Veteran):**
- Current Level: 2
- Düşmanlar %20 daha güçlü
- Daha akıllı hedef seçimi

**Level 3 (Elite):**
- Current Level: 3
- Düşmanlar %50 daha güçlü
- Canı %10'un altına düşünce geri çekilip iyileşir

---

## ✅ KURULUM TAMAMLANDI!

### Son Kontrol Listesi:

- [ ] Tag'ler oluşturuldu (PlayerUnit, EnemyUnit)
- [ ] Referans noktaları oluşturuldu (Enemy Spawn, Castles)
- [ ] NavMesh bake edildi (mavi alan görünüyor)
- [ ] 3 düşman prefab'ı hazır (Infantry, Archer, Cavalry)
- [ ] Prefab'larda NavMeshAgent ayarları doğru (Update Rotation/Up Axis KAPALI)
- [ ] Prefab'larda EnemyUnitAI scripti var
- [ ] Prefab'ların tag'i "EnemyUnit"
- [ ] AI Commander oluşturuldu
- [ ] Commander'da tüm referanslar atandı
- [ ] Commander'da prefab'lar atandı
- [ ] Oyuncu birimlerinin tag'i "PlayerUnit"
- [ ] Play butonuna basıldı ve test edildi

### Başarı Kriterleri:

✅ Console'da "[AI Commander]" mesajları görünüyor
✅ Düşmanlar spawn oluyor
✅ Düşmanlar hareket ediyor
✅ Düşmanlar saldırıyor
✅ Commander strateji geliştiriyor

---

## 🎉 TEBRİKLER!

AI sisteminiz çalışıyor! Artık:
- Düşmanlar akıllıca hedef seçiyor
- Commander oyuncu kompozisyonunu analiz ediyor
- Karşı strateji geliştiriliyor
- Kaynak yönetimi yapılıyor

**Sonraki Adımlar:**
1. Farklı zorluk seviyelerini test et
2. Level sistemini dene
3. Ayarları optimize et
4. Oyununuza entegre et

İyi oyunlar! 🚀
