

# DataPlatform 数据采集平台

## 项目简介

DataPlatform 是一个基于 C# .NET 开发的工业数据采集平台，支持多种工业通讯协议（Modbus TCP/RTU、OPC UA、 Siemens PLC）的设备数据采集、存储与访问。

## 功能特性

- **多协议支持**
  - Modbus TCP
  - Modbus RTU Over TCP
  - OPC UA
  - Siemens S7 PLC

- **数据采集**
  - 支持批量读取数据点
  - 定时采集任务
  - 设备状态监控与自动重连

- **数据存储**
  - 使用 SqlSugar ORM 操作数据库
  - 支持设备信息与数据点管理

- **HTTP API**
  - 提供 HTTP 接口进行数据写入
  - 支持跨平台数据访问

- **配置管理**
  - INI 配置文件支持
  - 灵活的点位地址解析

## 项目结构

```
DataPlatform/
├── API/                    # HTTP API 接口
├── DBLib/                  # 数据库操作
├── Drive/                  # 驱动层（Modbus、OPC UA、Siemens）
├── Models/                 # 数据模型
│   ├── APIClass/          # API 请求/响应类
│   ├── DataBase/          # 数据库实体类
│   └── OPCTagClass.cs     # OPC 标签类
├── Read/                   # 数据读取模块
├── Tools/                  # 工具类
│   ├── AddressHelper/     # 地址解析
│   └── ParseBatchReadResults/  # 批量读取结果解析
└── MainStart.cs           # 主启动类
```

## 技术栈

- .NET Framework / .NET Core
- SqlSugar (ORM)
- HslCommunication (工业通讯库)
- OPC UA SDK

## 快速开始

### 环境要求

- Visual Studio 2019 或更高版本
- .NET Framework 4.6.1 或更高版本

### 编译运行

1. 使用 Visual Studio 打开 `DataPlatform.sln`
2. 还原 NuGet 包
3. 编译解决方案
4. 运行 `DataPlatform` 项目

### 配置说明

在 `App.config` 中配置数据库连接字符串。

## 驱动说明

### Modbus TCP/RTU

- 支持 TCP 和 RTU Over TCP 两种模式
- 支持多种数据格式 (CDBA, ABCD, BADC, etc.)
- 支持 PLC 地址格式

### OPC UA

- 支持用户名/密码认证
- 支持证书认证
- 支持订阅方式实时数据获取

### Siemens PLC

- 支持多种 CPU 类型
- 自动地址解析
- 支持 Bool、Int16、UInt16、Float 等数据类型

## API 接口

### 数据写入

```
POST /write
Content-Type: application/json

{
    "deviceId": 1,
    "deviceName": "设备名称",
    "address": "地址",
    "dataType": "数据类型",
    "writeValue": "写入值"
}
```

## 数据库表

### device 表

| 字段 | 说明 |
|------|------|
| id | 设备ID |
| device_name | 设备名称 |
| model | 设备型号 |
| ip | IP地址 |
| port | 端口 |
| link_address | 连接地址 |
| user_name | 用户名 |
| pass_word | 密码 |
| certificate_path | 证书路径 |
| device_state | 设备状态 |
| communication_mode | 通讯模式 |
| collection_interval | 采集间隔 |

### point 表

| 字段 | 说明 |
|------|------|
| id | 点位ID |
| device_id | 设备ID |
| point_name | 点位名称 |
| point_address | 点位地址 |
| point_value | 点位值 |
| update_time | 更新时间 |
| point_type | 点位类型 |
| parent_config | 父配置 |
| scale_factor | 缩放因子 |

## 许可证

本项目仅供学习参考使用。
