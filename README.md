# Galaxy Network

Сетевая библиотека и примеры для игровых клиентов и серверов: готовые комплекты SDK, Unity-проект, сервер на .NET 8 и интеграция с UNIGINE.

## Быстрый старт

**Требования:** .NET 8 SDK и Unity **6.3 LTS / 6000.3.24f1**. Для UNIGINE отдельно требуется совместимый SDK движка.

```powershell
dotnet run --project Templates/SimpleMmoServer/SimpleMmoServer -c Release
```

Откройте `Templates/SimpleMmoClient` в Unity, загрузите `Assets/GalaxyNetwork/Scenes/Main.unity` и включите Play Mode. Адрес — `127.0.0.1:30200`. Демонстрационная авторизация разрешает логины, содержащие `test`.

Через интерфейс можно создавать комнаты и запускать примеры синхронизации объектов, пространственной видимости, сетевых переменных, физики и навигации.

## Возможности

- UDP-транспорт, гарантированная и негарантированная доставка, каналы и маршрутизация.
- Инстансы, участники и владельцы, сетевые сущности и полная синхронизация мира.
- Иерархические transform, сглаживание и серверное или клиентское управление движением.
- GalaxyVars, типизированные сообщения, битовый сериализатор, RPC и сетевое время.
- Опциональная физика Jitter2: raycast, triggers, joints и геометрия GPHB.
- Пространственные индексы и встроенные модели выбора алгоритмов в рантайме.
- Виртуальные подключения, повторы, диагностика и наблюдение за соединением.
- HTTP API администрирования сервера.

[Патчноут — RU](CHANGELOG.md) · [Changelog — EN](CHANGELOG.en.md) · [Производительность](PERFORMANCE.md)

## Структура

| Каталог | Содержимое |
|---|---|
| [Core](Core/README.md) | SDK для Unity, обычного .NET-клиента и сервера |
| [Templates/SimpleMmoClient](Templates/SimpleMmoClient) | Unity 6: адаптер, UI, сцены, префабы и примеры |
| [Templates/SimpleMmoServer](Templates/SimpleMmoServer/README.md) | Сервер .NET 8 и общие сообщения |
| [Templates/UnigineClient](Templates/UnigineClient/README.md) | .NET 8 и интеграция с UNIGINE |
| [Documentation](Documentation/README.md) | Установка, сериализация, физика и проверки |
| [build](build) | Обновление SDK и проверка поставки |

Клиентские DLL используют .NET Standard 2.1 и AOT-профиль для Unity/IL2CPP. Исполняемые C#-примеры используют .NET 8. Обновляйте клиент и сервер согласованным комплектом.

## Проверка сервера

```powershell
dotnet run --project Templates/SimpleMmoServer/SimpleMmoServer -c Release -- --port 30420 --smoke-test
```

Исходники ядра для готовых примеров не требуются. Если они доступны локально, обновите поставку через `build/update-sdk.ps1 -SourcePath <путь-к-исходникам>`.

## English quick start

Install .NET 8 SDK and Unity 6.3 LTS (`6000.3.24f1`). Run the server command above, open `Templates/SimpleMmoClient`, and play `Assets/GalaxyNetwork/Scenes/Main.unity`. The default endpoint is `127.0.0.1:30200`; demo authentication accepts a login containing `test`.

The SDK includes a Unity AOT client, a managed .NET client, the server core, and an optional Jitter2 backend. Keep client/server binaries in sync. UNIGINE requires its engine SDK and native deployment; see its template README for validation requirements.
