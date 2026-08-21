# TurnDemo — Unity 回合制 RPG 客户端框架与战斗 Demo

一个从零独立开发的 Unity 回合制 RPG 项目。
HybridCLR 热更新、DI 依赖注入、事件中心、AssetBundle 打包与断点续传更新、MVC/MVVM UI 框架、
实体组件（EC）架构、TCP + 帧同步网络层，以及在此框架之上实现的回合制战斗与背包/任务/对话等业务系统。

> 开发环境：Unity 2022.3 LTS（2022.3.57f1c2）｜ C# ｜ 独立开发

---

## 🖼️ 项目截图

<!-- 替换成你的实际截图：战斗画面、背包界面、任务界面、主界面各放一张即可 -->

| 战斗 | 背包 |
| --- | --- |
| ![战斗截图](docs/battle.png) | ![背包截图](docs/inventory.png) |

| 任务 | 主界面 |
| --- | --- |
| ![任务截图](docs/quest.png) | ![主界面截图](docs/main.png) |

> 完整源码见本仓库，各系统逻辑可直接阅读。

---

## 框架层（Assets/Scripts/Core，可独立复用的客户端框架）

这套框架按"框架（不热更）+ 业务（热更）"分层，核心模块及其设计考量如下：

| 模块 | 做了什么 | 原因 |
| --- | --- | --- |
| **热更新（HybridCLR）** | `HotUpdateManager` 运行时加载 hotfix DLL 与 AOT 元数据；业务拆到 `HotUpdate.Base / Common / Entry / Game / UI` 五个热更程序集 | 框架层保持稳定不动，只热更业务，降低热更风险 |
| **DI 依赖注入** | 手写 `DIContainer`：单例/瞬态绑定、构造/字段注入（`[Inject]`）、构造缓存、解析栈 | 为深入理解注入原理手写（未引入 Zenject/VContainer），减少对重型框架的依赖 |
| **事件中心** | `EventCenter` 订阅/取消/触发/延迟触发，支持按类型过滤 | 限制单帧触发上限 + 递归深度保护，防止战斗/UI 事件风暴造成帧率抖动 |
| **资源管理** | `AssetManager` / `AssetBundleManager`，句柄化（`AssetHandle`）生命周期管理，LFU 滑动窗口缓存 | 句柄化引用计数管理生命周期，避免资源泄漏与重复加载 |
| **AB 打包与更新** | 编辑器一键打包（依赖解析、Key 生成、差异对比、上传）；运行时 `UpdateService` 状态机流程，HTTP Range 断点续传 | 更新拆成独立状态（校验→对比→下载目录→下载资源），每步可单独重试/回退，异常可恢复 |
| **网络层** | TCP：ID+长度消息头解决分包/粘包，心跳保活，消息工厂分发（已在登录流程实际连接服务器）；帧同步：搭好 UDP 收发 + 帧命令框架 | 帧同步核心同步逻辑尚未接入业务，如实标注实现程度，避免误导 |
| **UI 框架** | `UIManager` 分层（`E_UILayer`），MVC（`UIController`/`UIView`）+ MVVM（`ReactiveProperty<T>` 响应式驱动），反射控件绑定 | 数据驱动响应式更新，数据与 UI 解耦，界面状态可独立测试 |
| **EC 实体组件** | `EntityObject` + `ComponentCore` + `ComponentService`，组件按 ID 注册、依赖顺序初始化 | 战斗实体（角色/怪物）按组件组合，新增实体能力不改动已有逻辑 |
| **Mono 适配层** | `MonoAdapter` 把 Unity 生命周期代理给纯 C# 逻辑类 | 核心代码脱离 MonoBehaviour，便于单元测试与复用 |
| **异步任务** | `AoTask` 封装 async/await，支持 UWR、AB 加载、超时等待 | 替代协程回调，业务代码顺序可读、异常可捕获 |
| **对象池 / 单例 / 日志** | `PoolManager`（对象池+数据池）、Singleton 基类、`LogManager` 分级标签日志 | 减少高频创建销毁的 GC 压力；统一日志便于定位 |
| **编辑器工具链** | Excel 导表（基于 Excel 解析插件）、配置代码生成（Class/Enum/InputAction/AB Key）、配置编辑器、AB 打包窗口、HybridCLR 构建工具 | 配置驱动，策划改表免改代码，减少手工同步错误 |

> 第三方依赖：JSON 序列化使用 Newtonsoft.Json，Excel 解析使用第三方插件，其余核心框架逻辑为手写。

