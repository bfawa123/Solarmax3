# 地图切换功能 - 胜利结算问题修复

## 问题描述

在 500117 关卡中，当玩家攻陷敌方城堡时，**直接触发了胜利结算**，而不是预期的**地图切换**效果。

## 问题分析

### 根本原因

1. **默认胜利条件**
   - 500117 关卡配置中未指定 `wintype` 属性
   - 系统使用默认值 `"killall"`（击杀所有）

2. **胜利检查逻辑**
   - `SingleBattleController.UpdateEndAllOccupied()` 每5帧检查一次
   - 调用 `NodeManager.AllOccupied()` 检查所有节点是否被玩家阵营占领
   - 如果返回 `true`，立即触发胜利结算

3. **触发时机**
   - 玩家攻陷敌方城堡 → 触发 `CheckAndTriggerMapSwitch`
   - 地图切换开始 → `isFading = true`，游戏暂停
   - **但** `SingleBattleController.Tick` 仍在运行
   - `UpdateEndAllOccupied` 检查发现所有节点已被占领
   - 触发 `QuitBattle(true)` → 胜利结算

### 代码流程

```
玩家攻陷城堡 (tag="6")
    ↓
Node.Captured() [Node.cs:2364]
    ↓
SceneManager.CheckAndTriggerMapSwitch()
    ↓ [通过所有检查]
    ↓
SceneManager.SwitchMapAndMusic()
    ↓
SceneManager.isFading = true
SceneManager.mapSwitched = true
BattleSystem.SetPause(true)
    ↓ [游戏暂停，但 SingleBattleController.Tick 仍在运行]
    ↓
SingleBattleController.Tick() 继续执行
    ↓
UpdateEndAllOccupied() 检查 AllOccupied()
    ↓ [发现所有节点被占领]
    ↓
QuitBattle(true) → 胜利结算 ❌
```

## 解决方案

在地图切换期间（`isFading = true`），暂停胜利条件检查。

### 修复内容

#### 文件：`SingleBattleController.cs`

**修复 1：`UpdateEndAllOccupied` 方法**

**位置**：第 371-389 行

**修改前**：
```csharp
private void UpdateEndAllOccupied(int frame, float interval)
{
    if (++this.tickEndCount == 5)
    {
        if (Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.AllOccupied((int)this.battleData.currentTeam))
        {
            this.battleData.winTEAM = this.battleData.currentTeam;
            this.QuitBattle(true);
            return;
        }
        this.tickEndCount = 0;
    }
}
```

**修改后**：
```csharp
private void UpdateEndAllOccupied(int frame, float interval)
{
    // 地图切换期间不检查胜利条件
    if (Solarmax.Singleton<BattleSystem>.Instance.sceneManager.isFading)
    {
        return;
    }

    if (++this.tickEndCount == 5)
    {
        if (Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.AllOccupied((int)this.battleData.currentTeam))
        {
            this.battleData.winTEAM = this.battleData.currentTeam;
            this.QuitBattle(true);
            return;
        }
        this.tickEndCount = 0;
    }
}
```

**修复 2：`UpdateEndOtherWinLoseType` 方法**

**位置**：第 391-393 行

**修改前**：
```csharp
private void UpdateEndOtherWinLoseType(int frame, float interval)
{
    if (++this.tickEndOtherCount == 5)
    {
```

**修改后**：
```csharp
private void UpdateEndOtherWinLoseType(int frame, float interval)
{
    // 地图切换期间不检查胜利条件
    if (Solarmax.Singleton<BattleSystem>.Instance.sceneManager.isFading)
    {
        return;
    }

    if (++this.tickEndOtherCount == 5)
    {
```

## isFading 状态机

### 状态变化

```
isFading = false (正常游戏)
    ↓ [攻陷城堡，触发切换]
    ↓
SwitchMapAndMusic()
    ↓
isFading = true, mapSwitched = true
    ↓ [渐白完成]
    ↓
SwitchToInsideMap() [切换地图]
    ↓ [暂停 0.5 秒]
    ↓
isFading = false [渐回开始]
    ↓ [渐回完成]
    ↓
isFading = false (恢复正常)
```

### 检查点

在以下时间点，胜利检查会被暂停：
1. ✅ 渐白阶段（屏幕变白）
2. ✅ 地图切换阶段（加载新地图）
3. ✅ 短暂停留阶段（0.5秒）
4. ✅ 渐回阶段（屏幕恢复）

### 恢复检查

渐回完成后（`isFading = false`），胜利条件检查恢复正常。

## 修复验证

### 测试场景 1：500117 关卡

