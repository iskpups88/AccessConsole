using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AccessConsole.Enums;
using AccessConsole.Extensions;

namespace AccessConsole
{
    public class Program
    {
        private const string AdminName = "Ivan";

        private static readonly HashSet<string> Subjects = new() { AdminName, "Sergey", "Boris", "Alexander" };

        private static readonly Dictionary<string, AccessTypes> AccessTypesCommand = GetCommands();

        private static readonly Dictionary<Key, AccessTypes> AccessMatrix = GetMatrix();

        static void Main(string[] args)
        {
            while (true)
            {
                string user;
                Console.WriteLine("Введите идентификатор пользователя:");
                var command = Console.ReadLine()?.Trim();

                if (command == "quit")
                    return;

                if (Subjects.TryGetValue(command, out user))
                {
                    Console.WriteLine($"User: {user}");
                    Console.WriteLine("Идентификация прошла успешно, добро пожаловать в систему");
                    Console.WriteLine("Перечень ваших прав:");

                    var objectDictionary = PrintAccesses(user);
                    WaitForActions(user, objectDictionary);
                }
                else
                {
                    Console.WriteLine("Неверный идентификатор пользователя");
                }
            }
        }

        private static void WaitForActions(string user, Dictionary<int, Objects> objectDictionary)
        {
            while (true)
            {
                Console.Write("Жду ваших указаний > ");
                var command = Console.ReadLine()?.Trim();

                if (command == "quit")
                {
                    Console.WriteLine($"Работа пользователя {user} завершена. До свидания");
                    return;
                }

                AccessTypes accessCommand;
                while (!AccessTypesCommand.TryGetValue(command, out accessCommand))
                {
                    Console.WriteLine("Ошибка ввода. Введите корректную команду.");
                    Console.WriteLine("Жду ваших указаний > ");
                    command = Console.ReadLine()?.Trim();
                }

                if (accessCommand.HasFlag(AccessTypes.Grant))
                {
                    var currentObject = GetCurrentObject(objectDictionary, "Право на какой объект передается?");
                    var key = new Key(user, currentObject);

                    if (AccessMatrix.TryGetValue(key, out var accessType))
                    {
                        if (!accessType.HasFlag(accessCommand)) Console.WriteLine("Отказ в выполнении операции. У Вас нет прав для ее осуществления");

                        Console.WriteLine("Какое право передается?");
                        command = Console.ReadLine()?.Trim();

                        while (!AccessTypesCommand.TryGetValue(command, out accessCommand))
                        {
                            Console.WriteLine("Некорректная команда");
                            Console.WriteLine("Какое право передается?");
                            command = Console.ReadLine()?.Trim();
                        }

                        Console.WriteLine("Какому пользователю передается право?");
                        command = Console.ReadLine()?.Trim();

                        while (!Subjects.TryGetValue(command, out user))
                        {
                            Console.WriteLine("Неккоректный пользователь");
                            Console.WriteLine("Какому пользователю передается право?");
                            command = Console.ReadLine()?.Trim();
                        }

                        key = new Key(user, currentObject);
                        AccessMatrix[key] |= accessCommand;
                        Console.WriteLine("Операция прошла успешно");
                    }
                }
                else
                {
                    var currentObject = GetCurrentObject(objectDictionary, "Над каким объектом производится операция?");
                    var key = new Key(user, currentObject);
                    var access = AccessMatrix[key];

                    if (access.HasFlag(accessCommand))
                        Console.WriteLine("Операция прошла успешно");
                    else
                        Console.WriteLine("Отказ в выполнении операции. У Вас нет прав для ее осуществления");
                }
            }
        }

        private static int GetIndexOfCurrenObject(string message)
        {
            Console.WriteLine(message);
            var command = Console.ReadLine()?.Trim();
            int objectIndex;
            while (!int.TryParse(command, out objectIndex))
            {
                Console.WriteLine("Неправильный идентификатор объекта");
                Console.WriteLine(message);
                command = Console.ReadLine()?.Trim();
            }

            return objectIndex;
        }

        private static Objects GetCurrentObject(Dictionary<int, Objects> objectDictionary, string message)
        {
            var index = GetIndexOfCurrenObject(message);

            Objects currentObject;
            while (!objectDictionary.TryGetValue(index, out currentObject))
            {
                index = GetIndexOfCurrenObject(message);
            }

            return currentObject;
        }

        private static Dictionary<int, Objects> PrintAccesses(string currentUser)
        {
            var printedObjects = new Dictionary<int, Objects>();
            var currentObjectIndex = 1;

            foreach (var item in Enum.GetValues<Objects>())
            {
                if (AccessMatrix.TryGetValue(new Key(currentUser, item), out var accessType))
                {
                    var objectName = item.GetAttribute<DisplayAttribute>().Name;
                    var accessName = accessType.GetAttribute<DisplayAttribute>().Name;
                    Console.WriteLine($"{currentObjectIndex}. {objectName}: {accessName}");
                    printedObjects.Add(currentObjectIndex, item);
                    currentObjectIndex++;
                }
            }

            return printedObjects;
        }

        private static Dictionary<Key, AccessTypes> GetMatrix()
        {
            var random = new Random();
            var accesses = Enum.GetValues<AccessTypes>();
            var accessMatrix = new Dictionary<Key, AccessTypes>();

            foreach (var subject in Subjects)
            {
                foreach (var item in Enum.GetValues<Objects>())
                {
                    if (subject == AdminName)
                    {
                        accessMatrix.Add(new Key(subject, item), AccessTypes.All);
                    }
                    else
                    {
                        var randomIndex = random.Next(0, accesses.Length - 1);
                        accessMatrix.Add(new Key(subject, item), accesses[randomIndex]);
                    }
                }
            }

            return accessMatrix;
        }

        private static Dictionary<string, AccessTypes> GetCommands()
        {
            var accesses = Enum.GetValues<AccessTypes>();
            var accessesDictionary = new Dictionary<string, AccessTypes>();

            foreach (var accessType in accesses)
                accessesDictionary.Add(accessType.GetAttribute<DisplayAttribute>().Name!, accessType);

            return accessesDictionary;
        }
    }
}