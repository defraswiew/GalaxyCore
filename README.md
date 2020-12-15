Galaxy Network это сетевое решение предназначенное в первую очередь, для применения в игровых движках таких как Unity, Unigine, Stride (Xenko). Однако возможно использование в любых других средах поддерживающих c# net Standart 2.0+.  
Решение состоит из клиентской и серверной части, обе представлены в виде подключаемого к проекту набора библиотек.  Galaxy network способен работать как в авторитарном режиме, так и в режиме ретранслятора.  Ниже представлен набор поддерживаемого на данный момент функционала.

Инструкция по установке https://youtu.be/eo6nW2DM0TE

Сеть:
Авторизация / регистрация.
Учет сетевых соединений.
Автоматическая работа с NAT.
Автоматическое восстановление соединения.
Поддержка шифрования трафика.
Защита от подмены пакетов.
Поддержка сообщений с гарантией доставки и без, с сортировкой, с очередностью.
Поддержка автоматического дропа устаревших сообщений.
Роутинг ( сервер, клиент, инстанс, сущность).

Инстансы (комнаты, миры, локации):
Создание. 
Получение списка . 
Менеджер инстансов.
Вход / выход.
Рассылка сообщения всем клиентам инстанса.
Поддержка управление клиентом.
Автоматическая и ручная передача хоста для не авторитарной или частично авторитарной логики.
Полная поддержка авторитарной логики.
Поддержка физики.
Поддержка сетевых фреймов от (1 до 120 сетевых фпс).
Возможность установить пароль на вход.
Возможность создавать невидимые инстансы.
Поддержка огромный локаций с сетевой видимостью.


Физика:
поддержка примитивных коллайдеров.
поддержка террейн коллайдера.
инструментарий по запеканию коллайдеров сцены для сервера.
ray cast с поддержкой тегов и динамических объектов.
поддержка работы с линейным и угловым ускорением.

Сетевые сущности (сетевые объекты):
Создание / удаление.
Поддержка серверной физикой.
Поддержка управления клиентом.
Возможность передачи объекта другому игроку, или перевод на авторитарную модель.
Возможность балансировки объектов между клиентами.
Поддержка Авторитарного управления сервером.
Возможность прямого обмена сообщений между экземплярами сущности.
Функционал создания и удаления сущности (+ go).
Встроенный функционал синхронизации положения и ориентации в пространстве.

Прочий функционал:
Встроенная поддержка protobuf.
Встроенный Bit сериализатор.

Поддерживаемый бекенд:
Поддержка mono.
Поддержка il2cpp.
Соответствует спецификации net standart 2.0.
Поддержка ECS, Jobs, Dots.
Возможно использование вне Unity.
Поддержка Stride (Xenko).
Поддержка движка Unigine.

Поддерживаемые сервером платформы:
Windows.
Linux.
MacOS.

 Поддерживаемые и протестированные платформы (для клиента):
Windows.
Linux.
MaxOS.
Android.
iOS.
 В теории должны поддерживаться и иные платформы за исключением WebGL.

В альфе:
Синхронизация параметров аниматора.
Серверный поиск пути

Общее:

Решение распространяется в виде Шаблона с открытым кодом, и набором закрытых библиотек.
Отсутствует прямая зависимость к типу базы данных, можно использовать любую поддерживаемую .net



Installation Instructions 
https://youtu.be/sDxyENBw77E

Galaxy Network is a network solution designed primarily for use in game engines such as Unity, Unigine, Stride (Xenko). However, it is possible to use in any other environment supporting C# net Standard 2.0+.
The solution consists of client and server parts, both are presented as a set of libraries connected to the project. Galaxy network is able to work both in authoritive mode and in repeater mode. Below is a set of currently supported features.



Net:
Authorization / registration.
Accounting for network connections.
Automatic work with NAT.
Auto reconnect.
Support traffic encryption.
Protection against packet spoofing.
Support for messages with and without delivery guarantee, with sorting, with priority.
Support for automatic drop outdated messages.
Routing (server, client, instance, entity).

Instances (rooms, worlds, locations):
Creature.
Getting the list.
Instance manager.
Enter exit.
Sending a message to all instance clients.
Support customer management.
Automatic and manual host transfer for non-authoritarian or partially authoritarian logic.
Full support for authoritarian logic.
Physics support.
Support for network frames from (1 to 120 network FPS).
Ability to set a password for entry.
Ability to create invisible instances.


Physics:
support for primitive colliders.
terrane collider support.
tools for baking scene colliders for the server.
ray cast with support for tags and dynamic objects.
support for linear and angular acceleration.

Network entities (network objects):
Create / delete.
Support for server physics.
Customer Management Support.
The ability to transfer the object to another player, or transfer to an authoritarian model.
The ability to balance objects between customers.
Support for Authoritarian server management.
The ability to directly exchange messages between instances of the entity.
Functionality for creating and deleting an entity (+ go).
Built-in functionality for synchronizing position and orientation in space.

Other functionality:
Native protobuf support.
Built-in Bit serializer.

Supported backend:
Mono support.
Support for il2cpp.
Complies with net standart 2.0 specification.
Support ECS, Jobs, Dots.
Use outside Unity is possible.
Support for Stride (Xenko).
Experimental support for the Unigine engine.

Platforms supported by the server:
Windows
Linux
MacOS

 Supported and tested platforms (for the client):
Windows
Linux
MaxOS.
Android
iOS
 In theory, other platforms other than WebGL should be supported.

In alpha:
Sync animator settings.

General:

The solution is distributed in the form of an open source template, and a set of private libraries.
There is no direct dependence on the type of database, you can use any supported .net version.