**预期行为**：
- 玩家攻陷城堡 (tag="6") → 触发地图切换
- 渐白屏幕 → 切换到 500117.1 地图
- 播放音乐 "射程外的神祇.ogg"
- 渐回屏幕 → 继续游戏
- ❌ **不触发**胜利结算

**修复后行为**：
- ✅ 地图切换正常进行
- ✅ 无胜利结算弹出

### 测试场景 2：其他关卡（无地图切换）

**预期行为**：
- 攻陷所有敌方节点 → 触发胜利结算
- ✅ 不受影响（`isFading` 始终为 `false`）

### 测试场景 3：快速攻陷

**预期行为**：
- 快速连续攻陷多个节点 → 地图切换正常
- ✅ `isFading` 标志正确防止重复触发和提前结算

## 技术细节

### 为什么选择检查 `isFading`？

1. **语义明确**：`isFading` 表示地图切换过渡期间
2. **状态可靠**：在 `SwitchMapAndMusic()` 开始时设为 `true`，在 `UnfadeScreenCoroutine()` 结束时设为 `false`
3. **覆盖全面**：包括渐白、地图切换、暂停、渐回所有阶段

### 为什么不修改胜利条件？

原方案：
- 修改 500117 关卡的 `wintype` 为其他类型（如 "occupy"、"alive"）

优点：
- 简单直接

缺点：
- 影响关卡设计灵活性
- 不同关卡可能需要不同胜利条件
- 修改配置不解决根本问题

**选择方案**：在地图切换期间暂停胜利检查，保持关卡配置的通用性。

### 为什么不停止 SingleBattleController.Tick？

原方案：
- 在 `SwitchToInsideMap` 中暂停 `SingleBattleController`

缺点：
- 过度设计
- 需要额外的暂停/恢复逻辑
- 可能导致其他功能异常

**选择方案**：添加简单的 `isFading` 检查，最小化代码修改。

## 相关文件

### 修改文件

1. **Assembly-CSharp\SingleBattleController.cs**
   - `UpdateEndAllOccupied()` - 添加地图切换检查
   - `UpdateEndOtherWinLoseType()` - 添加地图切换检查

2. **Assembly-CSharp\SceneManager.cs**（已修改）
   - `isFading` 字段 - 状态标志
   - `SwitchMapAndMusic()` - 设置 `isFading = true`
   - `UnfadeScreenCoroutine()` - 设置 `isFading = false`

3. **Assembly-CSharp\Solarmax\MapConfig.cs**（已修改）
   - 添加 `the_end` 支持

### 配置文件

1. **level.xml** (第 1195 行)
   ```xml
   <level id="500117"
          map="500117"
          inside_map="500117.1"
          inside_musicName="射程外的神祇.ogg"
          wintype="killall"  <!-- 默认值，未显式配置 -->
          ... />
   ```

2. **500117.xml**
   ```xml
   <map id="500117" The_end="6">
     <mapbuilding type="castle" tag="6" ... />
   </map>
   ```

## 完整修复清单

### 地图切换功能修复

| # | 文件 | 修改内容 | 状态 |
|---|------|---------|------|
| 1 | MapConfig.cs | 添加 `the_end` 字段 | ✅ |
| 2 | SceneManager.cs | 添加 `using System.Collections` | ✅ |
| 3 | SceneManager.cs | 修复触发逻辑支持队友AI | ✅ |
| 4 | SceneManager.cs | 修复 The_end 检查 | ✅ |
| 5 | SceneManager.cs | 修复音乐控制方法 | ✅ |
| 6 | SceneManager.cs | 修复协程调用 | ✅ |
| 7 | SceneManager.cs | 修复白色遮罩实现 | ✅ |
| 8 | SceneManager.cs | 修复方法闭合括号 | ✅ |
| 9 | **SingleBattleController.cs** | **暂停地图切换期间胜利检查** | ✅ **新增** |

### 胜利条件检查修复

| # | 方法 | 修改内容 | 状态 |
|---|------|---------|------|
| 1 | `UpdateEndAllOccupied()` | 添加 `isFading` 检查 | ✅ |
| 2 | `UpdateEndOtherWinLoseType()` | 添加 `isFading` 检查 | ✅ |

---

## 总结

**问题**：地图切换期间，胜利条件检查仍然运行，导致提前触发胜利结算。

**解决**：在 `SingleBattleController` 的两个胜利检查方法中添加 `isFading` 检查，在地图切换过渡期间暂停胜利判定。

**效果**：
- ✅ 攻陷城堡 → 触发地图切换（不触发胜利）
- ✅ 地图切换完成后 → 继续正常游戏
- ✅ 其他关卡 → 不受影响
