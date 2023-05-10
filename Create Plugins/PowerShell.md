# Создание своего плагина для Shark Remote с использованием PowerShell

### 1. Создайте папку с *названием в виде имени будущего плагина*
### 2. В данной папке (далее в гайде просто **main_folder**) (\*)
### 3. Создайте следующую структуру файлов:
 ```
{main_folder}\main.manifest
{main_folder}\main.ps1
```
### 4. В файл **main.manifest** (текстовый файл манифеста плагина с надстройками) вставьте следующий текст:
```
chat_action_type = 1
message_type = 1
arguments_count = 0
```

**chat_action_type** - тип действия, которое будет написано в чате во время работы файла

*Значения* 
 - 0 - ничего не писать
 - 1 - Бот набирает сообщение...
 
 **message_type** - тип возвращаемого сообщения в боте
 
*Значения* 
 - 0 - Текстовое сообщение
 - 1 - Текстовое сообщение с поддержкой HTML форматирования
 - 2 - Ничего не возвращать (*возможность добавлена в Shark Remote версии 4.3.1 Beta*)

 **arguments_count** - количество обязательных принимаемых аргументов плагина от пользователя
 
*Значения* 
 - От 0 до 4

### 5. Основной код необходимо написать в файле **main.ps1** (файл кода написанный для *PowerShell*). Можно создать и редактировать с помощью *Windows PowerShell ISE*, который встроен в Windows.

![image](https://user-images.githubusercontent.com/51060911/190862456-101a23fa-3ec2-4517-a5ab-86972b15b69c.png)


### 6. Вставьте в файл **main.ps1** следующий код:
```
#app {plugin_name}, Version="{plugin_version}", Author={author}, Command={call_command}

# Аргументы (опционально)
# $arg0 = $args[0] # Аргумент 1
# $arg1 = $args[1] # Аргумент 2
# $arg2 = $args[2] # Аргумент 3
# $arg3 = $args[3] # Аргумент 4
# Количество аргументов зависит от заданного количества аргументов из манифеста

# Write-Output Arguments: $arg0 $arg1 $arg2 $arg3 # Вывод аргументов
# Write-Output - вывод текста в бота по окончании работы плагина
# Вывод текста будет нормально работать в боте, только если текст написан на английском языке!

Write-Output Hello World!
```
### 7. Заполните данные (всё без пробелов):
- **{plugin_name}** - название плагина, такое же как для **{main_folder}**
- **{plugin_version}** - версия в формате x.x.x.x, где x - число (например: 1.0.0.4)
- **{author}** - автор плагина
- **{call_command}** - команда для вызова плагина (обязательно начинается с **/**), например: /hello

Должно получиться что-то похожее: ```#app HelloWorld, Version="1.0.0.0", Author=developer, Command=/hello```

**Данная строка является обязательной для файла плагина!**

### 8. Создайте zip архив с папкой **{main_folder}** (рекомендую воспользоваться архиватором [7-Zip](https://www.7-zip.org/) со сжатием **Нормальное**)

![image](https://user-images.githubusercontent.com/51060911/191972666-a2732f62-6bf0-4ff8-9e4c-eeaee52e6f08.png)

### 9. Переименуйте файл в **{main_folder}** и измените расширение архива на ```srp```

## Примечание

\* **{main_folder}** = **main_folder**. Все переменные в гайде обозначаются в **фигурных скобках**