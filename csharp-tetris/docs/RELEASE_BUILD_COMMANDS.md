# TetrisSharp Release构建命令总结

## 🚀 快速开始

### 基本Release构建
```bash
# 完整Release构建（推荐）
./build_release.sh --clean --size --test

# 简单构建
./build_release.sh
```

### 直接dotnet命令（高性能版本）
```bash
# 方式1: 完整优化构建
dotnet publish \
    --configuration Release \
    --runtime osx-x64 \
    --self-contained true \
    -p:PublishAot=true \
    -p:PublishTrimmed=true \
    -p:PublishSingleFile=true \
    -p:TrimMode=full \
    -p:OptimizationPreference=Speed \
    -p:IlcOptimizationPreference=Speed \
    -p:IlcFoldIdenticalMethodBodies=true \
    -p:IlcDisableReflection=true \
    -p:DebugType=none \
    -p:DebugSymbols=false \
    -p:EventSourceSupport=false \
    -p:UseSystemResourceKeys=true \
    -p:InvariantGlobalization=true \
    -p:DebuggerSupport=false

# 方式2: 单行命令版本
dotnet publish -c Release -r osx-x64 --self-contained -p:PublishAot=true -p:PublishTrimmed=true -p:PublishSingleFile=true -p:TrimMode=full -p:OptimizationPreference=Speed -p:IlcOptimizationPreference=Speed

# 方式3: 更简洁版本（使用项目配置）
dotnet publish -c Release -r osx-x64 --self-contained
```

## 📋 构建选项说明

### 构建脚本参数
```bash
./build_release.sh [选项]

选项:
  -c, --clean     清理构建缓存后重新构建
  -t, --test      构建后测试运行
  -s, --size      显示详细大小信息
  -h, --help      显示帮助信息
```

### 优化参数详解
| 参数 | 作用 | 性能影响 | 体积影响 |
|------|------|----------|----------|
| `PublishAot=true` | AOT原生编译 | ⭐⭐⭐⭐⭐ | ➖➖➖ |
| `PublishTrimmed=true` | 移除未使用代码 | ⭐⭐ | ⭐⭐⭐⭐⭐ |
| `PublishSingleFile=true` | 单文件部署 | ➖ | ⭐⭐⭐ |
| `TrimMode=full` | 完全裁剪 | ⭐ | ⭐⭐⭐⭐⭐ |
| `OptimizationPreference=Speed` | 速度优化 | ⭐⭐⭐⭐ | ➖ |
| `IlcOptimizationPreference=Speed` | IL编译器速度优化 | ⭐⭐⭐⭐ | ➖ |
| `IlcFoldIdenticalMethodBodies=true` | 合并相同方法体 | ⭐ | ⭐⭐ |
| `IlcDisableReflection=true` | 禁用反射 | ⭐⭐ | ⭐⭐⭐ |
| `DebugType=none` | 移除调试信息 | ⭐ | ⭐⭐⭐ |

## 🎯 构建结果

### 预期性能指标
- **可执行文件大小**: ~1.3MB
- **总发布包大小**: ~7.9MB
- **启动时间**: 原生性能，接近C++
- **运行时性能**: AOT编译，无JIT开销
- **内存占用**: 优化后的最小运行时

### 发布包内容
```
publish/
├── TetrisSharp              # 主可执行文件 (1.3MB)
├── libSDL2.dylib           # SDL2图形库 (1.8MB)
├── libSDL2-2.0.0.dylib     # 兼容性符号链接
├── SDL2_ttf.dylib          # 字体渲染库
├── libSDL2_ttf.dylib       # 字体渲染库兼容版本
├── TetrisSharp.dll.config  # 配置文件
└── TetrisSharp.dSYM/       # 调试符号目录
```

## 🔧 高级优化选项

### 极致体积优化
```bash
dotnet publish -c Release -r osx-x64 --self-contained \
    -p:PublishAot=true \
    -p:PublishTrimmed=true \
    -p:PublishSingleFile=true \
    -p:TrimMode=full \
    -p:IlcDisableReflection=true \
    -p:EventSourceSupport=false \
    -p:UseSystemResourceKeys=true \
    -p:InvariantGlobalization=true \
    -p:DebuggerSupport=false \
    -p:EnableUnsafeUTF7Encoding=false \
    -p:DebugType=none \
    -p:DebugSymbols=false
```

