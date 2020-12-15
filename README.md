<<<<<<< HEAD
Galaxy Network is a network solution designed primarily for use in game engines such as Unity, Unigine, Stride (Xenko). However, it is possible to use in any other environments supporting net Standard 2.0.
The solution consists of a client and server part, both presented as a set of libraries connected to the project. The Galaxy network is capable of operating both in an authoritarive mode and in relay mode. Below is a set of currently supported functionality.

Instructions for installing https://youtu.be/eo6nW2DM0TE

Network: Authorization/registration. Accounting for network connections. Automatic work with NAT. Automatic connection recovery. Support for traffic encryption. Protection from packet spoofing. Support messages with and without delivery guarantee, with sorting, with order. Support for an automatic drop of out-of-date messages. Routing (server, client, instances, essence).

Instances (rooms, worlds, locations): Creation. Getting a list. Instances manager. Entrance/exit. Sending a message to all customers of the instances. Customer management support. Automatic and manual host transmission for non-authoritarian or partially authoritarian logic. Full support for authoritarian logic. Physics support. Support network frames from (1 to 120 network phps). The ability to set a password to the login. The ability to create invisible instances. Support huge locations with network visibility.

Physics: support for primitive colliders. Support for the Terrain Collider. a toolkit for baking stage colliders for the server. ray cast with tag and dynamic features support. Support for line and angular acceleration.

Network entities: Create/delete. Support for server physics. Customer management support. The ability to transfer an object to another player, or transfer to an authoritarian model. The ability to balance objects between customers. Support for Authoritarian Server Management. The ability to directly exchange messages between instances of the entity. The function of creating and deleting an entity (z go). Built-in position and orientation synchronization functionality in space.

Other features: Built-in protobuf support. Built-in Bit serializer.

Supported backend: Support mono. Support il2cpp. Meets net standard 2.0 specifications. Support ECS, Jobs, Dots. Possible use outside of Unity. Support for Stride (Xenko). Unigine engine support.

Server-supported platforms: Windows. Linux. Macos.

Supported and tested platforms (for the customer): Windows. Linux. MaxOS. Android. Ios. In theory, other platforms should be supported, with the exception of WebGL.

In alpha: Synchronizing the parameters of the animator. Server Search for a Path

Total:

The solution is distributed as an open source template and a set of closed libraries. There is no direct dependence on the type of database, you can use any .net supported

Installation Instructions https://youtu.be/sDxyENBw77E

Galaxy Network is a network solution designed primarily for use in game engines such as Unity, Unigine, Stride (Xenko). However, it is possible to use in any other environment supporting C# net Standard 2.0+. The solution consists of client and server parts, both are presented as a set of libraries connected to the project. Galaxy network is able to work both in authoritarive mode and in repeater mode. Below is a set of currently supported features.

Net: Authorization / registration. Accounting for network connections. Automatic work with NAT. Auto reconnect. Support traffic encryption. Protection against packet spoofing. Support for messages with and without delivery guarantee, with sorting, with priority. Support for automatic drop outdated messages. Routing (server, client, instance, entity).

Instances (rooms, worlds, locations): Creature. Getting the list. Instance manager. Enter exit. Sending a message to all instance clients. Support customer management. Automatic and manual host transfer for non-authoritarian or partially authoritarian logic. Full support for authoritarian logic. Physics support. Support for network frames from (1 to 120 network FPS). Ability to set a password for entry. Ability to create invisible instances.

Physics: support for primitive colliders. terrane collider support. tools for baking scene colliders for the server. ray cast with support for tags and dynamic objects. support for linear and angular acceleration.

Network entities (network objects): Create / delete. Support for server physics. Customer Management Support. The ability to transfer the object to another player, or transfer to an authoritarive model. The ability to balance objects between customers. Support for Authoritarian server management. The ability to directly exchange messages between instances of the entity. Functionality for creating and deleting an entity (+ go). Built-in functionality for synchronizing position and orientation in space.

Other functionality: Native protobuf support. Built-in Bit serializer.

Supported backend: Mono support. Support for il2cpp. Complies with net standard 2.0 specification. Support ECS, Jobs, Dots. Use outside Unity is possible. Support for Stride (Xenko). Experimental support for the Unigine engine.

Platforms supported by the server: Windows Linux MacOS

Supported and tested platforms (for the client): Windows Linux MaxOS. Android iOS In theory, other platforms other than WebGL should be supported.

In alpha: Sync animator settings.
=======
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
Экспериментальная поддержка движка Unigine.

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

Общее:

Решение распространяется в виде Шаблона с открытым кодом, и набором закрытых библиотек.
Отсутствует прямая зависимость к типу базы данных, можно использовать любую поддерживаемую .net



Installation Instructions 
https://youtu.be/sDxyENBw77E

Galaxy Network is a network solution designed primarily for use in game engines such as Unity, Unigine, Stride (Xenko). However, it is possible to use in any other environment supporting c # net Standart 2.0+.
The solution consists of client and server parts, both are presented as a set of libraries connected to the project. Galaxy network is able to work both in authoritarian mode and in repeater mode. Below is a set of currently supported features.



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
>>>>>>> parent of 6dfef60... Update README.md

General:

The solution is distributed in the form of an open source template, and a set of private libraries. There is no direct dependence on the type of database, you can use any supported .net
