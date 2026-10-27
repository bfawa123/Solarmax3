# 地图切换功能修复总结

## 修复日期
2026-08-28

## 问题描述
在实现地图切换功能时出现了多个编译错误，导致功能无法正常运行。

## 修复内容

### 1️⃣ MapConfig.cs - 添加 The_end 支持

**位置**: `Assembly-CSharp\Solarmax\MapConfig.cs`

**修改**:
- **第 182/447 行**: 添加读取 `The_end` 属性
  ```csharp
  this.the_end = xelement.GetAttribute("The_end", string.Empty);
  ```

- **第 932 行**: 添加字段声明
  ```csharp
  public string the_end;
  ```

**原因**: MapConfig 缺少 `The_end` 字段，无法读取地图配置中的触发节点标记。

---

### 2️⃣ SceneManager.cs - 核心功能修复

**位置**: `Assembly-CSharp\SceneManager.cs`

#### ✅ 修复 2.1: 添加缺失的命名空间

**第 2 行**:
```csharp
using System.Collections;  // 添加这一行
```

**原因**: `IEnumerator`（协程使用）位于 `System.Collections` 命名空间，而不是 `System.Collections.Generic`。

#### ✅ 修复 2.2: 修复触发逻辑

**第 1770-1774 行** (修改前):
```csharp
// 检查节点的阵营是否从敌方变为中立(即被玩家攻陷)
Team playerTeam = this.teamManager.GetTeam(this.battleData.currentTeam);
if (previousOwner.team == playerTeam.team)
{
    return; // 不是玩家攻陷的,忽略
}
```

**第 1770-1774 行** (修改后):
```csharp
// 检查节点是否被玩家阵营攻陷（包括玩家和队友AI）
Team playerTeam = this.teamManager.GetTeam(this.battleData.currentTeam);
if (node.currentTeam == null || !playerTeam.IsFriend(node.currentTeam.groupID))
{
    return; // 不是玩家阵营攻陷的,忽略
}
```

**原因**:
- 原逻辑只检查"攻陷前的阵营不是玩家"
- 新逻辑检查"攻陷后的阵营属于玩家阵营（包括队友AI）"
- 使用 `IsFriend()` 方法判断阵营关系，支持合作模式

#### ✅ 修复 2.3: 修复 The_end 检查

**第 1776-1781 行** (修改前):
```csharp
// 检查节点是否有 "The end" 标签
if (node.tag != "The end")
{
    return; // 不是触发节点
}
```

**第 1776-1781 行** (修改后):
```csharp
// 检查节点是否有 "The end" 标签
MapConfig mapConfig = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(this.battleData.matchId);
if (mapConfig == null || string.IsNullOrEmpty(mapConfig.the_end) || node.tag != mapConfig.the_end)
{
    return; // 不是触发节点
}
```

**原因**:
- 原逻辑硬编码检查 `"The end"` 字符串
- 新逻辑从 MapConfig 读取实际配置的 `The_end` 值
- 支持不同地图配置不同的触发节点

#### ✅ 修复 2.4: 修复音乐停止方法

**第 1807 行** (修改前):
```csharp
Solarmax.Singleton<AudioManger>.Get().StopAudioBG();
```

**第 1807 行** (修改后):
```csharp
Solarmax.Singleton<AudioManger>.Get().MuteBGAudio(true);
```

**原因**: `AudioManger` 类没有 `StopAudioBG()` 方法，只有 `MuteBGAudio()` 方法。

#### ✅ 修复 2.5: 修复协程调用

**第 1810/1838 行** (修改前):
```csharp
this.StartCoroutine(this.FadeScreenCoroutine(levelConfig));
StartCoroutine(this.UnfadeScreenCoroutine());
```

**第 1810/1838 行** (修改后):
```csharp
Coroutine.Start(this.FadeScreenCoroutine(levelConfig));
Coroutine.Start(this.UnfadeScreenCoroutine());
```

**原因**: `SceneManager` 继承自 `Lifecycle2`，不是 `MonoBehaviour`，不能直接调用 `StartCoroutine`。需要使用自定义的 `Coroutine.Start()` 方法。

#### ✅ 修复 2.6: 修复白色遮罩实现

**第 1895-1923 行** (修改前):
```csharp
// 创建一个全屏的白色面板用于渐白效果
// 这个面板应该在不阻挡UI的情况下覆盖背景
// 实现方式:创建一个Canvas,设置为只在Background层渲染
if (this.whiteOverlay == null)
{
    GameObject overlayObj = new GameObject("MapSwitchWhiteOverlay");
    overlayObj.transform.SetParent(this.transform, false);

    // 设置为背景层
    overlayObj.layer = LayerMask.NameToLayer("Invisible");

    // 添加UI组件
    Canvas canvas = overlayObj.AddComponent<Canvas>();
    canvas.renderMode = RenderMode.ScreenSpaceCamera;
    canvas.planeDistance = 100;

    UIImage image = overlayObj.AddComponent<UIImage>();
    image.color = new Color(1f, 1f, 1f, alpha);
    image.type = UIImage.Type.Sliced;

    this.whiteOverlay = overlayObj;
}
else
{
    UIImage image = this.whiteOverlay.GetComponent<UIImage>();
    if (image != null)
    {
        image.color = new Color(1f, 1f, 1f, alpha);
    }
}
```

