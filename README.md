# CUnityTableImporter

一个用于在Unity项目中导入和处理Excel(.xlsx)和CSV(.csv)表格文件的轻量级插件。

## 功能介绍

CUnityTableImporter提供了简单易用的工具，帮助Unity开发者快速导入表格数据并生成对应的C#类和JSON文件，适用于游戏开发中的配置数据、本地化文本等场景。

### 主要特性

- 支持Excel(.xlsx)和CSV(.csv)文件格式
- 自动生成对应的C#数据类
- 导出表格数据为JSON格式
- 简单直观的编辑器界面
- 使用NPOI库处理Excel文件，无需额外依赖

## 使用方法

### 1. 表格格式要求

表格需要按照以下格式组织：
- 第1行：字段名（将用作C#类的属性名）
- 第2行：数据类型（支持的类型包括：int, float, string, bool等）
- 第3行及以后：数据内容

### 2. 打开导入工具

在Unity编辑器中，选择菜单 `cnoom > 表格导入工具` 打开工具窗口。

### 3. 导入表格

1. 点击"选择表格路径"按钮，选择要导入的Excel(.xlsx)或CSV(.csv)文件
2. 设置"类输出目录"，指定生成的C#类文件保存位置
3. 设置"JSON输出目录"，指定生成的JSON文件保存位置
4. 可以自定义类名，默认使用CSV文件名或Excel文件的第一个表名
5. 点击"生成表类"按钮生成C#类文件
6. 点击"生成JSON"按钮生成JSON数据文件

## 安装方法

### 通过Unity Package Manager安装

1. 打开Unity项目
2. 打开Package Manager (菜单: Window > Package Manager)
3. 点击"+"按钮，选择"Add package from git URL..."
4. 输入: `https://github.com/cnoom/CUnityTableImporter.git`
5. 点击"Add"按钮

### 手动安装

1. 下载最新版本的CUnityTableImporter
2. 将下载的文件解压到你的Unity项目的`Packages`文件夹中

## 示例

### 表格示例 (players.csv)

```
ID,Name,Level,Health,IsMagicUser
int,string,int,float,bool
1,Knight,10,100.5,false
2,Wizard,8,75.0,true
3,Archer,9,85.5,false
```

### 生成的C#类

```csharp
// 自动生成的代码
using System;
using System.Collections.Generic;

[Serializable]
public class Player
{
    public int ID;
    public string Name;
    public int Level;
    public float Health;
    public bool IsMagicUser;
}
```

### 生成的JSON

```json
[
  {
    "ID": 1,
    "Name": "Knight",
    "Level": 10,
    "Health": 100.5,
    "IsMagicUser": false
  },
  {
    "ID": 2,
    "Name": "Wizard",
    "Level": 8,
    "Health": 75.0,
    "IsMagicUser": true
  },
  {
    "ID": 3,
    "Name": "Archer",
    "Level": 9,
    "Health": 85.5,
    "IsMagicUser": false
  }
]
```

## 系统要求

- Unity 2019.4 或更高版本

## 版本历史

- 0.5.0: 初始版本，支持基本的Excel和CSV导入功能

## 许可证

本项目使用MIT许可证 - 详情请查看LICENSE文件