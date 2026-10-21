# Документация Galaxy

## Запуск

1. Установите .NET 8 SDK и Unity 6.3 LTS (`6000.3.24f1`).
2. Запустите сервер из корня репозитория:

   ```powershell
   dotnet run --project Templates/SimpleMmoServer/SimpleMmoServer -c Release
   ```

3. Откройте Unity-проект `Templates/SimpleMmoClient`, сцену `Assets/GalaxyNetwork/Scenes/Main.unity` и включите Play Mode.
4. Для демонстрационной авторизации используйте логин, содержащий `test`.
5. Создайте комнату нужного типа. Адрес по умолчанию — `127.0.0.1:30200`, имя приложения — `SimpleMmoServer`.

Авторизация демонстрационная: пароль не проверяется по базе пользователей. Для реальной игры замените обработчик проверки.

## Руководства

- [Комплекты SDK и обновление DLL](../Core/README.md)
- [Сервер и примеры](../Templates/SimpleMmoServer/README.md)
- [Клиент UNIGINE](../Templates/UnigineClient/README.md)
- [Сериализация](Serialization.md)
- [Серверная физика](../Core/Server/Physics/README.md)
- [Патчноут](../CHANGELOG.md)
- [Производительность](../PERFORMANCE.md)
- [Обновление комплекта и результаты проверок](Validation.md)

## Общие сообщения и IL2CPP

`Templates/SimpleMmoServer/SimpleMmoCommon` содержит общие типы сообщений. После их изменения пересоберите DLL и обновите Unity через `build/update-sdk.ps1`.

Плагины лежат в `Assets/GalaxyNetwork/Core/Plugins/netstandard2.1`. Reflection-модели сохраняются от stripping через `Assets/GalaxyNetwork/Resources/link.xml`. Не добавляйте второй комплект SDK в другие папки Assets.

## Физика

Сервер регистрирует Jitter2 до `GalaxyCore.Start`, комнаты запускают собственный мир. Коллайдеры Unity экспортируются в GPHB через меню Galaxy физики; команда и пути описаны в серверном README. Сервер загружает геометрию из `Content/physics`. Координаты используют float; старый PhysBake не является новым форматом.

## Проверки

```powershell
dotnet run --project Templates/SimpleMmoServer/SimpleMmoServer -c Release -- --port 30420 --smoke-test
```

Unity batch-методы: `GalaxyMigrationChecks.Audit`, `GalaxyMigrationChecks.BuildWindowsMono`, `GalaxyMigrationChecks.BuildWindowsIl2Cpp`. Передавайте их редактору через `-executeMethod`. Отчёты и сборки сохраняются в локальной `.artifacts`, исключённой из Git.

Готовые команды из корня репозитория:

```powershell
.\build\verify-sdk.ps1
.\build\unity.ps1 -Action Audit
.\build\unity.ps1 -Action Mono
.\build\unity.ps1 -Action Il2Cpp
.\build\smoke-players.ps1
```

Последняя команда поднимает временный локальный сервер на свободном UDP-порту 30421, запускает оба собранных клиента и останавливает созданные ею процессы. Она проверяет авторизацию, создание комнаты и вход, список комнат, синхронизацию мира, привязку префаба, владельца и начальные/последующие GalaxyVars. Можно выбрать `-Backend Mono` или `-Backend Il2Cpp`, а также другой `-Port`.

Для проверки переходов между сценами выполните `build/smoke-players.ps1 -AddForce`. Этот сценарий дважды создаёт комнату Add Force, проверяет появление нескольких сетевых сфер и обновления движения, затем возвращается в Main. Он также проверяет принадлежность объектов игровой сцене и удаление старых представлений при выходе.

## Навигационные данные

Готовые небольшие карты для клиентских примеров хранятся в `Assets/GalaxyNetwork/Core/Alpha/Navigation/Data`. Сервер получает соответствующий bake `ExampleNav` из `Content/navigation/NavigationExample.gnm`.

Большая сцена редактора `Core/Alpha/Navigation/Scenes/Navigation.unity` хранит в Git геометрию и компактную конфигурацию слоёв. Её сгенерированная карта записывается в `.artifacts/navigation/NavigationAuthoring.gnm`; при первом Play в редакторе без кэша карта запекается автоматически. Первое запекание этой большой сцены может занять несколько минут.

При изменении геометрии выполните `build/unity.ps1 -Action Navigation`, затем скопируйте обновлённый `ExampleNav.gnm.bytes` в серверный `Content/navigation/NavigationExample.gnm` и пересоберите сервер.
