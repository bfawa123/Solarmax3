# 场景地图切换设计文档

## 功能概述

在关卡进行中,当玩家破坏/攻陷敌方带有特定标记(`The end`)的天体时,触发地图和音乐的无缝切换。

## 实现方案

### 1. 修改 LevelConfig.cs
添加 `inside_map` 和 `inside_musicName` 字段,用于配置第二地图和第二音乐。

### 2. 修改 Node.cs
在节点被攻陷(Captured)时,调用 SceneManager 的检查方法。

### 3. 修改 SceneManager.cs
添加以下功能:
- `CheckAndTriggerMapSwitch(Node node, Team previousOwner)` - 检查是否需要切换地图
- `SwitchMapAndMusic(LevelConfig levelConfig)` - 执行地图和音乐切换
- `FadeScreenCoroutine()` - 渐白屏幕协程
- `UnfadeScreenCoroutine()` - 渐回屏幕协程
- `SwitchToInsideMap(LevelConfig levelConfig)` - 执行地图和音乐切换
- `UpdateWhiteOverlay(float alpha)` - 更新白色遮罩层
- `SetInteractionEnabled(bool enabled)` - 控制交互

## 触发条件

1. 玩家破坏/攻陷敌方建筑节点
2. 被破坏的节点 tag 为 "The end"
3. 当前 LevelConfig 配置了 inside_map 和 inside_musicName

## 切换流程

1. **触发瞬间**: 游戏继续运行
2. **开始渐白**: 屏幕逐渐变白(背景层,不影响UI)
3. **渐白完成**: 暂停游戏
4. **切换地图**: 加载新地图,切换音乐
5. **短暂停留**: 暂停0.5秒让玩家看到新地图
6. **恢复游戏**: 解除暂停
7. **渐回屏幕**: 屏幕逐渐恢复
8. **切换完成**: 恢复玩家交互

## 关键特性

- ✅ **渐白不遮挡UI**: 白色遮罩只在背景层,战斗UI仍然可见
- ✅ **游戏暂停**: 渐白完成后自动暂停,渐回开始前自动恢复
- ✅ **平滑过渡**: 渐白/渐回过程流畅,提供视觉缓冲
- ✅ **音乐切换**: 暂停期间切换音乐,恢复后播放新音乐
- ✅ **输入控制**: 渐白期间禁用交互,渐回后恢复

## 注意事项

- 切换只触发一次 (防止重复触发)
- 切换期间游戏暂停,玩家不能进行操作
- 暂停时间约0.5秒,让玩家看到新地图
- 渐白效果不影响UI层,保证玩家可以看到游戏状态
- 确保 `inside_map` 对应的地图文件存在
- 确保 `inside_musicName` 对应的音频文件存在

## 代码位置

- LevelConfig.cs: 167-169 行 (insideMap, insideMusicName 字段)
- Node.cs: 2362 行 (调用 CheckAndTriggerMapSwitch)
- SceneManager.cs: 1762-1950 行 (地图切换逻辑)
