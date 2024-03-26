# myRecAll

<div align="center">
    <div align="center">
        <img src="https://s2.loli.net/2024/03/25/XWyaJ6GZjECkseI.png" alt="Not find"/>
    </div>
    <h3 align="center">Record All of your belongings🧠, and help you RecAll it!🤩</h3>
    <h3 align="center">一个功能丰富的错题本微服务</h3>
</div>

- [myRecAll](#myrecall)
  - [写在前面](#写在前面)
    - [使用 vscode](#使用-vscode)
    - [叠甲](#叠甲)
- [开始](#开始)
  - [关于.NET 开发环境](#关于net-开发环境)
  - [使用 vscode + docker CLI 完成微服务开发](#使用-vscode--docker-cli-完成微服务开发)
    - [创建项目](#创建项目)
      - [方案 1：使用 vscode 命令中心](#方案-1使用-vscode-命令中心)
      - [方案 2：使用 dotnet CLI](#方案-2使用-dotnet-cli)
      - [方案 3：使用 C# Dev Kit 提供的解决方案资源管理器](#方案-3使用-c-dev-kit-提供的解决方案资源管理器)
    - [为项目添加 docker 支持](#为项目添加-docker-支持)
    - [为项目添加 package 依赖](#为项目添加-package-依赖)
      - [TextItem.Api:](#textitemapi)
      - [Infrastructure.Api:](#infrastructureapi)
      - [ServiceStatus](#servicestatus)
      - [通过 dotnet CLI 使用 Nuget 来为项目添加依赖](#通过-dotnet-cli-使用-nuget-来为项目添加依赖)
  - [补充说明](#补充说明)
    - [关于 docker 端口号](#关于-docker-端口号)
    - [关于 docker 环境变量`ASPNETCORE_ENVIRONMENT`](#关于-docker-环境变量aspnetcore_environment)

## 写在前面

这是一个使用 ASP.NET core + docker 微服务实现的小玩具，微服务架构与设计模式课设，可能最后也可以变成一个比较完善的软件？

感谢 Zhang, Yin 老师课上非常详细的解释和指导！

### 使用 vscode

从配置到编码全程使用 vscode ，事实证明使用 vscode 也有不输于 visual sudio 或 Rider 的生产力(这点对于 mac 用户来说就非常重要了，我丐版 mac 8G 内存怎么顶得住 JetBrains 家族对内存的胡吃海喝啊)。

<div align="center">
    <img src="https://s2.loli.net/2024/03/25/vHnQxTwY9IGAcOt.jpg"
       width="500" alt="Not find"
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

#### 方案 1：使用 vscode 命令中心

首先创建一个空文件夹，然后使用 dotnet CLI 在该文件夹之中创建一个新的解决方案：

```bash
dotnet new sln -n my-recall
```

现在这个空解决方案的文件树应该长这样：

```bash
my-recall
└── my-recall.sln
```

好，继续使用`mkdir`命令创建目录：

```bash
mkdir Contrib
```

然后`cd`到 Contrib 目录之中，按下 `F1` 键，输入 `net`，找到<strong>.NET 新建项目(.NET new project)</strong>，回车:

<div align="center">
    <img src="https://s2.loli.net/2024/03/25/XZIpJLqDK3Fvuck.png" width="500" alt="Not find"/>
</div>

现在 vscode 会询问我们要创建哪一种类型的项目，我们选择 **ASP.NET Core Web API Web, Web API**，回车

<!-- ![](https://s2.loli.net/2024/03/25/HZEXMtejRu6vAF4.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/HZEXMtejRu6vAF4.png" width="500" alt="Not find"/>
</div>

接下来会让我们指定项目名称，输入**TextItem.Api**，回车：

<!-- ![](https://s2.loli.net/2024/03/25/DbxTQE9euKrHPX7.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/DbxTQE9euKrHPX7.png" width="500" alt="Not find"/>
</div>

该到指定项目文件夹路径的时候了，因为我们要将项目放到`Contrib`目录下，所以在**选择其他目录**上按回车，并在你你操作系统的文件系统上（访达或文件资源管理器）选定 Contrib 目录

<!-- ![](https://s2.loli.net/2024/03/25/HRbO2NwqryeBogZ.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/HRbO2NwqryeBogZ.png" width="500" alt="Not find"/>
</div>

项目文件夹就变成了这样：

<!-- ![](https://s2.loli.net/2024/03/25/T5Pyk3NjIpM2YEL.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/T5Pyk3NjIpM2YEL.png" width="500" alt="Not find"/>
</div>

🤯🤯🤯，怎么回事！解决方案，一下就出现了两个！好吧，实际上上面的流程就仅仅适用于解决方案和项目同名的情况，可以非常方便地进行创建，现在看来我们要另某它法了。

#### 方案 2：使用 dotnet CLI

退回到这一步：

```bash
cd Contrib
```

使用 dotnet CLI 来创建项目，这一部分详细可以参考微软的[官方文档](https://learn.microsoft.com/zh-cn/dotnet/core/tools/dotnet-new)：

<!-- ![](https://s2.loli.net/2024/03/25/qsrYVLZuElgvpaG.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/qsrYVLZuElgvpaG.png" width="500" alt="Not find"/>
</div>

在 Contrib 目录下执行命令，其中`-n`参数指定项目的名称，`-f`参数指定项目所依赖的.NET 运行时版本：

```bash
dotnet new webapi -n TextItem.Api -f net8.0
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
    <img src="https://s2.loli.net/2024/03/25/dswrFPfBiAK9ZlR.png" width="500" alt="Not find"/>
</div>

但是此时在项目解决方案之中看不到任何内容：

<!-- ![](https://s2.loli.net/2024/03/25/8veHNUhj7GqlMow.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/8veHNUhj7GqlMow.png" width="500" alt="Not find"/>
</div>

这是为什么呢？实际上解决方案资源管理器之中显示的都是在项目根目录下的这个`.sln`配置文件之中所配置的内容，其中记录了解决方案的元数据，虚拟文件夹等信息。

依然可以通过 dotnet CLI 来将项目添加到解决方案之中：

```bash
dotnet sln your-solution.sln add path-to-your-project.csproj
```

在本项目之中就是(在项目根目录下执行)：

```bash
dotnet sln myRecAll.sln add ./Contrib/TextItem.Api/TextItem.Api.csproj
```

看到输出“已将项目“Contrib/TextItem.Api/TextItem.Api.csproj”添加到解决方案中。”之后，可以发现 myRecAll.sln 文件内容发生了改变：

<!-- ![](https://s2.loli.net/2024/03/25/sYSIGtJaqne4XrL.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/sYSIGtJaqne4XrL.png" width="500" alt="Not find"/>
</div>

表示虚拟文件夹已经添加进来了。

<!-- ![](https://s2.loli.net/2024/03/25/waQ416PIuMWr9Zk.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/waQ416PIuMWr9Zk.png" width="500" alt="Not find"/>
</div>

现在我们就可以愉快地在项目之中构建代码了 😆

#### 方案 3：使用 C# Dev Kit 提供的解决方案资源管理器

回到我们刚刚创建一个空的解决方案的步骤：

```bash
./
└── myRecAll.sln
```

此时在资源管理器之中右键`.sln`文件，点击打开解决方案：

<!-- ![](https://s2.loli.net/2024/03/25/fsPLhTAJu6Qo4vx.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/fsPLhTAJu6Qo4vx.png" width="500" alt="Not find"/>
</div>

然后在解决方案之中点击右侧的**加号**➕，这里在截图的时候忘记把`Contrib`文件夹创建好了，在点之前记得创建以下 🥲

<!-- ![](https://s2.loli.net/2024/03/25/QbU3JlLhXBznuS1.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/QbU3JlLhXBznuS1.png" width="500" alt="Not find"/>
</div>

接下来的操作和我们在方案 1 之中讲的差不多，然后就可以非常方便地得到一个和方案 2 之中一样正确配置的项目文件夹了：

<!-- ![](https://s2.loli.net/2024/03/25/U7X6xSIYJie2gdl.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/U7X6xSIYJie2gdl.png" width="500" alt="Not find"/>
</div>

### 为项目添加 docker 支持

注意，这一部分的内容必须要依赖插件 Dev Containers 才能进行，没有安装的朋友现在立马去安装一个

<!-- ![](https://s2.loli.net/2024/03/25/w4hE56zskD3YTJx.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/w4hE56zskD3YTJx.png" width="500" alt="Not find"/>
</div>

在 vscode 之中按`F1`进入到命令中心，输入 docker，选择**Docker: Add Docker Files to Workspace**:

<!-- ![](https://s2.loli.net/2024/03/25/pJYQX8H2s7kod5h.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/pJYQX8H2s7kod5h.png" width="500" alt="Not find"/>
</div>

第一项是要我们选择项目的运行平台，当然无脑选择<strong>.NET: ASP.NET Core</strong>，这样在 Dockerfile 之中就会帮我们自动配置好相关 ASP.NET Core 依赖项的构建:

<!-- ![](https://s2.loli.net/2024/03/25/WN5cP7zKCAHvpXe.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/WN5cP7zKCAHvpXe.png" width="500" alt="Not find"/>
</div>

第二项是选择 docker 基础镜像的操作系统，这里选择 Linux：

<!-- ![](https://s2.loli.net/2024/03/25/dxupJ8EFAqHS5Q9.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/dxupJ8EFAqHS5Q9.png" width="500" alt="Not find"/>
</div>

第三项是选择 container 容器在运行时暴露的端口，这里可以让 vscode 帮我们默认选择一个，也可以自己配置

<!-- ![](https://s2.loli.net/2024/03/25/lWyszGvLqwB4bUX.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/lWyszGvLqwB4bUX.png" width="500" alt="Not find"/>
</div>

第四项是选择是否为项目生成可选的 docker-compose.yml 文件，因为是微服务，之后会涉及到多个容器的运行，所以这里当然选择`yes`

<!-- ![](https://s2.loli.net/2024/03/25/ZmJY1GFD65z2Lnb.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/ZmJY1GFD65z2Lnb.png" width="500" alt="Not find"/>
</div>

现在项目的文件结构应该是这样的：

<!-- ![](https://s2.loli.net/2024/03/25/nu3K6BWFOCxk4GV.png) -->
<div align="center">
    <img src="https://s2.loli.net/2024/03/25/nu3K6BWFOCxk4GV.png" width="500" alt="Not find"/>
</div>

比较遗憾的是，Dev Container 插件并不会帮我们自动生成`docker-compose.override.yml`文件，所以我们要自己手动创建(`touch`)一个

好，这样一个有 docker 支持的.NET 项目就配置完毕了。

### 为项目添加 package 依赖

目前在本项目之中一共使用到了下面这几个包，可以在项目的`.csproj`文件之中找到：

#### TextItem.Api:

<center>

| 名称                                    | 功能说明                  |
| --------------------------------------- | ------------------------- |
| Microsoft.EntityFrameworkCore           | 数据库访问框架            |
| Microsoft.EntityFrameworkCore.Design    | EF Core 设计时支持        |
| Microsoft.EntityFrameworkCore.Tools     | EF Core 工具              |
| Microsoft.EntityFrameworkCore.SqlServer | SQL Server 数据库提供程序 |
| AspNetCore.HealthChecks.SqlServer       | SQL Server 健康检查       |
| AspNetCore.HealthChecks.UI.Client       | 健康检查 UI 客户端        |
| Dapr.AspNetCore                         | Dapr 集成支持             |
| Dapr.Extensions.Configuration           | Dapr 配置扩展             |
| polly                                   | 弹性和瞬态故障处理库      |
| Serilog.AspNetCore                      | 日志记录框架              |
| Serilog.Sinks.Seq                       | Seq 日志记录器            |
| Swashbuckle.AspNetCore                  | Swagger 文档生成器        |
| TheSalLab.GeneralReturnValues           | 通用返回值类型            |

</center>

#### Infrastructure.Api:

<center>

| 名称                                          | 功能说明                             |
| --------------------------------------------- | ------------------------------------ |
| Dapr.Client                                   | Dapr 客户端库，用于与 Dapr 进行交互  |
| Microsoft.AspNetCore.OpenApi                  | ASP.NET Core 的 OpenAPI/Swagger 支持 |
| Microsoft.Extensions.Diagnostics.HealthChecks | ASP.NET Core 的健康检查支持          |
| Swashbuckle.AspNetCore                        | ASP.NET Core 的 Swagger 文档生成器   |

</center>

#### ServiceStatus

<center>

| 名称                                        | 功能说明             |
| ------------------------------------------- | -------------------- |
| AspNetCore.HealthChecks.UI                  | 健康检查 UI          |
| AspNetCore.HealthChecks.UI.InMemory.Storage | 健康检查 UI 内存存储 |
| Microsoft.AspNetCore.OpenApi                | OpenAPI/Swagger 支持 |
| Swashbuckle.AspNetCore                      | Swagger 文档生成器   |

</center>

#### 通过 dotnet CLI 使用 Nuget 来为项目添加依赖

实际上就是下面这个命令，具体可以参考[官方文档](https://learn.microsoft.com/zh-cn/dotnet/core/tools/dotnet-add-package)：

```bash
$ dotnet add package xxx
```

**_注意！_**

一定要`cd`到目标项目目录下再进行操作，或者在`add`和`package`之间添加项目的`.csproj`文件路径，不然 Nuget 找不到要操作的项目

值得一提的是 vscode 还有 dotnet CLI 因为自动生成模板的原因，这些项目在创建时都会自动添加`Microsoft.AspNetCore.OpenApi`还有`Sashbuckle.AspNetCore`，Infrastructure.Api 还有 ServiceStatus 都是服务容器，没有对外提供接口的话应该把这两个 Swagger 依赖删掉，说不定还能减少镜像体积。

## 补充说明

### 关于 docker 端口号

使用 container 的时候这个端口号的问题确实有点烦人，在这里解释一下：

我们先来看看自动生成*Dockerfile*的开头内容：

```Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5231

ENV ASPNETCORE_URLS=http://+:5231
```

这里有两处指定了端口号，第一处是`EXPOSE`，第二处定义了一个环境变量`ASPNETCORE_URLS`那么哪处会真正起作用呢？答案是第二处，这个`EXPOSE`实际上就是一个约定，并没有实质上的作用：

> `EXPOSE` 指令是声明容器运行时提供服务的端口，这只是一个声明，在容器运行时并不会因为这个声明应用就会开启这个端口的服务。在 Dockerfile 中写入这样的声明有两个好处，一个是帮助镜像使用者理解这个镜像服务的守护端口，以方便配置映射；另一个用处则是在运行时使用随机端口映射时，也就是 `docker run -P` 时，会自动随机映射 `EXPOSE` 的端口。

就算写了`EXPOSE`，那么在这个端口上到底有没有服务，那都是不知道的。

而`ASPNETCORE_URLS`是 ASP.NET Core 应用程序的环境变量，它用于指定应用程序要监听的 URL 地址。在这种情况下， `http://+:5231` 表示 ASP.NET Core 应用程序将监听所有网络接口 (`+`) 上的端口 5231。

也可以在`docker-compose.yml`文件之中进行指定：

```yaml
recall-textitemapi:
  environment:
    - ASPNETCORE_ENVIRONMENT=Development
    # - ASPNETCORE_HTTP_PORTS=8080
  ports:
    - "5163:5163"
    # - "35210:80"
    - "50001"
```

因为我已经在 Dockerfile 之中注明了`ASPNETCORE_HTTP_PORTS`服务端口，在这里就不用再重复指定了

### 关于 docker 环境变量`ASPNETCORE_ENVIRONMENT`

还记得在`Program.cs`文件之中的这一段代码吗：

```C#
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseCustomSwagger();
    app.MapGet("/", () => Results.LocalRedirect("~/swagger"));
}
```

`app.Environment.IsDevelopment()` 是一个 ASP.NET Core 中的方法，用于检查当前应用程序是否在开发环境中运行。

在 ASP.NET Core 中，你可以通过设置环境变量 `ASPNETCORE_ENVIRONMENT` 来指定应用程序的运行环境。这个环境变量的值可以是 `"Development"`、`"Staging"`、`"Production"` 或者你自定义的任何值。

这个值一定要在`Dockerfile`或者`docker-compose.yml`之中指定为`Development`！有些时候自动生成的配置文件并不会帮你指定这个环境变量，就会导致`if`过不去，进而导致**Swagger 页面无法打开**。
