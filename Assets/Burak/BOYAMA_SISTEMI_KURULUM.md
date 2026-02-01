# BOYAMA SİSTEMİ KURULUM REHBERİ

Bu rehber, düşmanların geçemeyeceği boyalı alanlar oluşturma sisteminin sıfırdan kurulumunu anlatır.

---

## ADIM 1: TILEMAP OLUŞTURMA

### 1.1. Grid Oluştur
1. Hierarchy'de sağ tık → **2D Object** → **Tilemap** → **Rectangular**
2. Otomatik olarak "Grid" ve "Tilemap" objeleri oluşur
3. "Tilemap" objesini seç ve ismini **"ObstacleTilemap"** olarak değiştir

### 1.2. Tilemap Ayarları
ObstacleTilemap objesini seç:
- **Tilemap Renderer** bileşeninde:
  - **Order in Layer**: `5` (Zemin ve karakterlerin üstünde görünsün)
  - **Sorting Layer**: Default (veya istediğin layer)

---

## ADIM 2: TILE ASSET OLUŞTURMA

### 2.1. Tile Sprite Hazırla
1. Project penceresinde boyama için kullanacağın sprite'ı bul
   - Yoksa basit bir kare sprite oluştur (örn: 32x32 piksel kırmızı kare)
2. Sprite'ı seç ve Inspector'da:
   - **Texture Type**: Sprite (2D and UI)
   - **Pixels Per Unit**: 32 (veya grid boyutuna göre ayarla)
   - **Apply** butonuna tıkla

### 2.2. Tile Asset Oluştur
1. Project penceresinde sağ tık → **Create** → **2D** → **Tiles** → **Rule Tile** (veya Tile)
2. İsmini **"WallTile"** koy
3. WallTile'ı seç ve Inspector'da:
   - **Sprite** alanına hazırladığın sprite'ı sürükle
   - **Collider Type**: Sprite (önemli!)

---

## ADIM 3: TILEMAP'E COLLIDER EKLEME

### 3.1. Tilemap Collider 2D Ekle
ObstacleTilemap objesini seç:
1. Inspector'da **Add Component** butonuna tıkla
2. **Tilemap Collider 2D** yaz ve ekle

### 3.2. Composite Collider 2D Ekle (Opsiyonel ama Önerilen - Performans İçin)
**Not:** Bu adım opsiyonel ama çok sayıda tile için performansı artırır.

Aynı objede:
1. **Add Component** → **Composite Collider 2D**
2. Otomatik olarak **Rigidbody 2D** de eklenir
3. Tilemap Collider 2D'de otomatik olarak ✅ **Used By Composite** işaretlenir
4. Rigidbody 2D ayarları:
   - **Body Type**: Static
   - **Simulated**: ✅ İşaretli

**Composite Collider 2D eklemediysen:**
- Sadece Tilemap Collider 2D yeterli
- Her tile ayrı collider olur (daha fazla performans maliyeti)
- Küçük projeler için sorun olmaz

---

## ADIM 4: LAYER AYARLARI

### 4.1. Yeni Layer Oluştur
1. Unity'nin sağ üst köşesinde **Layers** dropdown → **Edit Layers**
2. Boş bir User Layer'a **"Obstacles"** ismini ver

### 4.2. Tilemap'e Layer Ata
ObstacleTilemap objesini seç:
- Inspector'ın üst kısmında **Layer** dropdown → **Obstacles** seç

---

## ADIM 5: NAVMESH AYARLARI

### 5.1. NavMeshSurface Bul veya Oluştur
**Eğer sahnede NavMeshSurface varsa:**
- Hierarchy'de bul (genellikle "NavMesh" veya "Ground" isimli objede)

**Eğer yoksa oluştur:**
1. Hierarchy'de sağ tık → **Create Empty**
2. İsmini **"NavMesh"** koy
3. Inspector'da **Add Component** → **NavMeshSurface** (NavMeshPlus'tan)

