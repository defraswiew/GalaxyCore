# Galaxy SDK

Готовые согласованные комплекты актуальной Galaxy для примеров этого репозитория.

| Каталог | Назначение |
|---|---|
| `Client/netstandard2.1` | Unity/IL2CPP, профиль `UnityRelease`, float, `GALAXY_AOT` |
| `Client/Managed/netstandard2.1` | Обычный .NET-клиент, профиль `Release`, float |
| `Server/netstandard2.1` | Серверная библиотека, профиль `Release`, float |
| `Server/Physics/net8.0` | Опциональный адаптер Jitter2 для .NET 8 |

Исполняемые C#-примеры используют .NET 8. Библиотеки клиента, общего протокола и ядра сервера сохраняют .NET Standard 2.1 для совместимости с Unity 6.

Копируйте комплект целиком. Клиент и сервер используют разные профили исполнения, но одинаковую модель координат float и согласованный сетевой протокол. Старые DLL из `netstandard2.0` заменены.

`SDK-MANIFEST.json` содержит ревизию исходников, профили и SHA-256 файлов. Обновление из локальных исходников:

```powershell
.\build\update-sdk.ps1 -SourcePath "C:\path\to\galaxycore"
```

Скрипт отключает личные post-build пути исходного проекта, собирает отдельные комплекты и обновляет shared-модели и плагины Unity. Исходники библиотеки для использования готовых примеров не требуются.

В Unity установлены по одной копии GalaxyCoreLib, GalaxyCoreCommon, protobuf-net и YamlDotNet. Newtonsoft.Json предоставляется официальным Unity-пакетом; сервер и обычный .NET-клиент получают его в своём комплекте DLL.

- [Документация](../Documentation/README.md)
- [Физика Jitter2](Server/Physics/README.md)
- [Зависимости](ThirdParty/README.md)
