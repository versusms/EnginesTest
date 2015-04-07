# EnginesTest
Test Application for IFMO (web-programming)

## Установка
Скачайте проект и запустите его в Visual Studio 2012/2013

## Настройка подключения к БД
Варианты подключения к БД:
* LocalDB-провайдер
* Удаленный MSSQL-сервер (включая SQLEXPRESS)

Выберите вариант подключения, раскомментируйте соответствующую строку в файле Models/DataModel.Context.cs :
> : base("name=LocalDataModelContainer")

для подключения к LocalDB (используется встроенная авторизация)

> : base("name=RemoteDataModelContainer")

для подключения к удаленному MSSQL-сервер (требуется настроить авторизацию в файле Web.config)

## Создание таблиц
SQL-cкрипт создания таблиц в файле Models/DataModel.edmx.sql
Имя БД - EnginesTest

## Первичное заполнение БД
Изначально таблицы с данными пустые. Для заполнения таблиц данными запустите проект и вызовите URL:
> http://&lt;hostname&gt;:&lt;port&gt;/init

Все вставляемые данные можно посмотреть в контроллере InitController