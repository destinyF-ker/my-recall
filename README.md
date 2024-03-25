<dev align="center">
    <h1>RecAll</h1>
    <div align="center">
        <img src="https://s2.loli.net/2024/03/25/XWyaJ6GZjECkseI.png"/>
    </div>
    <h3>Record All of your belongings🧠, and help you RecAll it!🤩</h3>
</dev>

# my-recall

一个功能丰富的错题本微服务

## 写在前面

这是一个使用 ASP.NET core + docker 微服务实现的小玩具，微服务架构与设计模式课设，师承本仓库的另一位 contributor，可能最后也可以变成一个比较完善的商业软件？

### 使用 vscode!

从配置到编码全程使用 vscode ，事实证明使用 vscode 也有不输于 visual sudio 或 Rider 的生产力(这点对于 mac 用户来说就非常重要了)。

<div align="center">
    <img src="https://s2.loli.net/2024/03/25/vHnQxTwY9IGAcOt.jpg"
       style="zoom:50%"
    />
</div>

所以接下来我会大概带领大家浏览一遍使用 vscode 开发基于 ASP.NET + docker 的过程

### 叠甲

本人也是第一次学习 ASP.NET 和 docker，所如果在本文档之中说出了什么暴论，欢迎在邮箱或者开 issue 来和我交流

# 开始

## 关于.NET 开发环境

本项目基于.NET8.0，所以要安装.NET8.0 的 sdk 还有 runtime。使用控制台执行命令包含有以下输出即可：

```bash
$ dotnet --list-sdks
8.0.203 L/usr/local/share/dotnet/sdk]
$ dotenet --list-runtimes
Microsoft.AspNetCore.App 8.0.3 [/usr/local/share/dotnet/shared/Microsoft.AspNetCore.App]
Microsoft. NETCore.App 8.0.3 [/usr/local/share/dotnet/shared/Microsoft.NETCore.App]
```

> 哈哈 😄，摊牌了，为什么在 mac 上不能使用 visual studio 进行开发呢？因为幽默微软直接把 visual studio for mac 枪毙了，而且现在 visual studio for mac 仅仅支持.NET7.0 进行开发

## 使用 vscode + docker CLI 完成微服务开发

很多人不用 vscode 的原因可能都是嫌弃它只是一个 editor，并没有像是 IDE 那样完备的工具链和方便的功能，但是 vscode 实际上属于“重剑无锋，大巧不工”的那种类型，丰富的插件让它在保持一个比较小的运行时的同时也能保证功能的强大。好像有点跑偏了，那具体到底为什么你应该选择 vscode 我就不展开说了，在 google 上一搜一大堆:)

好吧，如果没有插件的话 vscode 真的就只是一个文本编辑器而已，所以我们要做的第一件事情就是装插件。

**需要的插件有以下几个**：

| 名称                                | 功能                                  |
| ----------------------------------- | ------------------------------------- |
| C# for Visual Studio Code           | 提供 vscodeC#支持                     |
| C# Dev Kit                          | 提供解决方案资源管理器支持(.sln 文件) |
| .NET Install Tool                   | 提供.Net runtime 以及.Net sdk 支持    |
| IntelliCode                         | 提供智能代码补全功能                  |
| IntelliCode API Usage Examples      | 提供 IntelliCode API 使用示例         |
| Docker                              | 提供 Docker 支持                      |
| Dev Containers                      | 提供开发容器支持                      |
| Docker Compose                      | 提供 Docker Compose 支持              |
| Kubernetes                          | 提供 Kubernetes 支持                  |
| SQLTools                            | 提供 SQL 工具支持                     |
| SQLTools Microsoft SQL Server Azure | 提供 Microsoft SQL Server Azure 支持  |
| Dapr                                | 提供 Dapr 支持                        |

Docker 插件里面 Dev Container 是必装的，用于给项目添加容器，其他插件可以用 docker desktop 代替。

### 创建项目

首先创建一个空文件夹，然后使用 dotnet CLI 在该文件夹之中创建一个新的解决方案：

```bash
$ dotnet new sln -n my-recall
```

现在这个空解决方案的文件树应该长这样：

```bash
my-recall
└── my-recall.sln
```

好，继续使用`mkdir`命令创建目录：

```bash
$ mkdir Contrib
```

然后`cd`到 Contrib 目录之中，按下 `F1` 键，输入 `net`，找到<strong>.NET 新建项目(.NET new project)</strong>，回车:

<div align="center">
    <img src="https://s2.loli.net/2024/03/25/XZIpJLqDK3Fvuck.png" style="zoom:50%"/>
</div>

现在 vscode 会询问我们要创建哪一种类型的项目，我们选择 **ASP.NET Core Web API Web, Web API**，回车

