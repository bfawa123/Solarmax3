# 同化效果（Assimilation）实现方案

## Summary
在地图配置（mapconfig/*.xml）中新增 `Assimilation` 参数，按 `enable,x,y,camp,n,t` 解析并在战斗中**一次性**触发“同化节点+驻军”效果：当战斗时间到达 `t`（秒）后，指定圆形范围内的所有可同化天体/建筑切换归属到目标势力 `camp`，且该节点内的驻军一起转换为该势力。全程不使用 `HideFly/HidePop` 等隐身能力，保证飞船、数字、天体均可见。

## Current State Analysis
### 配置与读取
- MapConfig 目前会从 `data/mapconfig/{id}.xml` 的 `<map ...>` 根节点读取属性（如 `teamAITypes`、`IfBlackStrike` 等）：[MapConfig.cs](file:///d:/Solarmax3/Assembly-CSharp/Solarmax/MapConfig.cs)
- SceneManager 里已有“按地图/关卡开关触发一次性事件”的框架（BlackStrike）：[SceneManager.cs](file:///d:/Solarmax3/Assembly-CSharp/SceneManager.cs)

### 节点归属与显示
- 节点归属切换入口：`Node.SetRealTeam(Team value, bool initNode=false)`，会更新颜色、人口上限等，并确保节点 alpha=1： [Node.cs](file:///d:/Solarmax3/Assembly-CSharp/Node.cs#L92-L159)
- 现有 `HideFly/HidePop` 会影响节点/数字显示，不符合“全显示”的约束；本功能不依赖这套机制。

### 驻军结构
- 节点内驻军列表结构：`Node.shipArray[(int)TEAM]` / `shipMap`，以及 `numArray` 统计： [Node.cs](file:///d:/Solarmax3/Assembly-CSharp/Node.cs#L478-L569)
- 仅设置 `Ship.currentTeam` 不足以完成阵营转移（需同步节点内的分队列表与统计）。

## Desired Behavior (Locked)
来自用户确认：
- **效果对象**：同化节点 + 驻军（节点归属与节点内驻军一起变为 `camp`）。
- **触发方式**：一次性（单局只触发一次；跨局可再次触发），并支持延迟到指定时间 `t` 后触发。
- **配置格式**：`Assimilation="enable,x,y,camp,n,t"`，`enable` 为 0/1，`x/y` 为世界坐标，`camp` 为势力编号，`n` 为半径（世界坐标单位），`t` 为触发时间（秒，允许 0 表示立刻触发）。
- **显示约束**：飞船、数字、天体都必须显示（不启用 HideFly/HidePop，不通过 alpha 隐藏）。

## Proposed Changes
### 1) MapConfig：新增 Assimilation 配置解析
**文件**： [MapConfig.cs](file:///d:/Solarmax3/Assembly-CSharp/Solarmax/MapConfig.cs)
- 在 `<map ...>` 根节点新增可选属性：`Assimilation="enable,x,y,camp,n,t"`
- 解析规则：
  - 不存在/空字符串：视为未启用
  - 存在时按逗号分割为 6 段，容错空白与浮点格式
  - `enable != 0` 且 `n > 0` 且 `camp` 在有效 TEAM 范围内时启用；`t < 0` 视为 0
- MapConfig 暴露读取口（字段或方法），供 SceneManager 读取：
  - `bool assimilationEnabled`
  - `float assimilationX, assimilationY, assimilationRadius`
  - `float assimilationTriggerTime`
  - `int assimilationCamp`

### 2) Node：新增“同化到目标势力”的安全转移方法
**文件**： [Node.cs](file:///d:/Solarmax3/Assembly-CSharp/Node.cs)
- 新增一个公共方法（或内部 helper）用于同化节点：
  - `void AssimilateTo(Team targetTeam)`
- 行为：
  1. 调用 `SetRealTeam(targetTeam, initNode:false)` 切换节点归属（保持显示）。
  2. 将节点内所有非 `targetTeam` 的驻军迁移到 `targetTeam`：
     - 遍历 `shipArray` 中每个非空列表
     - 对每条 Ship：
       - 从原列表移除，并更新对应 `numArray`
       - `ship.currentTeam = targetTeam`
       - 加入目标队伍列表（必要时创建），并更新目标 `numArray`
  3. 调用节点内部 HUD 刷新（通过 `updateHUD(false)` 或等价路径），确保数字显示正确。
- 约束：
  - 不调用 `HideFly/HidePop`，不改 Ship/Node 的 alpha 到 0。
  - 对 `TEAM.Neutral`、Barrier/Cloud/FixedWarpDoor 等不可同化目标按既有规则跳过（见下一条）。

### 3) SceneManager：按 MapConfig 在开局一次性触发同化
**文件**： [SceneManager.cs](file:///d:/Solarmax3/Assembly-CSharp/SceneManager.cs)
- 在每局 tick 中增加 `TickAssimilation(frame)`（与 `TickBlackStrike(frame)` 并列调用）。
- 触发条件：
  - 从 `MapConfigProvider.GetData(battleData.matchId)` 获取当前地图配置
  - 若 `MapConfig.assimilationEnabled==true` 且 `m_BattleTime >= assimilationTriggerTime` 且当前局 `assimilationFired==false`，则触发一次并置位 `assimilationFired=true`
  - 用“帧回绕”(frame 变小)或 Reset/Destroy 重置 `assimilationFired`（复用 BlackStrike 的跨局重置策略）
- 同化范围：
  - 圆心：`(x,y)`，半径：`n`
  - 扫描节点集合：`nodeManager` 的可用节点列表（推荐 `GetUsefulNodeList()`，并额外过滤 `node.CanBeTarget()` 以跳过 Barrier/Cloud 等不可选目标）
  - 距离判定：`(node.GetPosition() - center).sqrMagnitude <= n*n`
- 同化执行：
  - `Team target = teamManager.GetTeam((TEAM)camp)`
  - 对范围内每个 Node 调用 `node.AssimilateTo(target)`

### 4) 可选：范围圈视觉指示（不影响显示）
**文件**： [EffectManager.cs](file:///d:/Solarmax3/Assembly-CSharp/EffectManager.cs) / 新增 EffectNode
- 若需要复刻截图的“范围圈/领域”效果，推荐复用现有资源：
  - `Entity_Select`（GroundNode 的圈）或 `Entity_Pulse`（PulseNode 的波纹）
- 方案：
  - 新增一个可按世界坐标放置的效果节点（例如 `AssimilationFieldNode : EffectNode`），资源用 `Entity_Select`
  - 初始化时 `transform.position = center`，`scale` 按半径换算到贴图尺寸
  - 生命周期：短暂显示（例如 1~2 秒）或常驻（由 MapConfig 决定）
- 这一部分只有在用户确认需要“视觉圈”时实现；否则同化为纯逻辑效果。

## Assumptions & Decisions
- 同化只影响“节点与节点内驻军”，不主动扫描飞行中的飞船（除非后续需求扩展）。
- 同化为一次性事件：满足配置即开局触发一次，不重复触发。
- `Assimilation` 的坐标系使用 Node/Ship 的世界坐标（与 `Node.GetPosition()` 一致）。
- 不使用任何隐身相关属性（HideFly/HidePop），确保全部可见。

## Verification
### 静态验证
- 运行 C# 诊断检查，确保无编译错误（新增字段/方法引用正确）。

### 手动验证（游戏内）
1. 在目标 `mapconfig/{id}.xml` 的 `<map>` 根节点加入：`Assimilation="1,0,0,7,2.5,10"`（示例：第 10 秒触发）。
2. 开始一局：在第 10 秒左右观察同化范围内的节点颜色应切换为 `camp` 颜色，且节点数字正常显示。
3. 观察同化范围内节点原本的驻军数字应变为 `camp` 的数字颜色，并保持可见。
4. 确认同局不会再次触发；退出再进入新局可再次触发。
