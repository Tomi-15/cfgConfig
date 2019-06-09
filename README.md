# cfgConfig - .NET Configuration Framework
cfgConfig provides you a modern, fast and easy-to-use way to create
configurations files for your applications.

> It's cross-platform so you can use it in Xamarin, .NET Core, Mono and .NET Framework projects

[![Build Status](https://img.shields.io/travis/Tomi-15/cfgConfig.svg?style=for-the-badge)](https://travis-ci.com/Tomi-15/cfgConfig)
[![License](https://img.shields.io/badge/license-GNU%20GPLv3-blue.svg?style=for-the-badge)](https://github.com/Tomi-15/cfgConfig/blob/master/LICENSE.txt)
[![OpenIssues](https://img.shields.io/github/issues-raw/Tomi-15/cfgConfig.svg?style=for-the-badge)](https://github.com/Tomi-15/cfgConfig/issues)
[![Discord](https://img.shields.io/badge/Discord-Tomas%238453-orange.svg?style=for-the-badge&logo=discord)]


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

### **Building a Configuration Manager**

#### Create a Configuration Manager
---
To create a Configuration Manager, you will use the ```ConfigurableManager``` class and the ```Make(string, string)``` method, which take two parameters:

 `path` **String**
>The path of the directory where the Configuration Manager will store configuration files

`identifier` **String**
>An unique identifier name for the manager

``` csharp
var preManager = ConfigurableManager.Make("AppData\\Roaming\\MyApp\\Settings", "myManager");
```

#### Configuring the Configuration Manager
---
To configure the manager, you have to use the ```Configure(Action<ConfigurationManagerSettings>)``` method, which take one parameter:

`settings` **Action{T}** *where T is ConfigurationManagerSettings*
>The action that is used to configure the manager

```csharp
preManager.Configure(settings => 
{
    settings.WithSaveMode(SavesMode.Json)
            .WithAutoSaveEach(TimeSpan.FromMinutes(5))
            .Encrypt();
});
```

The ```ConfigurationManagerSettings``` provides methods to configure the Configuration Manager to your needs:

- `WithSaveMode(SavesMode)` *Specifies the serialization method that the manager will use to serialize configurations*
- `WithAutoSaveEach(TimeSpan)` *Indicates the manager to save all the configurations in specified interval of time*
- `ConfigureBackups()` *Configures the Backup System for the Configuration Manager*
- `Encrypt()` *Enables encryption so all configuration files will be encrypted before being saved*

#### Building the Configuration Manager
---
After configuring the manager, you are ready to build it and start implementing configurations. For that, you will use the method `Build()` which returns an instance of the created `ConfigurationManager`
```csharp
ConfigurationManager myManager = preManager.Build();
```

#### Closing the Manager
---
To save all configurations and unload all Configuration Managers created, until your program ends you have to call the static method `Terminate()` found in the `ConfigurationManager` class:
```csharp
ConfigurationManager.Terminate();
```
> If its not called, it can cause bugs and data loss

### **Creating and implementing configurations**

#### Configure a class
---
First or all, you need to mark a class to act as a Configuration, and you do that by adding the `Config` attribute to the class:
```csharp
[Config]
public class Settings1
{
    public bool DarkModeEnabled { get; set; }

    public string Username { get; set; }

    public int MaxLoginAttemps { get; set; } = 5;
}
```
Optionally, you can define a custom name for the class by adding the `Name` property to the attribute:
```[Config(Name = "UserSettings")]```
> If it's not specified, the name of the class will be used as the name of the Configuration

#### Implementing the configuration
---
Once you have configured the class that will act as Configuration, you are ready to implement it by calling the `Implement<T>()` method, from the `Implementations` property found in the `ConfigurationManager` class:
```csharp
myManager.Implementations.Implement<Settings1>();
```
> You can use the non-generic method if you want: ```Implement(Type)``` which takes the type of your Configuration class

### **Using the configurations**

#### Get a configuration
---
To get a Configuration, you have to call the method `GetConfig<T>` from the `ConfigurationManager` class:
```csharp
var settings1 = myManager.GetConfig<Settings1>();
```
It will throw an `ConfigNotFoundException` if the Configuration is invalid or is not implemented.
> You can use the non-generic method if you want: `GetConfig(Type)` which takes the type of your Configuration class

#### Saving configurations
---
When you make a change in your Configuration class, you can wait for the auto save (if its configured), wait until the application gets closed, or you can save it by calling the extension method `SaveConfig<TConfig>(TConfig)` which takes one parameter:

 `config` **TConfig**
>The Configuration class that is going to be saved

```csharp
config.SaveConfig();
```
It will throw an `ConfigNotFoundException` if the Configuration is invalid or is not implemented.

### **Summary**
```csharp
// Console app's entry point
static void Main(string[] args)
{
    ConfigurationManager.UseConsole();

    // Create the manager
    var manager = ConfigurableManager.Make("AppData\\Roaming\\MyApp\\Settings", "myManager")
                                        .Configure(settings => // Configure manager
                                        {
                                            settings
                                                .WithAutoSaveEach(TimeSpan.FromMinutes(30))
                                                .WithSaveMode(SaveModes.Json);

                                        }).Build(); // Build it

    // Implement a configuration of type MySettings
    manager.Implementations.Implement<MySettings>();

    // Get the configuration
    var config = manager.GetConfig<MySettings>();

    // Print some values
    Console.WriteLine($"{config.DarkModeEnabled} : {config.Username}");

    // Wait until user input
    Console.ReadLine();

    // Terminate the session
    ConfigurationManager.Terminate();
}
```

### Using the Backup system
---
If you didn't enabled while configuring the manager, enable it by calling the `ConfigureBackups()` method.
> More information above

When you enable backups, at the time the application gets closed, and all configurations are saved, backups of them are made, encrypted and saved in a directory called Backups, located at the working path of the Configuration Manager specified.

If something happens to your configuration files, you can have to call **ONE TIME** the method `RestoreLastBackup()` from the `ConfigurationManager` instance after implementing your configurations.

```csharp
myManager.RestoreLastBackup();
```

After restoring, you have to eliminate that line from your program's code.

## License
Licensed under **GNU General Public License v3.0**

>For more information read LICENSE.txt

## Donate
[Buy me](https://www.paypal.me/tomasweg15?locale.x=es_XC) a Fernet ;)