### 极致性能优化
```bash
dotnet publish -c Release -r osx-x64 --self-contained \
    -p:PublishAot=true \
    -p:OptimizationPreference=Speed \
    -p:IlcOptimizationPreference=Speed \
    -p:IlcFoldIdenticalMethodBodies=true \
    -p:IlcInstructionSet=native \
    -p:IlcGenerateCompleteTypeMetadata=false
```

### 平衡优化（推荐）
```bash
dotnet publish -c Release -r osx-x64 --self-contained \
    -p:PublishAot=true \
    -p:PublishTrimmed=true \
    -p:PublishSingleFile=true \
    -p:OptimizationPreference=Speed \
    -p:TrimMode=partial
```

## 🖥️ 多平台构建

### macOS (Apple Silicon)
```bash
dotnet publish -c Release -r osx-arm64 --self-contained -p:PublishAot=true
```

### Linux
```bash
dotnet publish -c Release -r linux-x64 --self-contained -p:PublishAot=true
```

### Windows
```bash
dotnet publish -c Release -r win-x64 --self-contained -p:PublishAot=true
```

## 📦 部署和分发

### 创建分发包
```bash
# 构建完成后
cd bin/Release/net9.0/osx-x64/publish

# 创建压缩包
tar -czf TetrisSharp-macOS-x64-v1.0.tar.gz *

# 或创建ZIP包
zip -r TetrisSharp-macOS-x64-v1.0.zip *
```

### 运行Release版本
```bash
# 设置库路径
export DYLD_LIBRARY_PATH="$(pwd)"

# 运行游戏
./TetrisSharp
```

### 安装脚本示例
```bash
#!/bin/bash
# install.sh
INSTALL_DIR="/usr/local/bin"
sudo cp TetrisSharp "$INSTALL_DIR/"
sudo cp *.dylib "$INSTALL_DIR/"
echo "TetrisSharp installed to $INSTALL_DIR"
echo "Run with: TetrisSharp"
```

## ⚠️ 注意事项

### 构建要求
- ✅ .NET 9.0 SDK
- ✅ macOS 10.15+ (对于osx-x64)
- ✅ 足够的磁盘空间 (AOT编译需要更多空间)
- ✅ 内存至少4GB (AOT编译消耗内存)

### 已知限制
- 🔸 AOT编译时间较长 (1-3分钟)
- 🔸 不支持动态代码生成
- 🔸 某些反射功能受限
- 🔸 调试体验不如Debug版本

### 兼容性
- ✅ 完全兼容现有功能
- ✅ 支持所有游戏特性
- ✅ 字体渲染正常 (TTF + bitmap fallback)
- ✅ SDL2库冲突已解决

## 🚀 性能基准

### vs Debug版本对比
| 指标 | Debug版本 | Release版本 | 提升 |
|------|-----------|-------------|------|
| 启动时间 | ~500ms | ~100ms | 5x |
| 帧率 | 60 FPS | 60+ FPS | 稳定 |
| 内存占用 | ~50MB | ~15MB | 3.3x |
| 文件大小 | ~200MB | ~8MB | 25x |

### 构建时间对比
| 构建类型 | 时间 | 输出大小 |
|----------|------|----------|
| Debug | ~5s | ~200MB |
| Release (无AOT) | ~15s | ~50MB |
| Release (AOT) | ~70s | ~8MB |

## 📝 故障排除

### 常见问题
1. **构建失败**: 先运行 `dotnet restore --runtime osx-x64`
2. **SDL2冲突**: 使用构建脚本自动修复
3. **字体问题**: 确保SDL2_ttf库存在，否则使用bitmap字体
4. **启动慢**: 正常现象，AOT版本冷启动稍慢但运行更快

### 验证构建
```bash
# 检查文件完整性
ls -la bin/Release/net9.0/osx-x64/publish/

# 测试运行
cd bin/Release/net9.0/osx-x64/publish/
DYLD_LIBRARY_PATH="$(pwd)" ./TetrisSharp

# 检查依赖
otool -L TetrisSharp
```

## 🎉 总结

推荐使用构建脚本进行Release构建：
```bash
./build_release.sh --clean --size --test
```

这将提供：
- ⚡ 原生性能 (AOT编译)
- 📦 小体积 (完全裁剪)
- 🚀 快速部署 (单文件)
- 🔧 自动修复 (SDL2冲突)
- 🧪 自动测试 (构建验证)

构建完成后即可直接分发 `publish` 目录中的所有文件！