**第 1895-1923 行** (修改后):
```csharp
// 创建一个简单的白色平面用于渐白效果
if (this.whiteOverlay == null)
{
    // 创建一个简单的白色平面
    GameObject overlayObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
    overlayObj.name = "MapSwitchWhiteOverlay";

    // 设置材质为白色
    Renderer renderer = overlayObj.GetComponent<Renderer>();
    if (renderer != null)
    {
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.material.color = new Color(1f, 1f, 1f, alpha);
    }

    // 设置为背景层
    overlayObj.layer = LayerMask.NameToLayer("Invisible");

    this.whiteOverlay = overlayObj;
}
else
{
    Renderer renderer = this.whiteOverlay.GetComponent<Renderer>();
    if (renderer != null && renderer.material != null)
    {
        renderer.material.color = new Color(1f, 1f, 1f, alpha);
    }
}
```

**原因**:
- 原实现使用 `UIImage` 组件（不存在或需要额外引用）
- 新实现使用 `GameObject.CreatePrimitive(PrimitiveType.Quad)` 创建简单平面
- 使用 `Renderer` 和 `Material` 设置颜色，更简洁可靠

#### ✅ 修复 2.7: 修复方法闭合

**第 1923 行**: 添加缺失的方法闭合大括号
```csharp
}
```

**原因**: 在替换代码时遗漏了方法的闭合括号，导致后续代码语法错误。

---

## 最终功能

### 触发条件

当地图配置满足以下条件时：

**level.xml 配置**:
```xml
<level ... inside_map="500117.1" inside_musicName="射程外的神祇.ogg" />
```

**500117.xml 配置**:
```xml
<map id="500117" ... The_end="6">
  <mapbuildings>
    <mapbuilding type="castle" ... tag="6" ... />
  </mapbuildings>
</map>
```

**触发场景**:
- ✅ **玩家攻陷城堡** (tag="6") → 触发地图切换
- ✅ **玩家队友AI攻陷城堡** (tag="6") → 触发地图切换
- ❌ **敌方攻陷城堡** → 不触发
- ❌ **攻陷其他节点** (星星、塔等) → 不触发

### 切换流程

1. **触发瞬间**: 游戏继续运行
2. **暂停游戏**: `SetPause(true)`
3. **静音音乐**: `MuteBGAudio(true)`
4. **渐白屏幕**: 通过协程渐变白色遮罩 (2秒)
5. **切换地图**: 加载 `inside_map` 配置的地图
6. **切换音乐**: 播放 `inside_musicName` 配置的音乐
7. **短暂停留**: 暂停 0.5 秒让玩家看到新地图
8. **恢复游戏**: `SetPause(false)`
9. **渐回屏幕**: 通过协程渐变恢复 (2秒)
10. **恢复交互**: `SetInteractionEnabled(true)`

### 技术细节

- **协程管理**: 使用 `Coroutine.Start()` 而非 `MonoBehaviour.StartCoroutine()`
- **阵营判断**: 使用 `Team.IsFriend()` 支持合作模式
- **节点识别**: 从 `MapConfig` 动态读取 `The_end` 配置
- **视觉效果**: 使用 `PrimitiveType.Quad` + `Material` 实现白色遮罩
- **音频控制**: 使用 `MuteBGAudio()` 暂停/恢复背景音乐

---

## 测试建议

### 测试场景 1: 单玩家模式
- [ ] 攻陷标记为 `The_end` 的城堡 → 应该触发切换
- [ ] 攻陷其他节点 → 不应该触发切换

### 测试场景 2: 双人合作模式
- [ ] 玩家1 攻陷城堡 → 应该触发切换
- [ ] 玩家2 攻陷城堡 → 应该触发切换
- [ ] 任意玩家AI队友攻陷城堡 → 应该触发切换
- [ ] 敌方攻陷城堡 → 不应该触发切换

### 测试场景 3: 视觉效果
- [ ] 渐白效果平滑，不卡顿
- [ ] 白色遮罩不遮挡UI
- [ ] 渐回效果平滑
- [ ] 地图切换期间游戏暂停
- [ ] 音乐切换正确

### 测试场景 4: 边界情况
- [ ] 多次攻陷城堡 → 不应该重复触发
- [ ] 快速攻陷 → 切换流程正常
- [ ] 切换后游戏状态正常

---

## 相关文件

- `Assembly-CSharp\Solarmax\MapConfig.cs` - 地图配置类
- `Assembly-CSharp\Solarmax\LevelConfig.cs` - 关卡配置类
- `Assembly-CSharp\SceneManager.cs` - 场景管理器（主要修改）
- `Assembly-CSharp\Node.cs` - 节点类（调用触发检查）
- `Assembly-CSharp\Coroutine.cs` - 协程管理器
- `Assembly-CSharp\AudioManger.cs` - 音频管理器

---

## 配置文件示例

### level.xml
```xml
<level chapter="5001" levelname="500117" levelGroup="500117" id="500117"
       map="500117" inside_map="500117.1" inside_musicName="射程外的神祇.ogg"
       ... />
```

### 500117.xml (第一地图)
```xml
<map id="500117" player_count="2" The_end="6">
  <mapbuildings>
    <mapbuilding type="castle" tag="6" ... />
    <mapbuilding type="star" tag="0" ... />
    ...
  </mapbuildings>
</map>
```

### 500117.1.xml (第二地图)
```xml
<map id="500117.1" player_count="2" CoopAccelerateEnable="0">
  <mapbuildings>
    <mapbuilding type="castle" tag="16" ... />
    ...
  </mapbuildings>
  <mapplayers>
    <mapplayer tag="2" ship="40" camption="1" />
    <mapplayer tag="16" ship="10" camption="7" />
  </mapplayers>
</map>
```