---

## 战斗系统（HotUpdate/Game/Battle）

- **编排**：`BattleCoordinator` 统一编排伤害计算、事件调度、目标选择等独立服务，`BattleStateMachine` 管理战斗整体流程（准备 → 回合循环 → 结算 → 结束）。
- **回合状态机**：`TurnStart → TurnExecuting → TurnEnd → SettlementBuff`，角色行为由状态机驱动。
- **伤害计算**：`DamageCalcManager` + 策略模式（直伤 / 破击 / 持续伤害 / 真实伤害），`PropertyComponent` 统一封装攻/防/速等运行时数值。
- **受伤责任链**：`DamageChain` 拆为判断 → 护盾防御 → 韧性 → 伤害等多个处理器，支持动态增删。
- **技能多阶段流程**：技能拆为 PreCast → Cast → EventProcess → CastEnd 多阶段可组合执行（`SkillFlow` + `SkillPhaseStrategy`），角色/怪物技能通过工厂与策略扩展。
- **Buff/Status**：`StatusFactory` + 反射注册（`[StatusTypeId]`），Buff 参数由配置表驱动，支持层数变化、冲突、持续伤害等。
- **韧性击破**：`ToughnessStrategyFactory` 抽象削韧/击破为可替换策略。
- **指令系统**：`Command` + `BattleCommandsController` 基于优先级管理行动顺序，支持**终结技插队**与无效指令过滤。
- **其他**：多波次（`WaveHandler`）、目标选择策略、召唤物、追加攻击、遗器、弹道、Buff/伤害飘字。

## 业务系统

- **背包**：物品存储/堆叠/排序/销毁，`ReactiveProperty` 驱动 UI 自动更新，虚拟列表（`ItemGrid`）优化大量物品渲染，数据与 UI 解耦。
- **任务**：节点链式结构，ID 关联下一节点，完成条件工厂（击杀/收集/对话），事件中心推进进度，Json 持久化，ScriptableObject 配置。
- **对话**：Excel 配置驱动，逐字显示 + 历史回顾，分支来源抽象为可注入接口，分支处理器（跳转对话/触发战斗/提交物品）独立扩展。
- **交互**：统一交互接口，集中注册/触发/移除，靠近 NPC 弹出提示，与对话系统联动。
- **活动**：反射 + 特性自动注册活动类型，业务逻辑抽象为独立处理器，UI 统一接口调用。

---

## 运行流程

项目启动到进入主界面的完整流程如下（体现框架的启动链路）：

1. **启动**：从 `BeginScene` 开始，`GameLauncher` 注册核心框架（DI、事件中心、资源管理等），读取 `BootConfig` 启动配置；
2. **热更装配**：通过 HybridCLR 加载 hotfix DLL 与 AOT 元数据，实例化热更入口 `HotUpdateEntry`；
3. **模块初始化**：`ModuleService` 注册并异步初始化各业务模块，加载游戏设置（帧率等），初始化 UI 管理器（创建画布与 UI 相机）；
4. **检查更新**：打开开始界面，`AssetBundleUpdater` 走状态机流程（下载目录 → 对比差异 → 下载资源 → 校验完整性），支持断点续传与异常重试；
5. **进入游戏**：更新完成后显示"进入游戏"按钮，点击后初始化玩家，依次打开全局消息界面与主界面，进入 `MainScene` 后可进入战斗、背包、任务、对话等系统。

---

## 目录结构

```
Assets/
├── Scripts/
│   ├── Core/            # 框架层（不热更）：DI、事件中心、资源、更新、网络、UI、Mono 适配等
│   ├── Game/            # 启动器 GameLauncher（AOT）
│   └── HotUpdate/       # 业务层（热更程序集）
│       ├── Base/        #   EC 实体组件、模块、工厂、设置、数据接口
│       ├── Common/      #   配置数据（Excel 生成）、事件定义
│       ├── Entry/       #   热更入口 HotUpdateEntry
│       ├── Game/        #   战斗、背包、任务、对话、交互、活动、场景
│       └── UI/          #   各系统界面（MVC/MVVM）
├── Editor/              # 编辑器工具（导表、代码生成、AB 打包、HybridCLR）
├── Excel/               # 配置表源文件
└── ...
```

---

## 关于我

- 2026 届本科毕业，专注 Unity 游戏客户端开发，意向初级/应届岗位，可立即到岗
- 联系方式：【电话：15119229166 / 邮箱：3372745983@qq.com】
