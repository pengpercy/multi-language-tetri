# 项目结构概览 - Project Structure Overview

本文档描述了重新组织后的俄罗斯方块项目结构，清晰地分离了三个不同编程语言的实现版本。

## 📁 目录结构

```
vibecode/
├── README.md                   # 主项目说明文档
├── PROJECT_STRUCTURE.md        # 本文档 - 项目结构说明
├── mise.toml                   # 项目配置文件
├── akami.js                    # JavaScript相关文件
├── bubble_sort.html            # 演示文件
├── .vscode/                    # VS Code配置（共享）
├── docs/                       # 共享文档目录
│   ├── COMPARISON.md           # 版本对比分析
│   └── FINAL_STATUS.md         # 项目最终状态
├── python-tetris/              # Python版本目录
│   ├── README.md               # Python版本说明
│   ├── tetris.py               # 主游戏文件
│   ├── __pycache__/            # Python缓存
│   └── .ropeproject/           # Python项目配置
├── c-tetris/                   # C语言版本目录
│   ├── README.md               # C版本说明
│   ├── Makefile                # 构建配置
│   ├── tetris_improved.c       # 改进版本 (推荐)
│   ├── tetris_fixed.c          # 完整版本
│   ├── tetris_simple.c         # 简化版本
│   ├── tetris_fixed            # 编译后的可执行文件
│   ├── font_test.c             # 字体测试
│   ├── text_test.c             # 文本测试
│   ├── build_and_run.sh        # 构建运行脚本
│   ├── test_versions.sh        # 版本测试脚本
│   ├── verify_both_versions.sh # 版本验证脚本
│   ├── tetris_fixed_debug.dSYM/# 调试符号
│   └── docs/                   # C版本文档
│       ├── FIX_SUMMARY.md
│       ├── SDL2_COEXISTENCE_SOLUTION.md
│       └── TETRIS_FIXES_SUMMARY.md
└── csharp-tetris/              # C#版本目录
    ├── README.md               # C#版本说明
    ├── TetrisSharp.csproj      # 项目文件
    ├── Program.cs              # 程序入口
    ├── Game.cs                 # 游戏逻辑
    ├── Models.cs               # 数据模型
    ├── Constants.cs            # 游戏常量
    ├── FontManager.cs          # 字体管理
    ├── SDL_ttf.cs              # SDL绑定
    ├── DynamicPriorityTimingWheel.cs # 时间轮算法
    ├── build_and_run.sh        # 构建运行脚本
    ├── build_release.sh        # 发布构建脚本
    ├── launch.sh               # 启动脚本
    ├── fix_libs.sh             # 库修复脚本
    ├── bin/                    # 编译输出
    ├── obj/                    # 编译对象文件
    └── docs/                   # C#版本文档
        ├── FONT_FIXES_SUMMARY.md
        ├── IMPROVEMENTS_SUMMARY.md
        ├── SDL2_CONFLICT_FIX_SUMMARY.md
        ├── SOLUTION_FINAL.md
        ├── UI_IMPROVEMENTS_FINAL.md
        ├── UI_IMPROVEMENTS_SUMMARY.md
        ├── WARNING_FIXES_SUMMARY.md
        ├── RELEASE_BUILD_COMMANDS.md
        └── test_font.md
```

## 🎯 版本选择指南

### Python版本 (`python-tetris/`)
**适用场景:**
- 游戏开发学习
- 快速原型设计
- UI效果实验
- Python语言学习

**特点:**
- 开发速度快
- 代码易读易维护
- 丰富的视觉效果
- 依赖简单（仅需pygame）

### C语言版本 (`c-tetris/`)
**适用场景:**
- 高性能要求
- 系统编程学习
- 嵌入式开发
- 底层优化研究

**特点:**
- 性能最优
- 内存占用最小
- 多个版本可选
- 完整的SDL2集成

**版本说明:**
- `tetris_improved.c` - 推荐版本，平衡功能和依赖
- `tetris_fixed.c` - 完整版本，需要SDL2_ttf
- `tetris_simple.c` - 简化版本，最小依赖

### C#版本 (`csharp-tetris/`)
**适用场景:**
- 跨平台应用开发
- 企业级游戏开发
- .NET生态系统集成
- 现代语言特性研究

**特点:**
- 现代语言特性
- 良好的架构设计
- 完善的SDL2绑定
- 解决了库冲突问题

## 🚀 快速开始

### 1. Python版本
```bash
cd python-tetris
pip install pygame
python tetris.py
```

### 2. C语言版本
```bash
cd c-tetris
./build_and_run.sh
```

### 3. C#版本
```bash
cd csharp-tetris
./build_and_run.sh
```

## 📚 文档结构

### 主文档
- `README.md` - 项目总览和三版本对比
- `PROJECT_STRUCTURE.md` - 本文档，项目结构说明

### 版本文档
每个版本目录都包含：
- `README.md` - 版本特定的详细说明
- 构建和运行脚本
- 版本特定的文档和修复说明

### 共享文档 (`docs/`)
- `COMPARISON.md` - 三版本详细对比分析
- `FINAL_STATUS.md` - 项目开发完成状态

## 🔧 开发环境

### 通用要求
- 支持的操作系统: macOS, Linux, Windows
- Git版本控制
- 文本编辑器或IDE

### Python版本依赖
- Python 3.6+
- pygame 2.0+

### C语言版本依赖
- GCC 4.9+ 或 Clang 3.8+
- SDL2 2.0.0+
- SDL2_ttf (完整版需要)
- Make 构建工具

### C#版本依赖
- .NET 6.0+
- SDL2 动态库
- SDL2_ttf 动态库

## 🎮 功能对比

| 功能特性 | Python | C语言 | C# |
|----------|--------|-------|-----|
| 基础游戏逻辑 | ✅ | ✅ | ✅ |
| 3D视觉效果 | ✅ | ✅ | ✅ |
| 粒子动画 | ✅ | ✅ | ✅ |
| 多语言界面 | ❌ | ✅ | ✅ |
| 字体渲染 | ✅ | ✅ | ✅ |
| 自定义字体 | ❌ | ✅ | ✅ |
| 音效系统 | ❌ | ❌ | ❌ |
| 配置文件 | ❌ | ❌ | ❌ |

## 📈 性能对比

| 指标 | Python | C语言 | C# |
|------|--------|-------|-----|
| 启动时间 | ~3秒 | <1秒 | <2秒 |
| 内存占用 | ~20MB | <5MB | <15MB |
| CPU使用率 | ~5% | <3% | <2% |
| 帧率稳定性 | 良好 | 优秀 | 优秀 |

## 🔄 维护说明

### 代码同步
三个版本的核心游戏逻辑应保持一致：
- 方块旋转算法
- 消行检测逻辑  
- 分数计算规则
- 等级提升机制

### 功能更新
当添加新功能时，建议：
1. 先在Python版本实现（快速验证）
2. 移植到C版本（性能优化）
3. 适配到C#版本（架构完善）

### 文档维护
- 更新对应版本的README.md
- 同步更新主README.md的对比表
- 记录重要变更到对应的文档文件

## 🤝 贡献指南

1. 选择要贡献的版本目录
2. 阅读对应版本的README.md
3. 遵循版本特定的编码规范
4. 确保跨版本功能的一致性
5. 更新相关文档

---

**最后更新**: 2024-12-19  
**项目状态**: 结构重组完成，文档目录已整理 ✅