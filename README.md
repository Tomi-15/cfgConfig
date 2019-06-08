# cfgConfig - .NET Configuration Framework
cfgConfig provides you a modern, fast and easy-to-use way to create
configurations files for your applications.

> It's cross-platform so you can use it in Xamarin, .NET Core, Mono and .NET Framework projects



## Features
- Secured configuration files using AES encryptation
- Backup system
- Auto save
- Different serialization methods
- Fully configurable
- MVVM compatible

## Installation
You can install the latest version of cfgConfig via **Nuget Package Manager**

``` Shell
PM nuget install
```

Optionally, you can specify that you will use a console to log errors and information messages:
``` csharp
ConfigurationManager.UseConsole();
```
Otherwise, exceptions will be thrown when any error occurs. You have to call it at the beginning of your program 

## Usage
Before start creating configurations, you need a Configuration Manager. Each Configuration Manager can handle one directory where all configuration files will be stored.

#### Create a Configuration Manager
---
To create a Configuration Manager, you will use the ```ConfigurableManager``` class and the ```Make(string, string)``` method, which take two arguments:

 `path` **String**
>**The path of the directory where the Configuration Manager will store configuration files**

`identifier` **String**
>**An unique identifier name for the manager**

``` csharp
var manager = ConfigurableManager.Make($"{Directory.GetCurrentDirectory()}\\Settings", "myManager")
                                             .Configure(settings =>
                                             {
                                                settings
                                                 .WithAutoSaveEach(TimeSpan.FromMinutes(30))
                                                 .WithSaveMode(SaveModes.Json);

                                             }).Build();
```