<!-- ![](https://s2.loli.net/2024/03/25/HZEXMtejRu6vAF4.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/HZEXMtejRu6vAF4.png" style="zoom:50%"/>
</div>

接下来会让我们指定项目名称，输入**TextItem.Api**，回车：

<!-- ![](https://s2.loli.net/2024/03/25/DbxTQE9euKrHPX7.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/DbxTQE9euKrHPX7.png" style="zoom:50%"/>
</div>

该到指定项目文件夹路径的时候了，因为我们要将项目放到`Contrib`目录下，所以**选择其他目录**

<!-- ![](https://s2.loli.net/2024/03/25/HRbO2NwqryeBogZ.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/HRbO2NwqryeBogZ.png" style="zoom:50%"/>
</div>

项目文件夹就变成了这样：

<!-- ![](https://s2.loli.net/2024/03/25/T5Pyk3NjIpM2YEL.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/T5Pyk3NjIpM2YEL.png" style="zoom:50%"/>
</div>

🤯🤯🤯，怎么回事！解决方案，一下就出现了两个！好吧，实际上上面的流程就仅仅适用于解决方案和项目同名的情况，可以非常方便地进行创建，现在看来我们要另某它法了。

退回到这一步：

```bash
$ cd Contrib
```

使用 dotnet CLI 来创建项目，这一部分详细可以参考微软的[官方文档](https://learn.microsoft.com/zh-cn/dotnet/core/tools/dotnet-new)：

<!-- ![](https://s2.loli.net/2024/03/25/qsrYVLZuElgvpaG.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/qsrYVLZuElgvpaG.png" style="zoom:50%"/>
</div>

在 Contrib 目录下执行命令，其中`-n`参数指定项目的名称，`-f`参数指定项目所依赖的.NET 运行时版本：

```bash
$ dotnet new webapi -n TextItem.Api -f net8.0
```

应该可以看到下面的输出：

```bash
The template "ASP.NET Core Web API" was created successfully.

Processing post-creation actions...
Restoring /Users/xxx/Desktop/example/Contrib/TextItem.Api/TextItem.Api.csproj:
  Determining projects to restore...
  Restored /Users/xxx/Desktop/example/Contrib/TextItem.Api/TextItem.Api.csproj (in 126 ms).
Restore succeeded.
```

现在的项目结构就对了：

<!-- ![](https://s2.loli.net/2024/03/25/dswrFPfBiAK9ZlR.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/dswrFPfBiAK9ZlR.png" style="zoom:50%"/>
</div>

但是此时在项目解决方案之中看不到任何内容：

<!-- ![](https://s2.loli.net/2024/03/25/8veHNUhj7GqlMow.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/8veHNUhj7GqlMow.png" style="zoom:50%"/>
</div>

这是为什么呢？实际上解决方案资源管理器之中显示的都是在项目根目录下的这个`.sln`配置文件之中所配置的内容，其中记录了解决方案的元数据，虚拟文件夹等信息。

依然可以通过 dotnet CLI 来将项目添加到解决方案之中：

```bash
$ dotnet sln your-solution.sln add path-to-your-project.csproj
```

在本项目之中就是(在项目根目录下执行)：

```bash
$ dotnet sln myRecAll.sln add ./Contrib/TextItem.Api/TextItem.Api.csproj
```

看到输出“已将项目“Contrib/TextItem.Api/TextItem.Api.csproj”添加到解决方案中。”之后，可以发现 myRecAll.sln 文件内容发生了改变：

<!-- ![](https://s2.loli.net/2024/03/25/sYSIGtJaqne4XrL.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/sYSIGtJaqne4XrL.png" style="zoom:50%"/>
</div>

表示虚拟文件夹已经添加进来了。

<!-- ![](https://s2.loli.net/2024/03/25/waQ416PIuMWr9Zk.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/waQ416PIuMWr9Zk.png" style="zoom:50%"/>
</div>

现在我们就可以愉快地在项目之中构建代码了 😆

### 为项目添加 docker 支持

注意，这一部分的内容必须要依赖插件 Dev Containers 才能进行，没有安装的朋友现在立马去安装一个

<!-- ![](https://s2.loli.net/2024/03/25/w4hE56zskD3YTJx.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/w4hE56zskD3YTJx.png" style="zoom:50%"/>
</div>

在 vscode 之中按`F1`进入到命令中心，输入 docker，选择**Docker: Add Docker Files to Workspace**:

<!-- ![](https://s2.loli.net/2024/03/25/pJYQX8H2s7kod5h.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/pJYQX8H2s7kod5h.png" style="zoom:50%"/>
</div>

第一项是要我们选择项目的运行平台，当然无脑选择<strong>.NET: ASP.NET Core</strong>，这样在 Dockerfile 之中就会帮我们自动配置好相关 ASP.NET Core 依赖项的构建:

<!-- ![](https://s2.loli.net/2024/03/25/WN5cP7zKCAHvpXe.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/WN5cP7zKCAHvpXe.png" style="zoom:50%"/>
</div>

第二项是选择 docker 基础镜像的操作系统，这里选择 Linux：

<!-- ![](https://s2.loli.net/2024/03/25/dxupJ8EFAqHS5Q9.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/dxupJ8EFAqHS5Q9.png" style="zoom:50%"/>
</div>

第三项是选择 container 容器在运行时暴露的端口，这里可以让 vscode 帮我们默认选择一个，也可以自己配置

<!-- ![](https://s2.loli.net/2024/03/25/lWyszGvLqwB4bUX.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/lWyszGvLqwB4bUX.png" style="zoom:50%"/>
</div>

第四项是选择是否为项目生成可选的 docker-compose.yml 文件，因为是微服务，之后会涉及到多个容器的运行，所以这里当然选择`yes`

<!-- ![](https://s2.loli.net/2024/03/25/ZmJY1GFD65z2Lnb.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/ZmJY1GFD65z2Lnb.png" style="zoom:50%"/>
</div>

现在项目的文件结构应该是这样的：

<!-- ![](https://s2.loli.net/2024/03/25/nu3K6BWFOCxk4GV.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/nu3K6BWFOCxk4GV.png" style="zoom:50%"/>
</div>

比较遗憾的是，Dev Container 插件并不会帮我们自动生成`docker-compose.override.yml`文件，所以我们要自己手动创建(`touch`)一个

好，这样一个有 docker 支持的.NET 项目就配置完毕了。

## 补充说明