### 5.2. NavMeshSurface Ayarları
NavMeshSurface bileşenini seç:
- **Agent Type**: Humanoid
- **Collect Objects**: All
- **Include Layers**: 
  - ✅ Default (zemin için)
  - ✅ Obstacles (boyalı alanlar için)
- **Use Geometry**: Physics Colliders (önemli!)
- **Override Voxel Size**: ✅ İşaretle
- **Voxel Size**: 0.1 (daha hassas NavMesh için)

### 5.3. İlk NavMesh Bake
- NavMeshSurface bileşeninde **Bake** butonuna tıkla
- Mavi NavMesh görünmeli (Scene view'da)

---

## ADIM 6: TILEMAPPAINTER SCRİPTİNİ EKLEME

### 6.1. Script Objesi Oluştur
1. Hierarchy'de sağ tık → **Create Empty**
2. İsmini **"TilemapPainter"** koy

### 6.2. Script Bileşenini Ekle
TilemapPainter objesini seç:
1. Inspector'da **Add Component**
2. **TilemapPainter** scriptini bul ve ekle

### 6.3. Script Referanslarını Ata
TilemapPainter objesinde Inspector'da:

**Game Manager:**
- Hierarchy'den **GameManager** objesini sürükle

**Tilemap Ayarlari:**
- **Obstacle Tilemap**: ObstacleTilemap objesini sürükle
- **Wall Tile**: Project'ten WallTile asset'ini sürükle
- **Main Camera**: Hierarchy'den Main Camera'yı sürükle (otomatik bulunur)

**NavMesh Ayarlari:**
- **Nav Mesh Surface**: NavMeshSurface olan objeyi sürükle

**Murekkep Ayarlari:**
- **Cost Per Tile**: 1 (her kare 1 gold)

**Firca Ayarlari:**
- **Brush Radius**: 2 (fırça boyutu)

**Imlec Ayarlari:**
- **Brush Cursor**: Fırça için cursor sprite (opsiyonel)
- **Cursor Hotspot**: (0, 0)

**Yasam Suresi Ayarlari:**
- **Tile Lifetime**: 30 (saniye)
- **Fade Duration**: 5 (saniye)

---

## ADIM 7: GAMEMANAGER KONTROLÜ

### 7.1. GameManager Ayarları
GameManager objesini seç ve kontrol et:
- **Starting Money**: 1000 (veya istediğin miktar)
- Script'in aktif olduğundan emin ol

---

## ADIM 8: TEST ETME

### 8.1. Oyunu Başlat
Play butonuna tıkla

### 8.2. Boyama Modunu Aç
- **B** tuşuna bas
- Cursor değişmeli (eğer brush cursor atadıysan)
- Console'da "Paint Mode: True" yazmalı

### 8.3. Boyama Yap
- **Sol tık** ile boyama yap
- Console'da "X kare oluşturuldu!" yazmalı
- Tilemap'te renkli kareler görünmeli
- Para azalmalı (her kare 1 gold)

### 8.4. NavMesh Kontrolü
- Scene view'da NavMesh'i görmek için:
  - Window → AI → Navigation
  - Scene view'da mavi NavMesh görünmeli
  - Boyalı alanlar NavMesh'ten çıkarılmalı (mavi olmamalı)

### 8.5. Düşman Testi
- Düşman spawn et
- Düşman boyalı alanlardan geçememelidir
- Boyalı alanların etrafından dolaşmalıdır

---

## SORUN GİDERME

### Boyama Yapamıyorum
**Console'da ne yazıyor?**

❌ **"GameManager referansi atanmamis!"**
→ TilemapPainter'a GameManager objesini ata

❌ **"Para yok! Mevcut: 0"**
→ GameManager.startingMoney değerini artır

❌ **"obstacleTilemap atanmamis!"**
→ TilemapPainter'a ObstacleTilemap'i ata

❌ **"wallTile atanmamis!"**
→ TilemapPainter'a WallTile asset'ini ata

❌ **"Mouse UI uzerinde"**
→ UI'dan uzaklaş ve tekrar dene

### Düşmanlar Boyalı Alandan Geçiyor
**Kontrol Listesi:**

1. ✅ ObstacleTilemap'te **Tilemap Collider 2D** var mı?
2. ✅ ObstacleTilemap'in **Layer'ı Obstacles** mı?
3. ✅ NavMeshSurface'in **Include Layers**'ında Obstacles var mı?
4. ✅ NavMeshSurface'in **Use Geometry** ayarı **Physics Colliders** mı?
5. ✅ TilemapPainter'a **NavMeshSurface** referansı atandı mı?
6. ✅ Console'da **"NavMesh guncellendi!"** yazıyor mu?

**Hala çalışmıyorsa:**
- NavMeshSurface objesini seç
- Inspector'da **Clear** butonuna tıkla
- Sonra **Bake** butonuna tıkla
- Oyunu yeniden başlat

### Performans Sorunu (Yavaşlık)
NavMesh her tile'da güncelleniyor, bu yavaşlığa sebep olabilir.

**Çözüm: Batch Update**
TilemapPainter.cs'de CreateTile() fonksiyonundaki:
```csharp
UpdateNavMesh();
```
satırını kaldır ve Paint() fonksiyonunun sonuna ekle:
```csharp
if (tilesCreated > 0)
{
    UpdateNavMesh(); // Tüm tile'lar oluşturulduktan sonra bir kez güncelle
}
```

### Tile'lar Görünmüyor
1. ObstacleTilemap'in **Order in Layer** değerini artır (örn: 10)
2. WallTile asset'inde sprite atandığından emin ol
3. Camera'nın Tilemap'i görebildiğinden emin ol

---

## EK ÖZELLİKLER

### Fırça Boyutunu Değiştirme
TilemapPainter'da:
- **Brush Radius**: 1 (küçük fırça)
- **Brush Radius**: 3 (büyük fırça)

### Tile Ömrünü Değiştirme
TilemapPainter'da:
- **Tile Lifetime**: 60 (1 dakika)
- **Fade Duration**: 10 (10 saniye solma)

### Fırça Cursor'u Ekleme
1. Project'te bir cursor sprite hazırla (örn: 32x32 fırça ikonu)
2. TilemapPainter'da **Brush Cursor** alanına sürükle
3. B tuşuna basınca cursor değişecek

---

## KISA YOLLAR

**Oyun İçi:**
- **B**: Boyama modunu aç/kapat
- **Sol Tık**: Boyama yap
- **Sağ Tık**: Boyama modundan çık

**Unity Editor:**
- **Scene View'da NavMesh Göster**: Window → AI → Navigation

---

## ÖZET KONTROL LİSTESİ

Kurulum tamamlandığında şunlar olmalı:

✅ ObstacleTilemap objesi var
✅ ObstacleTilemap'te Tilemap Collider 2D var
✅ ObstacleTilemap'in Layer'ı Obstacles
✅ WallTile asset'i oluşturuldu
✅ NavMeshSurface objesi var ve ayarlandı
✅ NavMeshSurface bake edildi
✅ TilemapPainter objesi var
✅ TilemapPainter'a tüm referanslar atandı
✅ GameManager'da para var (startingMoney > 0)
✅ B tuşu ile boyama modu açılıyor
✅ Sol tık ile boyama yapılıyor
✅ Düşmanlar boyalı alanlardan geçemiyor

---

## BAŞARILAR! 🎨

Artık boyama sistemi çalışıyor! Düşmanları engellemek için stratejik alanlar boyayabilirsin.

**İpucu:** Para yönetimi önemli! Her kare 1 gold harcıyor, bu yüzden akıllıca boyama yap! 💰
